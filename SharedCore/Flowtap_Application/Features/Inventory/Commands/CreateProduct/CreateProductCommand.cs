using Flowtap_Application.Common.DTOs;
using MediatR;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Flowtap_Application.Features.Inventory.Commands.CreateProduct;

public record CreateProductCommand(
    Guid CompanyId, Guid CategoryId, string Name,
    [property: JsonConverter(typeof(StringOrNumberConverter))] string Kind, string? SKU,
    decimal DefaultCostPrice, decimal DefaultSalePrice,
    bool IsSerialized, bool IsUniversal, Guid? TaxSlabId, string? HsnCode) : IRequest<Result<Guid>>;

public class StringOrNumberConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (reader.TryGetInt32(out int intValue))
            {
                return intValue.ToString();
            }
            if (reader.TryGetDouble(out double doubleValue))
            {
                return doubleValue.ToString();
            }
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString() ?? string.Empty;
        }
        if (reader.TokenType == JsonTokenType.True)
        {
            return "true";
        }
        if (reader.TokenType == JsonTokenType.False)
        {
            return "false";
        }
        if (reader.TokenType == JsonTokenType.Null)
        {
            return string.Empty;
        }

        using var doc = JsonDocument.ParseValue(ref reader);
        return doc.RootElement.ToString();
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

