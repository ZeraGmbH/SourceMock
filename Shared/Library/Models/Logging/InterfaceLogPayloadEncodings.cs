using System.Text.Json.Serialization;

namespace SharedLibrary.Models.Logging;

/// <summary>
/// How the payload string has to be interpreted to get
/// the messages exchanged with the interface.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum InterfaceLogPayloadEncodings
{
    /// <summary>
    /// Raw string i.e. payload IS the message.
    /// </summary>
    Raw = 0,

    /// <summary>
    /// JSON string, i.e. payload is JSON encoded
    /// object.
    /// </summary>
    Json = 1,

    /// <summary>
    /// XML string, i.e. payload is XML DOM in
    /// text representation.
    /// </summary>
    Xml = 2,

    /// <summary>
    /// SSPI request or response string.
    /// </summary>
    Sspi = 3,

    /// <summary>
    /// Binary data encoded using the Base64 algorithm.
    /// </summary>
    Base64 = 4
}
