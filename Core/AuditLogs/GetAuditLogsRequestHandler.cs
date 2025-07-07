using Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.AuditLogs;

public sealed class GetAuditLogsRequest : IRequest<GetAuditLogsResponse> { }

public sealed class GetAuditLogsResponse
{
    public required IReadOnlyList<string> Logs { get; set; } = [];
}

public class GetAuditLogsRequestHandler : IRequestHandler<GetAuditLogsRequest, GetAuditLogsResponse>
{
    private readonly ApplicationDbContext _dbContext;

    public GetAuditLogsRequestHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetAuditLogsResponse> Handle(GetAuditLogsRequest request, CancellationToken ct)
    {
        var logs = await _dbContext.AuditLogs.AsNoTracking().Select(l => l.Log).ToListAsync(ct);
        var response = new GetAuditLogsResponse { Logs = logs };

        return response;
    }
}
