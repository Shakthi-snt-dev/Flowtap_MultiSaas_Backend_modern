using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.CreateCampaign;

public record CreateCampaignCommand(
    Guid CompanyId, string Name, string Type, string DiscountType,
    decimal DiscountValue, decimal? BudgetAmount,
    DateTime StartDate, DateTime EndDate,
    List<Guid>? ProductIds = null) : IRequest<Result<Guid>>;
