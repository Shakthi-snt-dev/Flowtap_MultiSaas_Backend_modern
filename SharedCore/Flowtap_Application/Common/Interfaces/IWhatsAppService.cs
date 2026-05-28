namespace Flowtap_Application.Common.Interfaces;

public interface IWhatsAppService
{
    /// <summary>
    /// Sends a WhatsApp message via Meta Cloud API (WhatsApp Business Platform).
    /// Credentials are resolved per-company from the Integrations table by the dispatch service;
    /// pass empty strings to trigger a graceful skip with a warning log.
    /// </summary>
    /// <param name="toNumber">Recipient phone number in E.164 format, e.g. +919876543210</param>
    /// <param name="message">Plain-text message body (*bold* markdown supported)</param>
    /// <param name="accessToken">Meta API access token from the company's WhatsApp Business integration</param>
    /// <param name="phoneNumberId">WhatsApp Business phone number ID from the Meta integration</param>
    Task SendAsync(
        string toNumber,
        string message,
        string accessToken,
        string phoneNumberId,
        CancellationToken ct = default);
}
