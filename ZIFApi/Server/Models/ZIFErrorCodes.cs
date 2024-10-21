using System.Text.Json.Serialization;

namespace ZIFApi.Models;

/// <summary>
/// Possible error conditions when communicating with
/// a ZIF socket..
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ZIFErrorCodes
{
    /// <summary>
    /// Something other than ACK or NAK received.
    /// </summary>
    NotACKorNAK,

    /// <summary>
    /// Invalid length in reply.
    /// </summary>
    BadLength,

    /// <summary>
    /// NAK received. 
    /// </summary>
    NAK,

    /// <summary>
    /// Communication channel closed.
    /// </summary>
    NoMoreData,

    /// <summary>
    /// Reply does not match command.
    /// </summary>
    OutOfBand,

    /// <summary>
    /// Error sending data to the socket.
    /// </summary>
    Write,

    /// <summary>
    /// Reply was to short to analyze.
    /// </summary>
    TooShort,

    /// <summary>
    /// Error receiving data from the socket.
    /// </summary>
    Read,
}