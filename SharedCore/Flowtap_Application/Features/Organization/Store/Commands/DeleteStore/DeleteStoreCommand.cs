using System;
using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Organization.Store.Commands.DeleteStore;

public record DeleteStoreCommand(Guid Id) : IRequest<Result<Unit>>;
