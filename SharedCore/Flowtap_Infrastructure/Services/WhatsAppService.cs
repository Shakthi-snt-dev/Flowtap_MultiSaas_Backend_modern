using Flowtap_Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Flowtap_Infrastructure.Services;

/// <summary>
/// Sends WhatsApp messages via Meta Cloud API (WhatsApp Business Platform).
///
/// API endpoint:
///   POST https://graph.facebook.com/v17.0/{phoneNumberId}/messages
///   Authorization: Bearer {accessToken}
///   Content-Type: application/json
///
/// Credentials come per-call from the company's WhatsApp Business integration
/// record (Provider = "whatsapp") stored in the Integrations table.
///
/// Setup steps (one-time per company):
///   1. Create a Meta Business account at business.facebook.com
///   2. Set up a WhatsApp Business app at developers.facebook.com
///   3. Get Access Token + Phone Number ID from the app dashboard
///   4. In Flowtap: Settings → Integrations → WhatsApp Business → Connect
///      Enter Access Token, Phone Number ID, Business Account ID
/// </summary>
public class WhatsAppService(
    IHttpClientFactory httpClientFactory,
    ILogger<WhatsAppService> logger) : IWhatsAppService
{
    public async Task SendAsync(
        string toNumber,
        string message,
        string accessToken,
        string phoneNumberId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken) ||
            string.IsNullOrWhiteSpace(phoneNumberId))
        {
            logger.LogWarning(
                "WhatsApp skipped — Meta credentials not configured for {Number}. " +
                "Configure the WhatsApp Business integration in IntegrationsPage.", toNumber);
            return;
        }

        var client = httpClientFactory.CreateClient("WhatsApp");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var url = $"https://graph.facebook.com/v17.0/{phoneNumberId}/messages";

        // Meta Cloud API expects the phone number WITHOUT '+' prefix
        var normalizedTo = toNumber.TrimStart('+');

        var payload = new
        {
            messaging_product = "whatsapp",
            to   = normalizedTo,
            type = "text",
            text = new { body = message }
        };

        var json    = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(url, content, ct);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                logger.LogError("WhatsApp (Meta) failed to {Number}: {StatusCode} — {Error}",
                    toNumber, response.StatusCode, error);
                throw new InvalidOperationException(
                    $"WhatsApp Meta API error {response.StatusCode}: {error}");
            }
            logger.LogInformation("WhatsApp message sent to {Number}", toNumber);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            logger.LogError(ex, "Network error sending WhatsApp to {Number}", toNumber);
            throw;
        }
    }
}
