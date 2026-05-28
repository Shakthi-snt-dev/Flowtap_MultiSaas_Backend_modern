using AutoMapper;
using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Subscription.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Subscription.Queries.GetSubscription;

public class GetSubscriptionQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetSubscriptionQuery, Result<SubscriptionDto?>>
{
    public async Task<Result<SubscriptionDto?>> Handle(GetSubscriptionQuery request, CancellationToken ct)
    {
        var sub = await db.CompanySubscriptions
            .Include(s => s.SubscriptionPlan)
            .Where(s => s.CompanyId == request.CompanyId && s.IsActive)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (sub is null) return Result<SubscriptionDto?>.Success(null);

        return Result<SubscriptionDto?>.Success(new SubscriptionDto(
            sub.Id, sub.CompanyId, sub.SubscriptionPlanId, sub.SubscriptionPlan.Name,
            sub.StartDate, sub.EndDate, sub.NextBillingDate,
            sub.Status.ToString(), sub.IsActive, sub.TotalLocations, sub.TotalAmount));
    }
}
