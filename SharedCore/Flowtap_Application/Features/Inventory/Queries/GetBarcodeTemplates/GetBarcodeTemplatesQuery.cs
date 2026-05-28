using Flowtap_Application.Common.DTOs;
using MediatR;

namespace Flowtap_Application.Features.Inventory.Queries.GetBarcodeTemplates;

public record BarcodeTemplateDto(
    Guid Id,
    string Name,
    string CodeType,
    bool IsDefault,
    int LabelsPerRow,
    decimal LabelWidthMm,
    decimal LabelHeightMm,
    bool ShowProductName,
    bool ShowSKU);

public record GetBarcodeTemplatesQuery(Guid CompanyId) : IRequest<Result<List<BarcodeTemplateDto>>>;
