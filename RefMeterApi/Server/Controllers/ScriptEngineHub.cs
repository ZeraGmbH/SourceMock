using Microsoft.AspNetCore.SignalR;

namespace RefMeterApi.Controllers;

/// <summary>
/// Sample DTO for outgoing message.
/// </summary>
public class VersionInfo
{
    /// <summary>
    /// Some data in the sample.
    /// </summary>
    public string Version { get; set; } = "0.0";
}

/// <summary>
/// SignalR (Web Socket) server to control script execution.
/// </summary>
public class ScriptEngineHub : Hub
{
    /// <summary>
    /// Incoming message with no parameters triggers outgoing information.
    /// </summary>
    public Task GetVersion() => Clients.Caller.SendAsync("VersionInfo", new VersionInfo());
}
