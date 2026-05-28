namespace Flowtap_Infrastructure.Settings;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Flowtap";
    public string Audience { get; set; } = "Flowtap";
    public int AccessTokenExpiryMinutes { get; set; } = 60;
    public int RefreshTokenExpiryDays { get; set; } = 30;
}
