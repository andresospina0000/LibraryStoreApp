namespace Library.Infrastructure.Security;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Library.API";
    public string Audience { get; set; } = "Library.Client";
    public int ExpiryMinutes { get; set; } = 120;
}
