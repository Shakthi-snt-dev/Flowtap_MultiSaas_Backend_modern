using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Sales.Commands.DeleteOffer;

public record DeleteOfferCommand(Guid Id, Guid CompanyId) : IRequest<Result<bool>>;
