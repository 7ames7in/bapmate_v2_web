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
    
}
// 클래스 닫기
}
// 네임스페이스 닫기
