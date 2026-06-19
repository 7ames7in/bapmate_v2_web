using BapMate.Infrastructure.Data;
using BapMate.WebApi.Extensions;
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
    // 회원가입 예시 엔드포인트
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User newUser, CancellationToken cancellationToken)
    {
        if (newUser is null || string.IsNullOrWhiteSpace(newUser.Id) || string.IsNullOrWhiteSpace(newUser.Name) || string.IsNullOrWhiteSpace(newUser.Phone))
            return BadRequest("필수 정보 누락");

        var exists = await _context.Users.AnyAsync(u => u.Id == newUser.Id, cancellationToken);
        if (exists)
            return Conflict("이미 가입된 사용자입니다.");

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync(cancellationToken);

        // 문자 발송
        await _notifier.NotifyRegistrationAsync(newUser.Phone, newUser.Name);

        return Ok(newUser.ToUserProfileDto());
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
            .SingleOrDefaultAsync(u => u.Email == normalizedIdentifier || u.Id == normalizedIdentifier || u.Phone == normalizedIdentifier, cancellationToken);

        if (user is null)
        {
            return NotFound(new { success = false, error = "존재하지 않는 사용자입니다." });
        }

        // Bypass password check for user 7ames7in@gmail.com
        if (string.Equals(user.Email, "7ames7in@gmail.com", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(user.Id, "kakao-4513767160", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(user.Phone, "01075011346", StringComparison.OrdinalIgnoreCase))
        {
            return Ok(user.ToUserProfileDto());
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { success = false, error = "비밀번호를 입력해주세요." });
        }

        // Auto-set: if user's password in DB is empty, set it on first login!
        if (string.IsNullOrEmpty(user.Password))
        {
            user.Password = request.Password;
            await _context.SaveChangesAsync(cancellationToken);
        }
        else if (user.Password != request.Password)
        {
            return BadRequest(new { success = false, error = "비밀번호가 일치하지 않습니다." });
        }

        return Ok(user.ToUserProfileDto());
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
        var user = new User
        {
            Id = id,
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            Password = request.Password,
            Gender = string.IsNullOrWhiteSpace(request.Gender) ? "male" : request.Gender.Trim(),
            BirthYear = request.BirthYear == 0 ? 1995 : request.BirthYear,
            Avatar = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(request.Name.Trim())}",
            Bio = "새로 가입한 밥메이트입니다!",
            Phone = normalizedPhone,
            Carrier = "이메일",
            ReliabilityScore = 70,
            WalletBalance = 50000m,
            EscrowBalance = 0m,
            BadgesJson = JsonContentHelper.Serialize(new[] { "이메일 인증" }),
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

        return Ok(user.ToUserProfileDto());
    }

    [HttpPost("kakao-register")]
    public async Task<IActionResult> RegisterFromKakao([FromBody] SocialRegisterRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest("필수 정보 누락");
        }

        var normalizedId = request.Id.Trim();
        var exists = await _context.Users.AnyAsync(u => u.Id == normalizedId, cancellationToken);
        if (exists)
        {
            return Conflict("이미 가입된 사용자입니다.");
        }

        var avatar = string.IsNullOrWhiteSpace(request.Avatar)
            ? $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(request.Name)}"
            : request.Avatar.Trim();

        var bio = string.IsNullOrWhiteSpace(request.Bio)
            ? $"카카오 간편 가입 회원 ({request.Gender ?? "성별 미기입"})"
            : request.Bio.Trim();

        var user = new User
        {
            Id = normalizedId,
            Name = request.Name.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email)
                ? $"{normalizedId}@kakao-user.bapmate"
                : request.Email.Trim(),
            Avatar = avatar,
            Bio = bio,
            Phone = request.Phone.Trim(),
            Carrier = string.IsNullOrWhiteSpace(request.Carrier) ? "카카오" : request.Carrier.Trim(),
            ReliabilityScore = 88,
            WalletBalance = 50000m,
            EscrowBalance = 0m,
            BadgesJson = JsonContentHelper.Serialize(new[] { "카카오 인증" }),
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

        return Ok(user.ToUserProfileDto());
    }

    [HttpPost("naver-register")]
    public async Task<IActionResult> RegisterFromNaver([FromBody] SocialRegisterRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest("필수 정보 누락");
        }

        var normalizedId = request.Id.Trim();
        var exists = await _context.Users.AnyAsync(u => u.Id == normalizedId, cancellationToken);
        if (exists)
        {
            return Conflict("이미 가입된 사용자입니다.");
        }

        var avatar = string.IsNullOrWhiteSpace(request.Avatar)
            ? $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(request.Name)}"
            : request.Avatar.Trim();

        var bio = string.IsNullOrWhiteSpace(request.Bio)
            ? $"네이버 간편 가입 회원 ({request.Gender ?? "성별 미기입"})"
            : request.Bio.Trim();

        var user = new User
        {
            Id = normalizedId,
            Name = request.Name.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email)
                ? $"{normalizedId}@naver-user.bapmate"
                : request.Email.Trim(),
            Avatar = avatar,
            Bio = bio,
            Phone = request.Phone.Trim(),
            Carrier = string.IsNullOrWhiteSpace(request.Carrier) ? "네이버" : request.Carrier.Trim(),
            ReliabilityScore = 88,
            WalletBalance = 50000m,
            EscrowBalance = 0m,
            BadgesJson = JsonContentHelper.Serialize(new[] { "네이버 인증" }),
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

        return Ok(user.ToUserProfileDto());
    }

    [HttpPost("google-register")]
    public async Task<IActionResult> RegisterFromGoogle([FromBody] SocialRegisterRequest request, CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Phone))
        {
            return BadRequest("필수 정보 누락");
        }

        var normalizedId = request.Id.Trim();
        var exists = await _context.Users.AnyAsync(u => u.Id == normalizedId, cancellationToken);
        if (exists)
        {
            return Conflict("이미 가입된 사용자입니다.");
        }

        var avatar = string.IsNullOrWhiteSpace(request.Avatar)
            ? $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(request.Name)}"
            : request.Avatar.Trim();

        var bio = string.IsNullOrWhiteSpace(request.Bio)
            ? $"구글 간편 가입 회원 ({request.Gender ?? "성별 미기입"})"
            : request.Bio.Trim();

        var user = new User
        {
            Id = normalizedId,
            Name = request.Name.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email)
                ? $"{normalizedId}@gmail.com"
                : request.Email.Trim(),
            Avatar = avatar,
            Bio = bio,
            Phone = request.Phone.Trim(),
            Carrier = string.IsNullOrWhiteSpace(request.Carrier) ? "구글" : request.Carrier.Trim(),
            ReliabilityScore = 88,
            WalletBalance = 50000m,
            EscrowBalance = 0m,
            BadgesJson = JsonContentHelper.Serialize(new[] { "구글 인증" }),
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

        return Ok(user.ToUserProfileDto());
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Ok(users.Select(u => u.ToUserProfileDto()));
    }

    [HttpGet("current")]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUser([FromQuery] string id = "", CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user.ToUserProfileDto());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfileDto>> GetUserById(string id, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user.ToUserProfileDto());
    }

    [HttpGet("{id}/friends")]
    public async Task<ActionResult<IEnumerable<FriendDto>>> GetUserFriends(string id, CancellationToken cancellationToken)
    {
        var friends = await _context.Friends
            .AsNoTracking()
            .Where(f => f.OwnerId == id)
            .ToListAsync(cancellationToken);

        return Ok(friends.Select(f => f.ToFriendDto()));
    }

    public sealed class AddFriendRequest
    {
        public string FriendId { get; init; } = string.Empty;
    }

    public sealed record UpdateDefaultGameSettingsRequest(
        IReadOnlyCollection<string>? DefaultMissions,
        IReadOnlyCollection<string>? DefaultCosts);

    private sealed record DefaultGameSettingsPayload(
        IReadOnlyCollection<string> DefaultMissions,
        IReadOnlyCollection<string> DefaultCosts);

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

        var friendUser = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == newFriend.Id, cancellationToken);

        var duplicate = await _context.Friends
            .AsNoTracking()
            .AnyAsync(f => f.OwnerId == id && f.Id == newFriend.Id, cancellationToken);

        if (duplicate)
            return Conflict("Already linked.");

        var friend = new Friend
        {
            OwnerId = id,
            Id = newFriend.Id,
            Name = string.IsNullOrWhiteSpace(newFriend.Name) ? (friendUser?.Name ?? "비회원 친구") : newFriend.Name,
            Avatar = string.IsNullOrWhiteSpace(newFriend.Avatar) ? (friendUser?.Avatar ?? string.Empty) : newFriend.Avatar,
            TrustLevel = newFriend.TrustLevel,
            LastMeal = newFriend.LastMeal ?? string.Empty,
            TagsJson = string.IsNullOrWhiteSpace(newFriend.TagsJson) ? "[]" : newFriend.TagsJson,
            Memo = newFriend.Memo ?? string.Empty,
            Phone = string.IsNullOrWhiteSpace(newFriend.Phone) ? (friendUser?.Phone ?? string.Empty) : newFriend.Phone,
            Identifier = string.IsNullOrWhiteSpace(newFriend.Identifier) ? (friendUser?.Email ?? string.Empty) : newFriend.Identifier
        };

        _context.Friends.Add(friend);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetUserFriends), new { id }, friend.ToFriendDto());
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

    [HttpPut("{id}/default-game-settings")]
    public async Task<IActionResult> UpdateDefaultGameSettings(
        string id,
        [FromBody] UpdateDefaultGameSettingsRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest("Payload required.");
        }

        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        static string[] Normalize(IEnumerable<string> source) =>
            source
                .Select(value => value?.Trim())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

        var payload = new DefaultGameSettingsPayload(
            request.DefaultMissions is null ? Array.Empty<string>() : Normalize(request.DefaultMissions),
            request.DefaultCosts is null ? Array.Empty<string>() : Normalize(request.DefaultCosts));

        user.DefaultGameSettingsJson = JsonContentHelper.Serialize(payload);
        await _context.SaveChangesAsync(cancellationToken);


        return NoContent();
    }
}
// 클래스 닫기
}
// 네임스페이스 닫기
