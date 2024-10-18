namespace SerialPortProxy;

/// <summary>
/// 
/// </summary>
public class SerialPortOptions
{
    /// <summary>
    /// 
    /// </summary>
    public int? BaudRate { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? DataBits { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public SerialPortParity? Parity { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public SerialPortStopBits? StopBits { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? ReadTimeout { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int? WriteTimeout { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? NewLine { get; set; }
}