using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Subscription.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Subscription.Queries.GetTrialStatus;

public record GetTrialStatusQuery(Guid CompanyId) : IRequest<Result<TrialDto?>>;
