using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Features.Subscription.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Subscription.Queries.GetSubscription;

public record GetSubscriptionQuery(Guid CompanyId) : IRequest<Result<SubscriptionDto?>>;
