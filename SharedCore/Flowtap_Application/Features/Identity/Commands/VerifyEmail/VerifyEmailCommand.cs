using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Identity.Commands.VerifyEmail;

public record VerifyEmailCommand(string Token) : IRequest<Result<bool>>;
