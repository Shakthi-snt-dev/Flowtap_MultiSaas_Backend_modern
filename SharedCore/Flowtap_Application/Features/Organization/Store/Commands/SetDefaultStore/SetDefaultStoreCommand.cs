using Flowtap_Application.Common.DTOs;
using MediatR;
using System;

namespace Flowtap_Application.Features.Organization.Store.Commands.SetDefaultStore;

public record SetDefaultStoreCommand(Guid StoreId) : IRequest<Result<Unit>>;
