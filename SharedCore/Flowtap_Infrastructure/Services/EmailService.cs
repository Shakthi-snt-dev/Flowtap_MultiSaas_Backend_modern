using Flowtap_Application.Common.Interfaces;
using Flowtap_Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Flowtap_Infrastructure.Services;

public class EmailService(IOptions<EmailSettings> emailOptions, ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = emailOptions.Value;

    public async Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(new MailboxAddress(string.Empty, to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder();
        if (isHtml)
            bodyBuilder.HtmlBody = body;
        else
            bodyBuilder.TextBody = body;

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            // Use SslOnConnect for Port 465, StartTls for 587
            var options = _settings.SmtpPort == 465 
                ? SecureSocketOptions.SslOnConnect 
                : SecureSocketOptions.StartTls;

            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, options, ct);
            await client.AuthenticateAsync(_settings.Username, _settings.Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }

    public async Task SendTemplatedAsync(string to, string templateName, object model, CancellationToken ct = default)
    {
        logger.LogInformation("Sending templated email '{Template}' to {To}", templateName, to);

        string subject = templateName;
        string body = string.Empty;

        if (templateName == "VerifyEmail")
        {
            subject = "Verify Your Email Address";
            var data = (dynamic)model;
            string name = data.GetType().GetProperty("Name")?.GetValue(data, null) as string ?? "Customer";
            string token = data.GetType().GetProperty("Token")?.GetValue(data, null) as string ?? string.Empty;
            string verificationUrl = $"http://localhost:3000/verify-email?token={token}";

            body = $@"
                <html>
                <body style='font-family: sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                        <h2 style='color: #333;'>Welcome to Flowtap, {name}!</h2>
                        <p>Thank you for registering. Please verify your email address by clicking the link below:</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{verificationUrl}' style='background-color: #3b82f6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold;'>Verify Email</a>
                        </div>
                        <p style='font-size: 12px; color: #666;'>Or copy and paste this link: <br/> {verificationUrl}</p>
                        <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;' />
                        <p style='font-size: 12px; color: #999;'>This link will expire in 24 hours.</p>
                    </div>
                </body>
                </html>";
        }
        else
        {
            body = System.Text.Json.JsonSerializer.Serialize(model, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            body = $"<pre>{body}</pre>";
        }

        await SendAsync(to, subject, body, isHtml: true, ct);
    }
}
