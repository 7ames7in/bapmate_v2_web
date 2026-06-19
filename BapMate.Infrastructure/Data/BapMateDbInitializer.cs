using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BapMate.Infrastructure.Data;

public static class BapMateDbInitializer
{
    public static async Task InitializeAsync(BapMateDbContext context, CancellationToken cancellationToken = default)
    {
        // Use EnsureCreatedAsync since migrations are removed and we want the DB schema to be created automatically on startup
        await context.Database.EnsureCreatedAsync(cancellationToken);
    }
}
