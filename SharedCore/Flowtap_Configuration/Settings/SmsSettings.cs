namespace Flowtap_Configuration.Settings;

public class SmsSettings
{
    public string Provider { get; set; } = "Twilio";
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string FromNumber { get; set; } = string.Empty;

    // Twilio WhatsApp sender number (format: +14155238886 — provided by Twilio sandbox or approved number)
    // Same AccountSid + AuthToken used — just different "From" format for WhatsApp
    public string WhatsAppFromNumber { get; set; } = string.Empty;
}
