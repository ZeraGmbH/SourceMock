using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using ErrorCalculatorApi.Models;
using MongoDB.Driver;

namespace ErrorCalculatorApi.Actions.Device;

/// <summary>
/// MAD communication using a TCP channel.
/// </summary>
public class MadTcpConnection : IMadConnection
{
    private static readonly Regex parseEndpoint = new(@"^(.+):([0-9]+)$");

    private TcpClient client = new();

    private string server = null!;

    private ushort port;

    /// <inheritdoc/>
    public void Dispose()
    {
        using (client)
            client = null!;
    }

    /// <inheritdoc/>
    public async Task<XmlDocument> Execute(XmlDocument request)
    {
        /* Serialize and send request. */
        if (!client.Connected) await client.ConnectAsync(server, port);

        var stream = client.GetStream();

        await stream.WriteAsync(Encoding.UTF8.GetBytes(request.OuterXml));

        /* Wait for reply. [TODO] */
        var res = request.OuterXml;

        /* Deserialize. */
        var doc = new XmlDocument();

        doc.LoadXml(res);

        return doc;
    }

    /// <inheritdoc/>
    public Task Initialize(ErrorCalculatorConfiguration config)
    {
        /* Test endpoint. */
        var match = parseEndpoint.Match(config.Endpoint ?? string.Empty);

        if (match == null) throw new ArgumentException("invalid endpoint");

        /* Remember. */
        server = match.Groups[1].Value;
        port = ushort.Parse(match.Groups[2].Value);

        return Task.CompletedTask;
    }
}
