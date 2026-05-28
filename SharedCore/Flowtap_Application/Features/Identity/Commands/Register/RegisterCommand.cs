using Flowtap_Application.Common.DTOs;
using Flowtap_Application.Common.Interfaces;
using MediatR;

namespace Flowtap_Application.Features.Identity.Commands.Register;

public record RegisterCommand(string Name, string Email, string Phone, string Password) : IRequest<Result<Guid>>;
