using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Inventory.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flowtap_Infrastructure.BackgroundJobs;

public class ReorderCheckService(
    IServiceScopeFactory scopeFactory,
    ILogger<ReorderCheckService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckReorderLevelsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during reorder level check");
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task CheckReorderLevelsAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var rules = await db.ReorderRules
            .Where(r => r.IsActive)
            .ToListAsync(ct);

        foreach (var rule in rules)
        {
            var stock = await db.WarehouseStocks
                .FirstOrDefaultAsync(s =>
                    s.ProductId == rule.ProductId &&
                    s.WarehouseId == rule.WarehouseId, ct);

            var currentQty = stock?.Quantity ?? 0m;

            if (currentQty <= rule.MinimumQuantity)
            {
                var alertExists = await db.ReorderAlerts
                    .AnyAsync(a =>
                        a.ProductId == rule.ProductId &&
                        a.WarehouseId == rule.WarehouseId &&
                        !a.IsHandled, ct);

                if (!alertExists)
                {
                    var severity = currentQty == 0
                        ? ReorderAlertSeverity.Critical
                        : currentQty <= rule.MinimumQuantity / 2
                            ? ReorderAlertSeverity.Warning
                            : ReorderAlertSeverity.Low;

                    db.ReorderAlerts.Add(new ReorderAlert
                    {
                        CompanyId = rule.CompanyId,
                        ProductId = rule.ProductId,
                        WarehouseId = rule.WarehouseId,
                        ReorderRuleId = rule.Id,
                        CurrentQuantity = (int)currentQty,
                        ReorderLevel = (int)rule.MinimumQuantity,
                        Severity = severity,
                        IsHandled = false
                    });

                    logger.LogInformation(
                        "Reorder alert created for Product {ProductId} in Warehouse {WarehouseId} (Severity: {Severity})",
                        rule.ProductId, rule.WarehouseId, severity);
                }
            }
        }

        await db.SaveChangesAsync(ct);
    }
}
