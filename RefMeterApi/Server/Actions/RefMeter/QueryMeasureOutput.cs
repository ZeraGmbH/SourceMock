using Microsoft.AspNetCore.Mvc;
using RefMeterApi.Actions.Parsers;
using RefMeterApi.Models;
using SerialPortProxy;

namespace RefMeterApi.Actions.RefMeter;

/// <summary>
/// Query a device for the current measurement data.
/// </summary>
public static class QueryMeasureOutput
{
    /// <summary>
    /// Queries a device connected to the serial port for the current
    /// measurement results.
    /// </summary>
    /// <param name="device">Device to connect to.</param>
    /// <returns>All measurement data.</returns>
    public static Task<MeasureOutput> Execute(SerialPortConnection device) =>
        device
            .Execute(SerialPortRequest.Create("AME", "AMEACK"))[0]
            .ContinueWith(t => MeasureValueOutputParser.Parse(t.Result));
}
