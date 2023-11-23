using System.Text;

using SerialPortProxy;

using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Actions.SerialPort;

/// <summary>
/// 
/// </summary>
public static class LoadpointTranslator
{
    /// <summary>
    /// Create a sequence of related serial port request from any loadpoint.
    /// </summary>
    /// <param name="loadpoint">Some already validated loadpoint.</param>
    /// <returns>Sequence of requests to send as a single transaction.</returns>
    public static SerialPortRequest[] ToSerialPortRequests(Loadpoint loadpoint)
    {
        var requests = new List<SerialPortRequest>();

        CreateFrequencyRequests(loadpoint, requests);

        CreateVoltageRequests(loadpoint, requests);

        CreateCurrentRequests(loadpoint, requests);

        CreatePhaseRequests(loadpoint, requests);

        return requests.ToArray();
    }

    /// <summary>
    /// Create serial port requests to set the frequency.
    /// </summary>
    /// <param name="loadpoint">The full loadpoint definition.</param>
    /// <param name="requests">The current list of all requests.</param>
    private static void CreateFrequencyRequests(Loadpoint loadpoint, List<SerialPortRequest> requests)
    {
        var request = new StringBuilder("SFR");

        var frequency = loadpoint.Frequency;

        /* Only set the frequency if syncthetic mode is requested - else use 00.00. */
        if (frequency.Mode == FrequencyMode.SYNTHETIC)
            request.Append(frequency.Value.ToString("00.00"));
        else
            request.Append("00.00");

        /* Finish the request and declare the expected success command. */
        requests.Add(SerialPortRequest.Create(request.ToString(), "SOKFR"));
    }

    /// <summary>
    /// Create the serial port request to set the voltages and corresponding angles for all phases.
    /// </summary>
    /// <param name="loadpoint">The full loadpoint definition.</param>
    /// <param name="requests">The current list of all requests.</param>
    private static void CreateVoltageRequests(Loadpoint loadpoint, List<SerialPortRequest> requests)
    {
        var request = new StringBuilder("SUP");

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

                if (voltage != null)
                {
                    /* Convert voltage and angle to API protocol format. */
                    request.Append(voltage.Rms.ToString("000.000"));
                    request.Append(voltage.Angle.ToString("000.00"));

                    continue;
                }
            }

            /* If no voltage is supplied simply sende value of 0 with angle of 0. */
            request.Append("000.000000.00");
        }

        /* Finish the request and declare the expected success command. */
        requests.Add(SerialPortRequest.Create(request.ToString(), "SOKUP"));
    }

    /// <summary>
    /// Create the serial port request to set the currents and corresponding angles for all phases.
    /// </summary>
    /// <param name="loadpoint">The full loadpoint definition.</param>
    /// <param name="requests">The current list of all requests.</param>
    private static void CreateCurrentRequests(Loadpoint loadpoint, List<SerialPortRequest> requests)
    {
        var request = new StringBuilder("SIP");

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

                if (current != null)
                {
                    /* Convert voltage and angle to API protocol format. */
                    request.Append(current.Rms.ToString("000.000"));
                    request.Append(current.Angle.ToString("000.00"));

                    continue;
                }
            }

            /* If no voltage is supplied simply sende value of 0 with angle of 0. */
            request.Append("000.000000.00");
        }

        /* Finish the request and declare the expected success command. */
        requests.Add(SerialPortRequest.Create(request.ToString(), "SOKIP"));
    }

    /// <summary>
    /// Create serial port request to switch phases.
    /// </summary>
    /// <param name="loadpoint">The full loadpoint definition.</param>
    /// <param name="requests">The current list of all requests.</param>
    private static void CreatePhaseRequests(Loadpoint loadpoint, List<SerialPortRequest> requests)
    {
        var request = new StringBuilder("SUI");

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
        requests.Add(SerialPortRequest.Create(request.ToString(), "SOKUI"));
    }
}
