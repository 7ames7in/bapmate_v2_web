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

// 데이터베이스 연결 설정 (Postgres 연결 정보가 입력되었으면 Postgres, 없으면 기본 SQLite 사용)
builder.Services.AddDbContext<BapMateDbContext>(opt =>
{
    var postgresConn = builder.Configuration.GetConnectionString("Postgres");
    if (!string.IsNullOrEmpty(postgresConn) && !postgresConn.Contains("YOUR_PASSWORD_HERE"))
    {
        opt.UseNpgsql(postgresConn);
    }
    else
    {
        opt.UseSqlite(builder.Configuration.GetConnectionString("Default"));
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));
});

var app = builder.Build();

// ---- 운영/개발 공통: 시작 시 마이그레이션 자동 적용 ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BapMateDbContext>();
    await BapMateDbInitializer.InitializeAsync(db);
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


// using BapMate.Infrastructure.Data;
// using Microsoft.EntityFrameworkCore;

// var builder = WebApplication.CreateBuilder(args);

// // 1) CORS 정책 등록: Vite 개발 서버를 허용
// const string VitePolicy = "ViteDev";
// builder.Services.AddCors(opt =>
// {
//     opt.AddPolicy(VitePolicy, policy =>
//     {
//         policy.WithOrigins("http://localhost:5173")   // Vite dev 서버
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//               // .AllowCredentials(); // 쿠키/인증 필요할 때만 사용
//     });
// });

// builder.Services.AddControllers();

// builder.Services.AddDbContext<BapMateDbContext>(opt =>
//     opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// // 2) CORS 미들웨어는 Authorization/MapControllers 전에
// app.UseCors(VitePolicy);

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<BapMateDbContext>();
//     db.Database.Migrate();
// }

// // (선택) 루트로 들어오면 Swagger로
// app.MapGet("/", () => Results.Redirect("/swagger"));

// app.UseAuthorization();
// app.MapControllers();
// app.Run();
