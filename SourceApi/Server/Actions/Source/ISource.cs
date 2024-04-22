using SharedLibrary.Models.Logging;
using SourceApi.Model;

namespace SourceApi.Actions.Source
{
    /// <summary>
    /// Interface of a class that simbulates the behaviour of a ZERA source.
    /// </summary>
    public interface ISource
    {
        /// <summary>
        /// Set if the source is fully configured and can be used.
        /// </summary>
        bool Available { get; }

        /// <summary>
        /// Gets the capabilities of this source.
        /// </summary>
        /// <returns>The corresponding <see cref="SourceCapabilities"/>-Object for this source.</returns>
        public Task<SourceCapabilities> GetCapabilities();

        /// <summary>
        /// Sets a specified loadpoint imediatly.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="loadpoint">The loadpoint to be set.</param>
        /// <returns>The corresponding value of <see cref="SourceApiErrorCodes"/> with regard to the success of the operation.</returns>
        public Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint);

        /// <summary>
        /// Turns off the source.
        /// </summary>
        /// <returns>The corresponding value of <see cref="SourceApiErrorCodes"/> with regard to the success of the operation.</returns>
        public Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger);

        /// <summary>
        /// Gets the currently set loadpoint.
        /// </summary>
        /// <returns>The loadpoint, null if none was set.</returns>
        public TargetLoadpoint? GetCurrentLoadpoint();

        /// <summary>
        /// Reports information on the active loadpoint.
        /// </summary>
        public LoadpointInfo GetActiveLoadpointInfo();

        /// <summary>
        /// Set the DOS mode.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="on">set to turn on.</param>
        Task SetDosageMode(IInterfaceLogger logger, bool on);

        /// <summary>
        /// Define the dosage energy.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="value">Value in Wh.</param>
        /// <param name="meterConstant">The meter constant used in the reference meter.</param>
        Task SetDosageEnergy(IInterfaceLogger logger, double value, double meterConstant);

        /// <summary>
        /// Start a dosage measurement.
        /// </summary>
        Task StartDosage(IInterfaceLogger logger);

        /// <summary>
        /// Terminate a dosage measurement.
        /// </summary>
        Task CancelDosage(IInterfaceLogger logger);

        /// <summary>
        /// Reports the remaining energy in the current dosage operation.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="meterConstant">The meter constant used in the reference meter.</param>
        /// <returns>Information on the current progress of the dosage measurement.</returns>
        Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, double meterConstant);

        /// <summary>
        /// If set the dosage mode has been activated but current is switch off.
        /// </summary>
        Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger);
    }
}