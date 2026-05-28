namespace Flowtap_Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default);
    Task SendTemplatedAsync(string to, string templateName, object model, CancellationToken ct = default);
}
