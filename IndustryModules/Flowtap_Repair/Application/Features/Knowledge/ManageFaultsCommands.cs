using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Exceptions;
using Flowtap_Repair.DbContext;
using Flowtap_Repair.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Knowledge;

// ─── Commands ─────────────────────────────────────────────────────────────────

public record CreateFaultCommand(
    Guid    CompanyId,
    string  Symptom,
    string? PossibleCause,
    string? StandardSolution) : IRequest<Result<Guid>>;

public record UpdateFaultCommand(
    Guid    Id, Guid CompanyId,
    string  Symptom,
    string? PossibleCause,
    string? StandardSolution) : IRequest<Result<bool>>;

public record DeleteFaultCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;

public record CreateChecklistCommand(
    Guid   CompanyId,
    string Name,
    string JsonItems) : IRequest<Result<Guid>>;

public record UpdateChecklistCommand(
    Guid   Id, Guid CompanyId,
    string Name,
    string JsonItems) : IRequest<Result<bool>>;

public record DeleteChecklistCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;

// ─── Handlers ─────────────────────────────────────────────────────────────────

public class CreateFaultCommandHandler(IRepairDbContext db)
    : IRequestHandler<CreateFaultCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateFaultCommand r, CancellationToken ct)
    {
        var fault = new TechnicalFault
        {
            CompanyId = r.CompanyId,
            Symptom = r.Symptom,
            PossibleCause = r.PossibleCause,
            StandardSolution = r.StandardSolution,
            IsActive = true
        };
        db.TechnicalFaults.Add(fault);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(fault.Id);
    }
}

public class UpdateFaultCommandHandler(IRepairDbContext db)
    : IRequestHandler<UpdateFaultCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateFaultCommand r, CancellationToken ct)
    {
        var fault = await db.TechnicalFaults
            .FirstOrDefaultAsync(f => f.Id == r.Id && f.CompanyId == r.CompanyId, ct)
            ?? throw new NotFoundException(nameof(TechnicalFault), r.Id);
        fault.Symptom = r.Symptom;
        fault.PossibleCause = r.PossibleCause;
        fault.StandardSolution = r.StandardSolution;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class DeleteFaultCommandHandler(IRepairDbContext db)
    : IRequestHandler<DeleteFaultCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteFaultCommand r, CancellationToken ct)
    {
        var fault = await db.TechnicalFaults
            .FirstOrDefaultAsync(f => f.Id == r.Id && f.CompanyId == r.CompanyId, ct)
            ?? throw new NotFoundException(nameof(TechnicalFault), r.Id);
        fault.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class CreateChecklistCommandHandler(IRepairDbContext db)
    : IRequestHandler<CreateChecklistCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateChecklistCommand r, CancellationToken ct)
    {
        var tpl = new RepairChecklistTemplate
        {
            CompanyId = r.CompanyId,
            Name = r.Name,
            JsonItems = r.JsonItems,
            IsActive = true
        };
        db.RepairChecklistTemplates.Add(tpl);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(tpl.Id);
    }
}

public class UpdateChecklistCommandHandler(IRepairDbContext db)
    : IRequestHandler<UpdateChecklistCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateChecklistCommand r, CancellationToken ct)
    {
        var tpl = await db.RepairChecklistTemplates
            .FirstOrDefaultAsync(t => t.Id == r.Id && t.CompanyId == r.CompanyId, ct)
            ?? throw new NotFoundException(nameof(RepairChecklistTemplate), r.Id);
        tpl.Name = r.Name;
        tpl.JsonItems = r.JsonItems;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}

public class DeleteChecklistCommandHandler(IRepairDbContext db)
    : IRequestHandler<DeleteChecklistCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteChecklistCommand r, CancellationToken ct)
    {
        var tpl = await db.RepairChecklistTemplates
            .FirstOrDefaultAsync(t => t.Id == r.Id && t.CompanyId == r.CompanyId, ct)
            ?? throw new NotFoundException(nameof(RepairChecklistTemplate), r.Id);
        tpl.IsActive = false;
        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
