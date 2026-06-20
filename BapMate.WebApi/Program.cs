using BapMate.Infrastructure.Data;
using BapMate.WebApi.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS 정책
const string PolicyName = "BapMatePolicy";
builder.Services.AddCors(opt =>
{
    var configuredOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
    var allowedOrigins = new HashSet<string>(configuredOrigins.Select(o => o?.TrimEnd('/')).Where(o => !string.IsNullOrWhiteSpace(o))!, StringComparer.OrdinalIgnoreCase);

    Console.WriteLine($"CORS AllowedOrigins: {string.Join(", ", allowedOrigins)}");

    opt.AddPolicy(PolicyName, policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();

        if (allowedOrigins.Count > 0)
        {
            policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrWhiteSpace(origin))
                {
                    return false;
                }

                var normalized = origin.TrimEnd('/');
                return allowedOrigins.Contains(normalized);
            });
        }
        else
        {
            policy.SetIsOriginAllowed(_ => true);
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();

// 데이터베이스 연결 설정 (Postgres)
builder.Services.AddDbContext<BapMateDbContext>(opt =>
{
    var postgresConn = builder.Configuration.GetConnectionString("Postgres");
    opt.UseNpgsql(postgresConn);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
});

var app = builder.Build();

// ---- 운영/개발 공통: 시작 시 마이그레이션 자동 적용 ----
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<BapMateDbContext>();
        await BapMateDbInitializer.InitializeAsync(db);
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("==============================================================");
    Console.WriteLine("❌ [DATABASE CONNECTION ERROR]");
    Console.WriteLine("시작 시 PostgreSQL 데이터베이스 연결에 실패했습니다.");
    Console.WriteLine($"에러 상세 정보:\n{ex}");
    
    var connString = builder.Configuration.GetConnectionString("Postgres");
    if (!string.IsNullOrEmpty(connString))
    {
        var maskedConnString = System.Text.RegularExpressions.Regex.Replace(
            connString, 
            "Password=[^;]+", 
            "Password=******", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        Console.WriteLine($"설정된 Connection String: {maskedConnString}");
    }
    
    Console.WriteLine("\n[확인 필요 사항]");
    Console.WriteLine("1. PostgreSQL 도커 컨테이너 또는 DB 서버가 정상 실행 중인지 확인하세요.");
    Console.WriteLine("2. appsettings.json 또는 환경 변수의 Postgres 연결 정보가 맞는지 확인하세요.");
    Console.WriteLine("3. 호스트명(Host), 포트(Port), 사용자명(Username), 비밀번호(Password)가 올바른지 확인하세요.");
    Console.WriteLine("==============================================================");
    Console.ResetColor();
}

// CORS (Auth/Controllers 앞)
app.UseCors(PolicyName);

// Swagger (운영에서도 옵션으로 켜기)
var enableSwagger = builder.Configuration.GetValue("EnableSwagger", false);
if (enableSwagger || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 헬스/루트 라우트
app.MapGet("/", () => Results.Redirect("/swagger")); // 운영 헬스체크/편의
app.MapGet("/healthz", () => Results.Ok("ok"));

app.UseAuthorization();
app.MapControllers();
app.MapHub<BombGameHub>("/hubs/bombgame");

app.Run();
