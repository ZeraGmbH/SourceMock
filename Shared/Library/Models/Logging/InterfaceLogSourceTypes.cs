namespace SharedLibrary.Models.Logging;

/// <summary>
/// Possible sources of a log entry.
/// </summary>
public enum InterfaceLogSourceTypes
{
    /// <summary>
    /// A source.
    /// </summary>
    Source = 0,

    /// <summary>
    /// A reference meter.
    /// </summary>
    ReferenceMeter = 1,

    /// <summary>
    /// An error calculator.
    /// </summary>
    ErrorCalculator = 2,

    /// <summary>
    /// A device under test.
    /// </summary>
    DeviceUnderTest = 3,

    /// <summary>
    /// The meter test system itself.
    /// </summary>
    MeterTestSystem = 4,
}