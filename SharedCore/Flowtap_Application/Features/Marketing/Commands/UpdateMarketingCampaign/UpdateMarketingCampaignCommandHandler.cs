using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Flowtap_Application.Features.Marketing.Commands.UpdateMarketingCampaign;

public class UpdateMarketingCampaignCommandHandler(IApplicationDbContext db)
    : IRequestHandler<UpdateMarketingCampaignCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateMarketingCampaignCommand request, CancellationToken ct)
    {
        var campaign = await db.MarketingCampaigns
            .Include(c => c.TargetLocations)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == request.CompanyId, ct);

        if (campaign is null)
            return Result<bool>.Failure("Marketing campaign not found.");

        if (!string.IsNullOrWhiteSpace(request.Title))
            campaign.Title = request.Title;

        if (!string.IsNullOrWhiteSpace(request.Message))
            campaign.Message = request.Message;

        if (request.DiscountPercentage.HasValue)
            campaign.DiscountPercentage = request.DiscountPercentage.Value;

        if (request.IsActive.HasValue)
            campaign.IsActive = request.IsActive.Value;

        // Replace target locations if provided
        if (request.TargetLocationIds is not null)
        {
            db.CampaignTargetLocations.RemoveRange(campaign.TargetLocations);
            foreach (var locationId in request.TargetLocationIds)
            {
                campaign.TargetLocations.Add(new CampaignTargetLocation
                {
                    CampaignId = campaign.Id,
                    LocationId = locationId
                });
            }
        }

        await db.SaveChangesAsync(ct);
        return Result<bool>.Success(true);
    }
}
