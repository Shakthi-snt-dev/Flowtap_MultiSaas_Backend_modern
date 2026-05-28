using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetCampaigns;

public record GetCampaignsQuery(Guid CompanyId, bool ActiveOnly = true)
    : IRequest<Result<List<CampaignDto>>>;
