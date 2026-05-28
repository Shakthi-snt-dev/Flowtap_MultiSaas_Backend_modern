using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Sales.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Queries.GetOffers;

public record GetOffersQuery(Guid CompanyId, bool ActiveOnly = true)
    : IRequest<Result<List<OfferDto>>>;
