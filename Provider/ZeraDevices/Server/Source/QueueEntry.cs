namespace ZeraDevices.Source;

/// <summary>
/// Represents a single reply from the device.
/// </summary>
public class QueueEntry
{
    /// <summary>
    /// The reply text.
    /// </summary>
    public readonly string Reply;

    /// <summary>
    /// Optional delay to simulate real device behaviour.
    /// </summary>
    public readonly int Delay;

    /// <summary>
    /// Create a new reply entry.
    /// </summary>
    /// <param name="reply">Message to reply.</param>
    /// <param name="delay">Number of milliseconds to wait before the message is sent.</param>
    public QueueEntry(string reply, int delay)
    {
        Delay = delay;
        Reply = reply;
    }

    /// <summary>
    /// Povide auto conversion in the regular case with no delay.
    /// </summary>
    /// <param name="reply">Message to use as a reply, delay is assumed to be zero.</param>
    public static implicit operator QueueEntry(string reply) { return new QueueEntry(reply, 0); }
}
