using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Subscription.Commands.StartTrial;

public record StartTrialCommand(Guid CompanyId) : IRequest<Result<Guid>>;
