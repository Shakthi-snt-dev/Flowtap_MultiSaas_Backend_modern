using Flowtap_Application.Common.DTOs;
using Flowtap_Repair.DbContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Repair.Application.Features.Knowledge;

// ─── DTOs ─────────────────────────────────────────────────────────────────────

public record TechnicalFaultDto(
    Guid    Id,
    string  Symptom,
    string? PossibleCause,
    string? StandardSolution,
    bool    IsActive);

public record ChecklistTemplateDto(
    Guid   Id,
    string Name,
    string JsonItems,   // JSON array of checklist step strings
    bool   IsActive);

// ─── Queries ──────────────────────────────────────────────────────────────────

public record GetTechnicalFaultsQuery(
    Guid    CompanyId,
    string? Search = null)
    : IRequest<Result<List<TechnicalFaultDto>>>;

public record GetChecklistTemplatesQuery(Guid CompanyId)
    : IRequest<Result<List<ChecklistTemplateDto>>>;

// ─── Handlers ─────────────────────────────────────────────────────────────────

public class GetTechnicalFaultsQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetTechnicalFaultsQuery, Result<List<TechnicalFaultDto>>>
{
    public async Task<Result<List<TechnicalFaultDto>>> Handle(
        GetTechnicalFaultsQuery request, CancellationToken ct)
    {
        var query = db.TechnicalFaults
            .Where(f => f.CompanyId == request.CompanyId && f.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(f =>
                f.Symptom.Contains(request.Search) ||
                (f.PossibleCause  != null && f.PossibleCause.Contains(request.Search)) ||
                (f.StandardSolution != null && f.StandardSolution.Contains(request.Search)));

        var list = await query
            .OrderBy(f => f.Symptom)
            .Select(f => new TechnicalFaultDto(
                f.Id, f.Symptom, f.PossibleCause, f.StandardSolution, f.IsActive))
            .ToListAsync(ct);

        return Result<List<TechnicalFaultDto>>.Success(list);
    }
}

public class GetChecklistTemplatesQueryHandler(IRepairDbContext db)
    : IRequestHandler<GetChecklistTemplatesQuery, Result<List<ChecklistTemplateDto>>>
{
    public async Task<Result<List<ChecklistTemplateDto>>> Handle(
        GetChecklistTemplatesQuery request, CancellationToken ct)
    {
        var list = await db.RepairChecklistTemplates
            .Where(t => t.CompanyId == request.CompanyId && t.IsActive)
            .OrderBy(t => t.Name)
            .Select(t => new ChecklistTemplateDto(t.Id, t.Name, t.JsonItems, t.IsActive))
            .ToListAsync(ct);

        return Result<List<ChecklistTemplateDto>>.Success(list);
    }
}
