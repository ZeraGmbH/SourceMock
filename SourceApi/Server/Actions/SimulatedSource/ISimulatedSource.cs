using ZERA.WebSam.Shared.Provider;
using SourceApi.Model;

namespace SourceApi.Actions.Source
{
    /// <summary>
    /// Extends <see cref="ISource"/> by functions to influece the sources behaviour.
    /// </summary>
    public interface ISimulatedSource
    {
        /// <summary>
        /// Sets the state of the simulated source.
        /// </summary>
        /// <param name="simulatedSourceState">The desired state.</param>
        public void SetSimulatedSourceState(SimulatedSourceState simulatedSourceState);

        /// <summary>
        /// Gets the current state of the simulated source.
        /// </summary>
        /// <returns>The current state.</returns>
        public SimulatedSourceState? GetSimulatedSourceState();
    }
}
