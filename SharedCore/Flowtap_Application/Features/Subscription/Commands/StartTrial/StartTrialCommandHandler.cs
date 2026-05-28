using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Subscription.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Subscription.Commands.StartTrial;

public class StartTrialCommandHandler(IApplicationDbContext db, IDateTimeService dateTime)
    : IRequestHandler<StartTrialCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(StartTrialCommand request, CancellationToken ct)
    {
        var existing = await db.TrialPlans.FirstOrDefaultAsync(t => t.CompanyId == request.CompanyId, ct);
        if (existing is not null) return Result<Guid>.Failure("Trial already started for this company.");

        var trial = new TrialPlan
        {
            CompanyId = request.CompanyId,
            TrialDays = 30,
            TrialStartDate = dateTime.UtcNow,
            TrialEndDate = dateTime.UtcNow.AddDays(30),
            LocationCount = 1
        };
        db.TrialPlans.Add(trial);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(trial.Id);
    }
}
