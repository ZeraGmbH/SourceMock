using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SerialPortProxy;

/// <summary>
/// 
/// </summary>
public class SerialPortReply
{
    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public List<string> Matches { get; set; } = [];

    /// <summary>
    /// Create a reply from the serial port response.
    /// </summary>
    /// <param name="request">Finished request with reply information.</param>
    /// <returns>New protocol description of the execution.</returns>
    public static SerialPortReply FromRequest(SerialPortRequest request)
    {
        var reply = new SerialPortReply();
        var match = request.EndMatch;

        if (match == null)
            reply.Matches.AddRange(request.Result.Task.Result);
        else
            reply.Matches.AddRange(match.Groups.Values.Select(g => g.Value));

        return reply;
    }
}
