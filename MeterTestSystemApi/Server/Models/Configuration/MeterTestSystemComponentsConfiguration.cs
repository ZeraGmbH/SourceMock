namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Overall configuration of the meter test system.
/// </summary>
public class MeterTestSystemComponentsConfiguration
{
    /// <summary>
    /// Configuration if each test position. Entries must not be 
    /// null but test positions can be disabled if needed.
    /// </summary>
    public List<TestPositionConfiguration> TestPositions { get; set; } = [];

    /// <summary>
    /// Set to use the MP2020 control using a CR2020.
    /// </summary>
    public bool EnableMP2020Control { get; set; }

    /// <summary>
    /// Individual flags to set if DC component should be used.
    /// </summary>
    public DCComponents DCComponents { get; set; }

    /// <summary>
    /// Report if any DC component is selected.
    /// </summary>
    public bool EnableDCComponents => DCComponents != DCComponents.None;
}