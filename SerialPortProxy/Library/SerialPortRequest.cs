using System.Text.RegularExpressions;

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
    private readonly string? _StaticEnd;

    /// <summary>
    /// Expected termination pattern.
    /// </summary>
    private readonly Regex? _DynamicEnd;

    /// <summary>
    /// If pattern was used the result of the match.
    /// </summary>
    public Match? EndMatch { get; private set; }

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
        _StaticEnd = end;

        Command = command;
    }

    /// <summary>
    /// Initialize a new request.
    /// </summary>
    /// <param name="command">Command string.</param>
    /// <param name="end">Pattern expected to terminate the request.</param>
    private SerialPortRequest(string command, Regex end)
    {
        _DynamicEnd = end;

        Command = command;
    }

    /// <summary>
    /// Create a new request.
    /// </summary>
    /// <param name="command">Command string.</param>
    /// <param name="end">Pattern expected to terminate the request.</param>
    /// <returns>The new request.</returns>
    public static SerialPortRequest Create(string command, Regex end) => new(command, end);

    /// <summary>
    /// Create a new request.
    /// </summary>
    /// <param name="command">Command string.</param>
    /// <param name="end">Final string from the device to end the request.</param>
    /// <returns>The new request.</returns>
    public static SerialPortRequest Create(string command, string end) => new(command, end);

    /// <summary>
    /// Match a reply against the termination condition and report result.
    /// </summary>
    /// <param name="reply">String from the device.</param>
    /// <returns>Set if the reply matches the end condition.</returns>
    public bool Match(string reply)
    {
        /* Use static string - the fastest way to process. */
        if (_DynamicEnd == null)
            return _StaticEnd == reply;

        /* Match against the end pattern and remember result. */
        EndMatch = _DynamicEnd.Match(reply);

        /* If reply matches end pattern report success. */
        return EndMatch.Success;
    }
}
