using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Food.Application.Tables.CreateTable;

public record CreateTableCommand(
    Guid CompanyId,
    Guid LocationId,
    string Name,
    int Capacity,
    string? Section) : IRequest<Result<Guid>>;
