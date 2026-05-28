using Flowtap_Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace Flowtap_Infrastructure.Services;

/// <summary>
/// Sends SMS via Twilio REST API (no SDK — plain HttpClient).
///
/// Credentials are passed per-call by SmsDispatchService, which resolves them
/// from the company's Twilio integration record in the Integrations table.
/// This keeps the service stateless and testable without any Settings injection.
///
/// Twilio endpoint:
///   POST https://api.twilio.com/2010-04-01/Accounts/{AccountSid}/Messages.json
///   Auth: HTTP Basic  AccountSid : AuthToken
/// </summary>
public class SmsService(
    IHttpClientFactory httpClientFactory,
    ILogger<SmsService> logger) : ISmsService
{
    public async Task SendAsync(
        string phoneNumber,
        string message,
        string accountSid,
        string authToken,
        string fromNumber,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(accountSid) ||
            string.IsNullOrWhiteSpace(authToken)  ||
            string.IsNullOrWhiteSpace(fromNumber))
        {
            logger.LogWarning(
                "SMS skipped — Twilio credentials not configured for recipient {Phone}. " +
                "Configure the Twilio integration in IntegrationsPage.", phoneNumber);
            return;
        }

        var client = httpClientFactory.CreateClient("Twilio");

        var credentials = Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{accountSid}:{authToken}"));
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);

        var url = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Messages.json";

        var body = new FormUrlEncodedContent([
            new("From", fromNumber),
            new("To",   phoneNumber),
            new("Body", message),
        ]);

        try
        {
            var response = await client.PostAsync(url, body, ct);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(ct);
                logger.LogError("Twilio SMS failed to {Phone}: {StatusCode} — {Error}",
                    phoneNumber, response.StatusCode, error);
                throw new InvalidOperationException($"Twilio SMS error {response.StatusCode}: {error}");
            }
            logger.LogInformation("SMS sent to {Phone}", phoneNumber);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            logger.LogError(ex, "Network error sending SMS to {Phone}", phoneNumber);
            throw;
        }
    }
}
