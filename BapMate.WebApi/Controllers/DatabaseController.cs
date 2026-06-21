using BapMate.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace BapMate.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly BapMateDbContext _context;

        public DatabaseController(BapMateDbContext context)
        {
            _context = context;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetDatabaseInfo(CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();
            var connectionString = connection.ConnectionString;

            string? configHost = null;
            int? configPort = null;
            string? configDatabase = null;
            string? configUsername = null;

            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);
                configHost = builder.Host;
                configPort = builder.Port;
                configDatabase = builder.Database;
                configUsername = builder.Username;
            }
            catch (Exception ex)
            {
                // Fallback if parsing connection string fails
                Console.WriteLine($"[DatabaseController] Connection string parsing failed: {ex.Message}");
            }

            bool isConnected = false;
            string? pgVersion = null;
            string? serverIp = null;
            string? serverPortVal = null;
            string? currentDatabase = null;
            string? errorMessage = null;

            try
            {
                var wasClosed = connection.State == ConnectionState.Closed;
                if (wasClosed)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT version();";
                        pgVersion = (string?)await command.ExecuteScalarAsync(cancellationToken);

                        command.CommandText = "SELECT current_database();";
                        currentDatabase = (string?)await command.ExecuteScalarAsync(cancellationToken);

                        command.CommandText = "SELECT inet_server_addr()::text;";
                        serverIp = (string?)await command.ExecuteScalarAsync(cancellationToken);

                        command.CommandText = "SELECT inet_server_port();";
                        var portObj = await command.ExecuteScalarAsync(cancellationToken);
                        serverPortVal = portObj?.ToString();
                    }
                    isConnected = true;
                }
                finally
                {
                    if (wasClosed)
                    {
                        await connection.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return Ok(new
            {
                status = isConnected ? "Connected" : "Disconnected",
                configured = new
                {
                    host = configHost,
                    port = configPort,
                    database = configDatabase,
                    username = configUsername
                },
                actual = isConnected ? new
                {
                    database = currentDatabase,
                    serverIp = serverIp,
                    serverPort = serverPortVal,
                    version = pgVersion
                } : null,
                error = errorMessage
            });
        }
    }
}
