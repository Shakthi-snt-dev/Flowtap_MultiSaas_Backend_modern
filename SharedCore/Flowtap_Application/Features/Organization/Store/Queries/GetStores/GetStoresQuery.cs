using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Organization.Store.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Store.Queries.GetStores;

public record GetStoresQuery(Guid CompanyId) : IRequest<Result<List<StoreListItemDto>>>;
