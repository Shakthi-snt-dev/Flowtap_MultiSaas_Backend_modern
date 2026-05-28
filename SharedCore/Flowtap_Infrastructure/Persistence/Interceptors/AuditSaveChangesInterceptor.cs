using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Core.Organization.Entities;
using Flowtap_Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace Flowtap_Infrastructure.Persistence.Interceptors;

public class AuditSaveChangesInterceptor(ICurrentUserService currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddAuditEntries(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        AddAuditEntries(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void AddAuditEntries(Microsoft.EntityFrameworkCore.DbContext? context)
    {
        if (context is null) return;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is TenantEntity &&
                        e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var tenantEntity = (TenantEntity)entry.Entity;
            var audit = new AuditLog
            {
                CompanyId = tenantEntity.CompanyId,
                EntityName = entry.Entity.GetType().Name,
                EntityId = tenantEntity.Id.ToString(),
                Action = entry.State.ToString(),
                UserId = currentUser.UserId,
                OldValues = entry.State == EntityState.Modified
                    ? JsonSerializer.Serialize(entry.OriginalValues.Properties
                        .ToDictionary(p => p.Name, p => entry.OriginalValues[p]?.ToString()))
                    : null,
                NewValues = entry.State != EntityState.Deleted
                    ? JsonSerializer.Serialize(entry.CurrentValues.Properties
                        .ToDictionary(p => p.Name, p => entry.CurrentValues[p]?.ToString()))
                    : null,
                IsActive = true
            };
            context.Set<AuditLog>().Add(audit);
        }
    }
}
