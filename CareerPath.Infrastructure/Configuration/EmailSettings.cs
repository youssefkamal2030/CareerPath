namespace CareerPath.Infrastructure.Configuration;

/// <summary>
/// Strongly-typed configuration for email/SMTP settings
/// </summary>
public class EmailSettings
{
    public const string SectionName = "EmailSettings";

    /// <summary>
    /// Sender email address
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server hostname
    /// </summary>
    public string SmtpServer { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port (465 for SSL, 587 for TLS)
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// SMTP authentication username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// SMTP authentication password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Enable SSL/TLS encryption (default: true)
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Default display name for sender
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Validates that all required email settings are configured
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(From) &&
               !string.IsNullOrWhiteSpace(SmtpServer) &&
               Port > 0 &&
               !string.IsNullOrWhiteSpace(Username) &&
               !string.IsNullOrWhiteSpace(Password);
    }
}
