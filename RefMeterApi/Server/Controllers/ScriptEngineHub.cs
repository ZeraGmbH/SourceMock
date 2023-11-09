using Microsoft.AspNetCore.SignalR;
using RefMeterApi.Models;

namespace RefMeterApi.Controllers;

/// <summary>
/// SignalR (Web Socket) server to control script execution.
/// </summary>
public class ScriptEngineHub : Hub
{
    /// <summary>
    /// Incoming message with no parameters triggers outgoing information.
    /// </summary>
    public Task GetVersion() => Clients.Caller.SendAsync("VersionInfo", new ScriptEngineVersion());

    /// <summary>
    /// When a client connects it will automatically receive a version information.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        /* Always start with the version information. */
        await GetVersion();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task DosageStart() => Clients.Caller.SendAsync("ScriptError");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task InputResponse(object value) => Clients.Caller.SendAsync("ScriptError", value);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task DosageDone() => Clients.Caller.SendAsync("ScriptError");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task DosageAbort() => Clients.Caller.SendAsync("ScriptError");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task Reconnect() => Clients.Caller.SendAsync("ScriptError");
}
