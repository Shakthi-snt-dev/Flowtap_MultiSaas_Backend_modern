using Flowtap_Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Flowtap_Infrastructure.Services;

public class SmsService(ILogger<SmsService> logger) : ISmsService
{
    public Task SendAsync(string phoneNumber, string message, CancellationToken ct = default)
    {
        logger.LogInformation("SMS to {Phone}: {Message}", phoneNumber, message);
        return Task.CompletedTask;
    }
}
