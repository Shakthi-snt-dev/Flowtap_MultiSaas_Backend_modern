namespace Flowtap_Application.Common.Interfaces;

public interface ISmsService
{
    /// <summary>
    /// Sends an SMS via Twilio REST API.
    /// Credentials are resolved per-company from the Integrations table by the dispatch service;
    /// pass empty strings to trigger a graceful skip with a warning log.
    /// </summary>
    Task SendAsync(
        string phoneNumber,
        string message,
        string accountSid,
        string authToken,
        string fromNumber,
        CancellationToken ct = default);
}
