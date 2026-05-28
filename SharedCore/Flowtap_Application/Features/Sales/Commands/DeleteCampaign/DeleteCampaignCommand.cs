using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.DeleteCampaign;

public record DeleteCampaignCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
