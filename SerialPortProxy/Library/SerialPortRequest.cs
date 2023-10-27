namespace SerialPortProxy;

/// <summary>
/// Queue entry for a single request to the device.
/// </summary>
public class SerialPortRequest
{
    /// <summary>
    /// Command string to send to the device - &lt;qCR> automatically added.
    /// </summary>
    public readonly string Command;

    /// <summary>
    /// Expected termination line.
    /// </summary>
    public readonly string End;

    /// <summary>
    /// Promise for the response of the device.
    /// </summary>
    public readonly TaskCompletionSource<string[]> Result = new();

    /// <summary>
    /// Initialize a new request.
    /// </summary>
    /// <param name="command">Command string.</param>
    /// <param name="end">Final string from the device to end the request.</param>
    private SerialPortRequest(string command, string end)
    {
        Command = command;
        End = end;
    }

    /// <summary>
    /// Create a new request.
    /// </summary>
    /// <param name="command">Command string.</param>
    /// <param name="end">Final string from the device to end the request.</param>
    /// <returns>The new request.</returns>
    public static SerialPortRequest Create(string command, string end) => new SerialPortRequest(command, end);
}
