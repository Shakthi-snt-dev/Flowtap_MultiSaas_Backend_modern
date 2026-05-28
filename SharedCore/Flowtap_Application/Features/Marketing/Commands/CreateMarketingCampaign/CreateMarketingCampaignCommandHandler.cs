using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using MediatR;

namespace Flowtap_Application.Features.Marketing.Commands.CreateMarketingCampaign;

public class CreateMarketingCampaignCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateMarketingCampaignCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateMarketingCampaignCommand request, CancellationToken ct)
    {
        var campaign = new MarketingCampaign
        {
            CompanyId = request.CompanyId,
            Title = request.Title,
            Message = request.Message,
            DiscountPercentage = request.DiscountPercentage ?? 0,
            IsActive = true
        };

        if (request.TargetLocationIds is { Count: > 0 })
        {
            foreach (var locationId in request.TargetLocationIds)
            {
                campaign.TargetLocations.Add(new CampaignTargetLocation
                {
                    CampaignId = campaign.Id,
                    LocationId = locationId
                });
            }
        }

        db.MarketingCampaigns.Add(campaign);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(campaign.Id);
    }
}
