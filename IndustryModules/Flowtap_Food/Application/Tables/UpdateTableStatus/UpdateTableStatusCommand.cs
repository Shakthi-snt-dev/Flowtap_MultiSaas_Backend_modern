using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.Tables.UpdateTableStatus;

public record UpdateTableStatusCommand(Guid Id, Guid CompanyId, string Status) : IRequest<Result>;
