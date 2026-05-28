namespace Flowtap_Infrastructure.Settings;

public class SmsSettings
{
    public string Provider { get; set; } = "Twilio";
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string FromNumber { get; set; } = string.Empty;

    // Twilio WhatsApp sender number (format: +14155238886)
    // Same AccountSid + AuthToken — Twilio adds "whatsapp:" prefix automatically in WhatsAppService
    public string WhatsAppFromNumber { get; set; } = string.Empty;
}
