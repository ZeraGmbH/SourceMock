namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Coonnectio to a SMTP E-Mail Server.
/// </summary>
public class SmtpConfiguration
{
    /// <summary>
    /// Name or IP of the server.
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    /// Port to use when connecting to the server.
    /// </summary>
    public ushort? Port { get; set; }

    /// <summary>
    /// Sender of all e-mails.
    /// </summary>
    public string From { get; set; } = "websam@zera.de";

    /// <summary>
    /// Name of the mailbox.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password to authorize against mailbox.
    /// </summary>
    public string? Password { get; set; }
}