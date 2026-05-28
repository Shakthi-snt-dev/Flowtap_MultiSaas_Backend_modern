using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Subscription.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Subscription.Queries.GetTrialStatus;

public class GetTrialStatusQueryHandler(IApplicationDbContext db, IDateTimeService dateTime)
    : IRequestHandler<GetTrialStatusQuery, Result<TrialDto?>>
{
    public async Task<Result<TrialDto?>> Handle(GetTrialStatusQuery request, CancellationToken ct)
    {
        var trial = await db.TrialPlans.FirstOrDefaultAsync(t => t.CompanyId == request.CompanyId, ct);
        if (trial is null) return Result<TrialDto?>.Success(null);

        if (!trial.IsExpired && trial.TrialEndDate < dateTime.UtcNow)
        {
            trial.IsExpired = true;
            await db.SaveChangesAsync(ct);
        }

        return Result<TrialDto?>.Success(new TrialDto(
            trial.Id, trial.CompanyId, trial.TrialDays,
            trial.TrialStartDate, trial.TrialEndDate,
            trial.IsExpired, trial.LocationCount, trial.IsConverted));
    }
}
