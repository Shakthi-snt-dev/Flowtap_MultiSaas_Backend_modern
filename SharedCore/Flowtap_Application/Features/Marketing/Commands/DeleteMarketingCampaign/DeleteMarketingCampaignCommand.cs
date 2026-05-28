using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Marketing.Commands.DeleteMarketingCampaign;

public record DeleteMarketingCampaignCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
