namespace DeviceApiSharedLibrary.Models;

/// <summary>
/// Describes a document including a detailed version information.
/// </summary>
/// <typeparam name="T">The concrete type of the document.</typeparam>
public class HistoryItem<T> where T : DatabaseObject
{
    /// <summary>
    /// The document in the indicated version.
    /// </summary>
    public T Item { get; set; } = default!;

    /// <summary>
    /// Detailed change information for the document - including the version
    /// number which is 1-based.
    /// </summary>
    public HistoryInfo Version { get; set; } = default!;
}
