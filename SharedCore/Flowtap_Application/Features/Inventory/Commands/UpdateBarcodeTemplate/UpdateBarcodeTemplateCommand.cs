using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Commands.UpdateBarcodeTemplate;

public record UpdateBarcodeTemplateCommand(
    Guid Id,
    Guid CompanyId,
    string? Name = null,
    string? CodeType = null,
    bool? IsDefault = null,
    int? LabelsPerRow = null,
    decimal? LabelWidthMm = null,
    decimal? LabelHeightMm = null,
    bool? ShowProductName = null,
    bool? ShowSKU = null) : IRequest<Result<bool>>;
