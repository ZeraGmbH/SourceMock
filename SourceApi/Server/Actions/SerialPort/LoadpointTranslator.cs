using System.Text;
using SerialPortProxy;
using ZERA.WebSam.Shared;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Source;
using ZERA.WebSam.Shared.Models;

namespace SourceApi.Actions.SerialPort;

/// <summary>
/// 
/// </summary>
public abstract class LoadpointTranslator : ILoadpointTranslator
{
    /// <inheritdoc/>
    public abstract SerialPortRequest[] ToSerialPortRequests(TargetLoadpoint loadpoint);

    /// <summary>
    /// Create serial port requests to set the frequency.
    /// </summary>
    /// <param name="command">Command to send.</param>
    /// <param name="reply">Answer to expect.</param>
    /// <param name="loadpoint">The full loadpoint definition.</param>
    /// <param name="requests">The current list of all requests.</param>
    protected static void CreateFrequencyRequests(string command, string reply, TargetLoadpoint loadpoint, List<SerialPortRequest> requests)
    {
        var request = new StringBuilder(command);

        var frequency = loadpoint.Frequency;

        /* Only set the frequency if syncthetic mode is requested - else use 00.00. */
        if (frequency.Mode == FrequencyMode.SYNTHETIC)
            request.Append(frequency.Value.Format("00.00", withUnit: false));
        else if (frequency.Mode == FrequencyMode.THIRD_OF_GRID_SYNCHRONOUS)
            request.Append("00.16");
        else
            request.Append("00.00");

        /* Finish the request and declare the expected success command. */
        requests.Add(SerialPortRequest.Create(request.ToString(), reply));
    }

    /// <summary>
    /// Create the serial port request to set the voltages and corresponding angles for all phases.
    /// </summary>
    /// <param name="command">Command to send.</param>
    /// <param name="reply">Reply to expect.</param>
    /// <param name="loadpoint">The full loadpoint definition.</param>
    /// <param name="requests">The current list of all requests.</param>
    protected static void CreateVoltageRequests(string command, string reply, TargetLoadpoint loadpoint, List<SerialPortRequest> requests)
    {
        var request = new StringBuilder(command);

        /* Set U-Absch. */
        request.Append('A');

        /* Set U-Dos. */
        request.Append('E');

        /* Process all phases. */
        for (var i = 0; i < 3; i++)
        {
            /* Set indicator for phase, API R=L1=A, S=L2=B, T=L3=C. */
            request.Append(i == 0 ? "R" : i == 1 ? "S" : "T");

            /* See if a voltage is supplied. */
            if (i < loadpoint.Phases.Count)
            {
                var voltage = loadpoint.Phases[i]?.Voltage;

                if (voltage != null && voltage.AcComponent != null)
                {
                    /* Convert voltage and angle to API protocol format. */
                    request.Append(voltage.AcComponent.Rms.Format("000.000", withUnit: false));
                    request.Append(voltage.AcComponent.Angle.Format("000.00", withUnit: false));

                    continue;
                }
            }

            /* If no voltage is supplied simply sende value of 0 with angle of 0. */
            request.Append("000.000000.00");
        }

        /* Finish the request and declare the expected success command. */
        requests.Add(SerialPortRequest.Create(request.ToString(), reply));
    }

    /// <summary>
    /// Create the serial port request to set the currents and corresponding angles for all phases.
    /// </summary>
    /// <param name="command">Command to send.</param>
    /// <param name="reply">Reply to expect.</param>
    /// <param name="loadpoint">The full loadpoint definition.</param>
    /// <param name="requests">The current list of all requests.</param>
    protected static void CreateCurrentRequests(string command, string reply, TargetLoadpoint loadpoint, List<SerialPortRequest> requests)
    {
        var request = new StringBuilder(command);

        /* Set I-Off. */
        request.Append('A');


        /* Set Dimens. */
        request.Append('A');

        /* Process all phases. */
        for (var i = 0; i < 3; i++)
        {
            /* Set indicator for phase, API R=L1=A, S=L2=B, T=L3=C. */
            request.Append(i == 0 ? "R" : i == 1 ? "S" : "T");

            /* See if a current is supplied. */
            if (i < loadpoint.Phases.Count)
            {
                var current = loadpoint.Phases[i]?.Current;

                if (current != null && current.AcComponent != null)
                {
                    /* Convert voltage and angle to API protocol format. */
                    request.Append(current.AcComponent.Rms.Format("000.000", withUnit: false));
                    request.Append(current.AcComponent.Angle.Format("000.00", withUnit: false));

                    continue;
                }
            }

            /* If no voltage is supplied simply sende value of 0 with angle of 0. */
            request.Append("000.000000.00");
        }

        /* Finish the request and declare the expected success command. */
        requests.Add(SerialPortRequest.Create(request.ToString(), reply));
    }

    /// <summary>
    /// Create serial port request to switch phases.
    /// </summary>
    /// <param name="command">Command to send.</param>
    /// <param name="reply">Reply to expect.</param>
    /// <param name="loadpoint">The full loadpoint definition.</param>
    /// <param name="requests">The current list of all requests.</param>
    protected static void CreatePhaseRequests(string command, string reply, TargetLoadpoint loadpoint, List<SerialPortRequest> requests)
    {
        var request = new StringBuilder(command);

        /* Process all phases. */
        for (var i = 0; i < 3; i++)
            if (i < loadpoint.Phases.Count)
                request.Append(loadpoint.Phases[i]?.Voltage?.On == true ? "E" : "A");
            else
                request.Append('A');

        for (var i = 0; i < 3; i++)
            if (i < loadpoint.Phases.Count)
                request.Append(loadpoint.Phases[i]?.Current?.On == true ? "P" : "A");
            else
                request.Append('A');

        /* H1. */
        request.Append('A');

        /* H2. */
        request.Append('A');

        /* R. */
        request.Append('A');

        /* Finish the request and declare the expected success command. */
        requests.Add(SerialPortRequest.Create(request.ToString(), reply));
    }

    protected static TargetLoadpoint ConvertFromIECtoDin(TargetLoadpoint loadpoint)
    {
        // Not manipulating the original loadpoint object
        var result = LibUtils.DeepCopy(loadpoint);

        var firstActiveVoltagePhase = loadpoint.Phases.FindIndex(p => p.Voltage.On);

        ConvertAngles(result, firstActiveVoltagePhase);

        return result;
    }

    private static void ConvertAngles(TargetLoadpoint loadpoint, int firstActiveVoltagePhase)
    {
        foreach (var phase in loadpoint.Phases)
        {
            phase.Voltage.AcComponent!.Angle = (-phase.Voltage.AcComponent.Angle).Normalize();
            phase.Current.AcComponent!.Angle = (-phase.Current.AcComponent.Angle).Normalize();
        }
        ;

        if (firstActiveVoltagePhase < 0) return;

        var angle = loadpoint.Phases[firstActiveVoltagePhase].Voltage.AcComponent!.Angle;

        if (!angle) return;

        foreach (var phase in loadpoint.Phases)
        {
            phase.Voltage.AcComponent!.Angle = (phase.Voltage.AcComponent.Angle - angle).Normalize();
            phase.Current.AcComponent!.Angle = (phase.Current.AcComponent.Angle - angle).Normalize();
        }
    }
}
