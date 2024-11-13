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
}