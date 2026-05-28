using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Marketing.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Marketing.Queries.GetMarketingCampaigns;

public record GetMarketingCampaignsQuery(
    Guid CompanyId,
    bool? IsActive = null
) : IRequest<Result<List<MarketingCampaignDto>>>;
