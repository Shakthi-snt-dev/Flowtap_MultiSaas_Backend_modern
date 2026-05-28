using Flowtap_Domain.BoundedContexts.Core.Organization.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Flowtap_Presentation.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequiresIndustryAttribute : TypeFilterAttribute
{
    public RequiresIndustryAttribute(params IndustryType[] industries) : base(typeof(IndustryAccessFilter))
    {
        Arguments = [industries];
    }
}
