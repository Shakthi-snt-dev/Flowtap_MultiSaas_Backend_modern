namespace Flowtap_Application.Common.Interfaces;

public interface ISubscriptionService
{
    Task<bool> HasFeatureAccess(Guid companyId, string featureCode);
    Task<bool> CanAddStore(Guid companyId);
    Task<bool> CanAddEmployee(Guid companyId);
}
