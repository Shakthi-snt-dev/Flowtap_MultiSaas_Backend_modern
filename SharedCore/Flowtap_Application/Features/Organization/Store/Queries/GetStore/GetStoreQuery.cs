using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Organization.Store.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Store.Queries.GetStore;

public record GetStoreQuery(Guid CompanyId, Guid StoreId) : IRequest<Result<StoreDto>>;
