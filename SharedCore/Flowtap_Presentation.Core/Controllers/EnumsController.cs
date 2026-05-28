using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Flowtap_Presentation.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/v1/enums")]
[Produces("application/json")]
public class EnumsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEnums()
    {
        var domainAssembly = typeof(Flowtap_Domain.BoundedContexts.Modules.Inventory.Entities.Warehouse).Assembly;
        
        var enumTypes = domainAssembly.GetTypes()
            .Where(t => t.IsEnum && t.IsPublic && (t.Namespace?.Contains("BoundedContexts") ?? false))
            .OrderBy(t => t.Name);

        var result = new Dictionary<string, List<object>>();

        foreach (var type in enumTypes)
        {
            var values = new List<object>();
            var underlyingType = Enum.GetUnderlyingType(type);

            foreach (var val in Enum.GetValues(type))
            {
                var name = val.ToString() ?? string.Empty;
                var value = Convert.ChangeType(val, underlyingType);

                values.Add(new
                {
                    Value = value,
                    Name = name,
                    Label = SplitCamelCase(name)
                });
            }

            result[type.Name] = values;
        }

        return Ok(new
        {
            Success = true,
            Message = "Enums retrieved successfully.",
            Data = result
        });
    }

    private static string SplitCamelCase(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return input;
        return Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
    }
}
