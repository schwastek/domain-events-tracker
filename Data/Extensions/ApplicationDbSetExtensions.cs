using Domain.Applications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Extensions;

public static class ApplicationDbSetExtensions
{
    public static async Task<Application?> GetRandomAsync(this DbSet<Application> applications, CancellationToken ct)
    {
        var count = await applications.CountAsync(ct);
        if (count == 0) return null;

        var skip = Random.Shared.Next(count);

        return await applications
            .Skip(skip)
            .FirstOrDefaultAsync(ct);
    }
}
