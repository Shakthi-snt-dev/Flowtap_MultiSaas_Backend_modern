namespace Flowtap_Infrastructure.Settings;

public class StorageSettings
{
    public string Provider { get; set; } = "Local"; // "Local" or "S3"
    public string LocalBasePath { get; set; } = "wwwroot/uploads";
    public string S3BucketName { get; set; } = string.Empty;
    public string S3Region { get; set; } = string.Empty;
    public string S3AccessKey { get; set; } = string.Empty;
    public string S3SecretKey { get; set; } = string.Empty;
}
