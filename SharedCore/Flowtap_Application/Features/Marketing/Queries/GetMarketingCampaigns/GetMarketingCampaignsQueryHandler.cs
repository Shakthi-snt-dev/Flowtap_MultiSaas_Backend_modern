using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Marketing.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Marketing.Queries.GetMarketingCampaigns;

public class GetMarketingCampaignsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetMarketingCampaignsQuery, Result<List<MarketingCampaignDto>>>
{
    public async Task<Result<List<MarketingCampaignDto>>> Handle(
        GetMarketingCampaignsQuery request, CancellationToken ct)
    {
        var query = db.MarketingCampaigns
            .Include(c => c.TargetLocations)
            .Where(c => c.CompanyId == request.CompanyId);

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

        var dtos = items.Select(c => new MarketingCampaignDto(
            c.Id,
            c.Title,
            c.Message,
            c.DiscountPercentage,
            c.IsActive,
            c.TargetLocations.Select(t => t.LocationId).ToList(),
            c.CreatedAt)).ToList();

        return Result<List<MarketingCampaignDto>>.Success(dtos);
    }
}
