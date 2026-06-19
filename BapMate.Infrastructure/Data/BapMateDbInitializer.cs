using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BapMate.Infrastructure.Data;

public static class BapMateDbInitializer
{
    public static async Task InitializeAsync(BapMateDbContext context, CancellationToken cancellationToken = default)
    {
        // Apply pending migrations to ensure tables are created correctly
        await context.Database.MigrateAsync(cancellationToken);
    }
}
