using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Entities;
using Flowtap_Domain.BoundedContexts.Modules.Sales.Enums;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.CreateCampaign;

public class CreateCampaignCommandHandler(IApplicationDbContext db)
    : IRequestHandler<CreateCampaignCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCampaignCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<CampaignType>(request.Type, true, out var campaignType))
            return Result<Guid>.Failure($"Invalid campaign type: {request.Type}");

        var campaign = new Campaign
        {
            CompanyId = request.CompanyId,
            Name = request.Name,
            Type = campaignType,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            BudgetAmount = request.BudgetAmount ?? 0,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = CampaignStatus.Scheduled
        };

        if (request.ProductIds is { Count: > 0 })
        {
            foreach (var productId in request.ProductIds)
            {
                campaign.Rules.Add(new CampaignProductRule
                {
                    CampaignId = campaign.Id,
                    ProductId = productId,
                    RuleType = "Product"
                });
            }
        }

        db.Campaigns.Add(campaign);
        await db.SaveChangesAsync(ct);
        return Result<Guid>.Success(campaign.Id);
    }
}
