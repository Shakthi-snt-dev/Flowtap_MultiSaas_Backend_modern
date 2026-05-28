using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Identity.Commands.ResendVerification;

public record ResendVerificationEmailCommand(string Email) : IRequest<Result<bool>>;
