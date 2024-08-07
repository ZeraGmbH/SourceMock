using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
using SourceApi.Model;

namespace SourceApi.Actions.Source
{
    /// <summary>
    /// Interface of a class that simulates the behaviour of the dosage capabilities of a ZERA source.
    /// </summary>
    public interface IDosage
    {
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
        Task SetDosageEnergy(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant);

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
        Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, MeterConstant meterConstant);

        /// <summary>
        /// If set the dosage mode has been activated but current is switch off.
        /// </summary>
        Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger);
    }
}