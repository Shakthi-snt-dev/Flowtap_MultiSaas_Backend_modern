using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Store.Commands.CreateStore;

public record CreateStoreCommand(
    Guid CompanyId, string Title, string Phone, string Address,
    string CountryCode, string CurrencyCode, string? TimeZoneId = null,
    string? LocationCode = null) : IRequest<Result<Guid>>;
