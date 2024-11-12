
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApi.Models;

/// <summary>
/// Interface to communicate with a burden.
/// </summary>
public interface IBurden
{
    /// <summary>
    /// Set if a burden has been configured and can be used.
    /// </summary>
    /// <value></value>
    bool IsAvailable { get; }

    /// <summary>
    /// Read the version information from the burden.
    /// </summary>
    /// <param name="interfaceLogger">Logging helper.</param>
    /// <returns>Version Information.</returns>
    Task<BurdenVersion> GetVersionAsync(IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Ture the burden on or off.
    /// </summary>
    /// <param name="on">Set to turn the burden on.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task SetActiveAsync(bool on, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Get the names of all burdens.
    /// </summary>
    /// <param name="interfaceLogger">Logging helper.</param>
    /// <returns>List of burdens.</returns>
    Task<string[]> GetBurdensAsync(IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Read a single calibration.
    /// </summary>
    /// <param name="burden">Burden to request.</param>
    /// <param name="range">Range to inspect.</param>
    /// <param name="step">Step to request.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    /// <returns>Calibration values or null if not calibrated.</returns>
    Task<Calibration?> GetCalibrationAsync(string burden, string range, string step, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Program burdens.
    /// </summary>
    /// <remarks>This call may take a rather long time.</remarks>
    /// <param name="burden">Burden to program or empty for all burdens.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task ProgramAsync(string? burden, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Apply a calibration to the FETs of the current active step.
    /// </summary>
    /// <param name="calibration">New calibration data.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task SetTransientCalibrationAsync(Calibration calibration, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Apply a calibration to the burden - without programming the settings 
    /// will be lost as soon as the burden is powered off.
    /// </summary>
    /// <param name="burden">Burden to request.</param>
    /// <param name="range">Range to inspect.</param>
    /// <param name="step">Step to request.</param>
    /// <param name="calibration">New calibration data.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task SetPermanentCalibrationAsync(string burden, string range, string step, Calibration calibration, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Request the current status of the burden.
    /// </summary>
    /// <param name="interfaceLogger">Logging helper.</param>
    /// <returns>Current status.</returns>
    Task<BurdenStatus> GetStatusAsync(IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Choose the active burden.
    /// </summary>
    /// <param name="burden">Burden to use.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task SetBurdenAsync(string burden, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Choose the active range.
    /// </summary>
    /// <param name="range">Range to use.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task SetRangeAsync(string range, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Choose the active step.
    /// </summary>
    /// <param name="step">Step to use.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task SetStepAsync(string step, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Measure the current values.
    /// </summary>
    /// <param name="interfaceLogger">Logging helper.</param>
    /// <returns>Values of a single phase.</returns>
    Task<BurdenValues> MeasureAsync(IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Cancel the current calibration.
    /// </summary>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task CancelCalibrationAsync(IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Starts or stops a measurement calibration.
    /// </summary>
    /// <param name="on">Set to start the measurement.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    Task SetMeasuringCalibrationAsync(bool on, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Get the ranges for a single burden.
    /// </summary>
    /// <param name="burden">Name of the burden.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    /// <returns>List of names.</returns>
    Task<string[]> GetRangesAsync(string burden, IInterfaceLogger interfaceLogger);

    /// <summary>
    /// Get the steps for a single burden.
    /// </summary>
    /// <param name="burden">Name of the burden.</param>
    /// <param name="interfaceLogger">Logging helper.</param>
    /// <returns>List of names.</returns>
    Task<string[]> GetStepsAsync(string burden, IInterfaceLogger interfaceLogger);
}