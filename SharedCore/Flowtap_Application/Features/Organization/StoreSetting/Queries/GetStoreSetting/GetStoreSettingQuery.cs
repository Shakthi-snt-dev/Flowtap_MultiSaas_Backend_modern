using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Organization.StoreSetting.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.StoreSetting.Queries.GetStoreSetting;

public record GetStoreSettingQuery(Guid CompanyId, Guid LocationId)
    : IRequest<Result<StoreSettingDto>>;
