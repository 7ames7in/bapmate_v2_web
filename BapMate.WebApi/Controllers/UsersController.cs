using BapMate.Infrastructure.Data;
using BapMate.WebApi.Infrastructure;
using BapMate.WebApi.Models;
using BapMate.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;


namespace BapMate.WebApi.Controllers
{
    using BapMate.Infrastructure.Services;
    using BapMate.Application.Features;
    using Microsoft.Extensions.Configuration;
    [ApiController]
    [Route("api/[controller]")]

public class UsersController : ControllerBase
{
    private readonly BapMateDbContext _context;
    private readonly SmsService _smsService;
    private readonly UserRegistrationNotifier _notifier;

    public UsersController(BapMateDbContext context, IConfiguration configuration)
    {
        _context = context;
        _smsService = new SmsService(configuration);
        _notifier = new UserRegistrationNotifier(_smsService);

    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Ok(users.Select(ToUserProfileDto));
    }

    public sealed class SocialRegisterRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string? Phone { get; set; }
        public string? Carrier { get; set; }
        public string? Gender { get; set; }
        public string? Bio { get; set; }
    }

    private static readonly MatchPreferencesPayload EmptyMatchPreferences = new([], [], []);
    private static readonly DefaultGameSettingsPayload EmptyGameSettings = new([], []);

    private sealed record MatchPreferencesPayload(
        IReadOnlyCollection<string> PreferredTimeSlots,
        IReadOnlyCollection<string> PreferredPaymentTypes,
        IReadOnlyCollection<string> PreferredInterests);

    private sealed record DefaultGameSettingsPayload(
        IReadOnlyCollection<string> DefaultMissions,
        IReadOnlyCollection<string> DefaultCosts);

    private UserProfileDto ToUserProfileDto(User entity)
    {
        var badges = JsonContentHelper.DeserializeList<string>(entity.BadgesJson);
        var matchPreferences = JsonContentHelper.DeserializeObject<MatchPreferencesPayload>(entity.MatchPreferencesJson) ?? EmptyMatchPreferences;
        var defaultSettings = JsonContentHelper.DeserializeObject<DefaultGameSettingsPayload>(entity.DefaultGameSettingsJson) ?? EmptyGameSettings;

        return new UserProfileDto(
            entity.Id,
            entity.Name,
            entity.Email,
            entity.Avatar,
            entity.Bio,
            entity.ReliabilityScore,
            entity.WalletBalance,
            entity.EscrowBalance,
            badges,
            new MatchPreferencesDto(matchPreferences.PreferredTimeSlots, matchPreferences.PreferredPaymentTypes, matchPreferences.PreferredInterests),
            new DefaultGameSettingsDto(defaultSettings.DefaultMissions, defaultSettings.DefaultCosts),
            entity.Gender,
            entity.BirthYear,
            entity.Phone);
    }

    public class LoginRequest
    {
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Identifier))
        {
            return BadRequest(new { success = false, error = "아이디를 입력해주세요." });
        }

        var normalizedIdentifier = request.Identifier.Trim();
        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Email == normalizedIdentifier || u.Id == normalizedIdentifier || u.Phone == normalizedIdentifier || u.Username == normalizedIdentifier, cancellationToken);

        if (user is null)
        {
            return NotFound(new { success = false, error = "존재하지 않는 사용자입니다." });
        }

        // Bypass password check for specific test users
        if (string.Equals(user.Email, "7ames7in@gmail.com", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(user.Id, "kakao-4513767160", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(user.Phone, "01075011346", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(ToUserProfileDto(user));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { success = false, error = "비밀번호를 입력해주세요." });
        }

        // Auto-set: if user's password in DB is empty, set it on first login!
        if (string.IsNullOrEmpty(user.Password))
        {
            user.Password = request.Password;
            user.PasswordHash = request.Password; // Also sync password hash
            await _context.SaveChangesAsync(cancellationToken);
        }
        else if (user.Password != request.Password)
        {
            return BadRequest(new { success = false, error = "비밀번호가 일치하지 않습니다." });
        }

        return Ok(ToUserProfileDto(user));
    }

    public class EmailRegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int BirthYear { get; set; }
    }

    [HttpPost("email-register")]
    public async Task<IActionResult> RegisterFromEmail([FromBody] EmailRegisterRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest("필수 정보 누락");
        }

        var normalizedEmail = request.Email.Trim();
        var normalizedPhone = request.Phone.Trim();

        var exists = await _context.Users.AnyAsync(u => u.Email == normalizedEmail || u.Phone == normalizedPhone, cancellationToken);
        if (exists)
        {
            return Conflict("이미 가입된 이메일 또는 휴대폰 번호입니다.");
        }

        var id = $"email-{Guid.NewGuid():N}";
        var avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(request.Name)}";
        var user = new User
        {
            Id = id,
            Username = normalizedEmail,
            PasswordHash = request.Password, // Using password directly as hash for now
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            Password = request.Password,
            Gender = string.IsNullOrWhiteSpace(request.Gender) ? "male" : request.Gender.Trim(),
            BirthYear = request.BirthYear,
            Avatar = avatar,
            Bio = $"밥메이트 회원 ({request.Gender})",
            Phone = normalizedPhone,
            Carrier = "이메일",
            ReliabilityScore = 88,
            WalletBalance = 50000m,
            EscrowBalance = 0m,
            BadgesJson = JsonContentHelper.Serialize(new[] { "일반 인증" }),
            MatchPreferencesJson = JsonContentHelper.Serialize(new
            {
                preferredInterests = Array.Empty<string>(),
                preferredTimeSlots = new[] { "lunch" },
                preferredPaymentTypes = new[] { "split" }
            }),
            DefaultGameSettingsJson = JsonContentHelper.Serialize(new
            {
                defaultMissions = new[] { "결제 담당", "후기 작성", "자리 예약", "쿠폰 담당", "음료 담당", "사진 담당" },
                defaultCosts = new[] { "0", "5,000", "10,000", "15,000", "20,000" }
            })
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            // Send welcome SMS
            await _notifier.NotifyRegistrationAsync(user.Phone, user.Name);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SMS SEND ERROR] {ex.Message}");
        }

        return Ok(ToUserProfileDto(user));
    }

    private static FriendDto ToFriendDto(Friend entity)
    {
        var tags = JsonContentHelper.DeserializeList<string>(entity.TagsJson);
        return new FriendDto(
            entity.Id,
            entity.OwnerId,
            entity.Name,
            entity.Avatar,
            entity.TrustLevel,
            entity.LastMeal,
            tags,
            entity.Memo,
            entity.Phone,
            entity.Identifier);
    }

    [HttpGet("{id}/friends")]
    public async Task<ActionResult<IEnumerable<FriendDto>>> GetUserFriends(string id, CancellationToken cancellationToken)
    {
        var friends = await _context.Friends
            .AsNoTracking()
            .Where(f => f.OwnerId == id)
            .ToListAsync(cancellationToken);

        return Ok(friends.Select(ToFriendDto));
    }

    [HttpPost("{id}/friends")]
    public async Task<ActionResult<FriendDto>> PostFriend(
        string id,
        [FromBody] Friend newFriend,
        CancellationToken cancellationToken)
    {
        if (newFriend is null)
            return BadRequest("Payload required.");
        if (string.IsNullOrWhiteSpace(newFriend.Id))
            return BadRequest("Friend Id required.");
        if (id == newFriend.Id)
            return BadRequest("Cannot add yourself.");

        var ownerExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == id, cancellationToken);

        if (!ownerExists)
            return NotFound($"User {id} not found.");

        var duplicate = await _context.Friends
            .AsNoTracking()
            .AnyAsync(f => f.OwnerId == id && f.Id == newFriend.Id, cancellationToken);

        if (duplicate)
            return Conflict("Already linked.");

        var friend = new Friend
        {
            OwnerId = id,
            Id = newFriend.Id,
            Name = string.IsNullOrWhiteSpace(newFriend.Name) ? "비회원 친구" : newFriend.Name,
            Avatar = newFriend.Avatar ?? string.Empty,
            TrustLevel = newFriend.TrustLevel,
            LastMeal = newFriend.LastMeal ?? string.Empty,
            TagsJson = string.IsNullOrWhiteSpace(newFriend.TagsJson) ? "[]" : newFriend.TagsJson,
            Memo = newFriend.Memo ?? string.Empty,
            Phone = newFriend.Phone ?? string.Empty,
            Identifier = newFriend.Identifier ?? string.Empty
        };

        _context.Friends.Add(friend);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetUserFriends), new { id }, ToFriendDto(friend));
    }

    [HttpDelete("{id}/friends/{friendId}")]
    public async Task<IActionResult> DeleteFriend(string id, string friendId, CancellationToken cancellationToken)
    {
        var friend = await _context.Friends
            .SingleOrDefaultAsync(f => f.OwnerId == id && f.Id == friendId, cancellationToken);

        if (friend is null)
            return NotFound();

        _context.Friends.Remove(friend);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
// 클래스 닫기
}
// 네임스페이스 닫기
