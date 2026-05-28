using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Sales.Queries.GetCampaigns;

public class GetCampaignsQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetCampaignsQuery, Result<List<CampaignDto>>>
{
    public async Task<Result<List<CampaignDto>>> Handle(GetCampaignsQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var query = db.Campaigns.Where(c => c.CompanyId == request.CompanyId);

        if (request.ActiveOnly)
            query = query.Where(c => c.StartDate <= now && c.EndDate >= now);

        var items = await query.OrderByDescending(c => c.CreatedAt).ToListAsync(ct);

        var dtos = items.Select(c => new CampaignDto(
            c.Id, c.Name, c.Type.ToString(), c.DiscountValue,
            c.DiscountType.ToString(), c.StartDate, c.EndDate, c.Status.ToString())).ToList();

        return Result<List<CampaignDto>>.Success(dtos);
    }
}
