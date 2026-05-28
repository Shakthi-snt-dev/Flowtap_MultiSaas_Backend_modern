using Flowtap_Application.Common.DTOs;

namespace Flowtap_Application.Common.Interfaces;

public interface ITaxTemplateService
{
    Task<Result<bool>> ApplyTemplateAsync(Guid storeId, string countryCode, CancellationToken ct = default);
}
