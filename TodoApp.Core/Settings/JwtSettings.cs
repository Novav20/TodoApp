namespace TodoApp.Core.Settings;

public class JwtSettings
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int ExpirationInDays { get; set; } = 7;

    public void Validate()
    {
        if (string.IsNullOrEmpty(SecretKey))
            throw new InvalidOperationException("JWT SecretKey not found in configuration");
        
        if (string.IsNullOrEmpty(Issuer))
            throw new InvalidOperationException("JWT Issuer not found in configuration");
        
        if (string.IsNullOrEmpty(Audience))
            throw new InvalidOperationException("JWT Audience not found in configuration");
        
        if (ExpirationInDays <= 0)
            throw new InvalidOperationException("JWT ExpirationInDays must be greater than 0");
    }
}
