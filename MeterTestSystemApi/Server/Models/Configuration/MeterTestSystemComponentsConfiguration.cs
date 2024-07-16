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

    /// <summary>
    /// Individual flags to set if transformer components shoul be used.
    /// </summary>
    public TransformerComponents TransformerComponents { get; set; }

    /// <summary>
    /// Report if any transformer component is selected.
    /// </summary>
    public bool EnableTransformerComponents => TransformerComponents != TransformerComponents.None;

    /// <summary>
    /// Individual flags to set if some MT310s2 function will be used.
    /// </summary>
    /// <value></value>
    public MT310s2Functions MT310s2Functions { get; set; }

    /// <summary>
    /// Report if any MT310s2 function should be used.
    /// </summary>
    public bool EnableMT310s2Functions => MT310s2Functions != MT310s2Functions.None;

    /// <summary>
    /// Set to use the Omega iBTHX temperature and humidity sensor.
    /// </summary>
    public bool EnableOmegaiBTHX { get; set; }

    /// <summary>
    /// Set to use a COM5003 as external reference (absolute).
    /// </summary>
    public bool EnableCOM5003 { get; set; }

    /// <summary>
    /// Set to use the IP watchdog for the whole test system.
    /// </summary>
    public bool EnableIPWatchDog { get; set; }

    /// <summary>
    /// Set to use the DTS100 keyboard.
    /// </summary>
    public bool EnableDTS100 { get; set; }
}