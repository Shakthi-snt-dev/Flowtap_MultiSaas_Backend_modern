using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.AddProductMedia;

public record AddProductMediaCommand(
    Guid ProductId,
    string Url,
    bool IsPrimary = false,
    int SortOrder = 0) : IRequest<Result<Guid>>;
