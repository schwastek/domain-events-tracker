using Data;
using Domain.Events.Core;
using Infrastructure.NotificationPublisherService;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Filters;

// IFilterFactory supports constructor DI in filters.
// See: https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-8.0#dependency-injection
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class UseTransactionAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<TransactionFilter>();
    }
}

public class TransactionFilter : IAsyncActionFilter
{
    private readonly ILogger<TransactionFilter> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly INotificationPublisher _notificationPublisher;

    public TransactionFilter(ILogger<TransactionFilter> logger, ApplicationDbContext dbContext, INotificationPublisher notificationPublisher)
    {
        _logger = logger;
        _dbContext = dbContext;
        _notificationPublisher = notificationPublisher;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var ct = context.HttpContext.RequestAborted;

        // If there's already a transaction, don’t start a new one
        if (_dbContext.Database.CurrentTransaction != null)
        {
            await next();
            return;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

        ActionExecutedContext? executedContext = null;

        try
        {
            executedContext = await next();

            if (executedContext.Exception == null || executedContext.ExceptionHandled)
            {
                // Save all pending changes before committing.
                await _dbContext.SaveChangesAsync(ct);
                await DispatchDomainEvents(ct);
                await transaction.CommitAsync(ct);
                _logger.LogDebug("Transaction committed.");
            }
            else
            {
                await transaction.RollbackAsync(ct);
                _logger.LogWarning(executedContext.Exception, "Transaction rolled back due to exception.");
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Transaction rolled back due to exception.");
            throw;
        }
    }

    private async Task DispatchDomainEvents(CancellationToken ct)
    {
        var domainEntities = _dbContext.ChangeTracker
            .Entries<IHaveDomainEvents>();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.CollectDomainEvents())
            .ToList();

        domainEntities
            .ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _notificationPublisher.Publish(domainEvent, ct);
        }
    }
}
