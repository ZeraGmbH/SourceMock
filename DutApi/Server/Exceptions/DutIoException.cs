
namespace DutApi.Exceptions;

/// <summary>
/// Reports a problem with the connection to the
/// device under test.
/// </summary>
/// <param name="message">Domain message.</param>
/// <param name="inner">Original exception.</param>
public class DutIoException(string message, Exception inner) : IOException(message, inner)
{
}