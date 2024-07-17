using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Overall configuration of the meter test system.
/// </summary>
public class MeterTestSystemComponentsConfiguration
{
    /// <summary>
    /// Connection to the Frequency generator.
    /// </summary>
    public SerialPortConfiguration? FrequencyGenerator { get; set; }

    /// <summary>
    /// Configuration if each test position. Entries must not be 
    /// null but test positions can be disabled if needed.
    /// </summary>
    [NotNull, Required]
    public List<TestPositionConfiguration> TestPositions { get; set; } = [];

    /// <summary>
    /// HID event index of the barcode reader - as taken from the device path.
    /// </summary>
    public uint? BarcodeReader { get; set; }

    /// <summary>
    /// Set to use the MP2020 control using a CR2020.
    /// </summary>
    [NotNull, Required]
    public bool EnableMP2020Control { get; set; }

    /// <summary>
    /// Individual flags to set if DC component should be used.
    /// </summary>
    [NotNull, Required]
    public DCComponents DCComponents { get; set; }

    /// <summary>
    /// Report if any DC component is selected.
    /// </summary>
    [JsonIgnore]
    public bool EnableDCComponents => DCComponents != DCComponents.None;

    /// <summary>
    /// Individual flags to set if transformer components shoul be used.
    /// </summary>
    [NotNull, Required]
    public TransformerComponents TransformerComponents { get; set; }

    /// <summary>
    /// Report if any transformer component is selected.
    /// </summary>
    [JsonIgnore]
    public bool EnableTransformerComponents => TransformerComponents != TransformerComponents.None;

    /// <summary>
    /// Individual flags to set if some MT310s2 function will be used.
    /// </summary>
    [NotNull, Required]
    public MT310s2Functions MT310s2Functions { get; set; }

    /// <summary>
    /// Report if any MT310s2 function should be used.
    /// </summary>
    [JsonIgnore]
    public bool EnableMT310s2Functions => MT310s2Functions != MT310s2Functions.None;

    /// <summary>
    /// Individual flags to set if some Nbox PLC router will be used.
    /// </summary>
    [NotNull, Required]
    public NBoxRouterTypes NBoxRouterTypes { get; set; }

    /// <summary>
    /// Report if any NBox PLC router should be used.
    /// </summary>
    [JsonIgnore]
    public bool EnableNBoxRouterTypes => NBoxRouterTypes != NBoxRouterTypes.None;

    /// <summary>
    /// Set to use the Omega iBTHX temperature and humidity sensor.
    /// </summary>
    [NotNull, Required]
    public bool EnableOmegaiBTHX { get; set; }

    /// <summary>
    /// Set to use a COM5003 as external reference (absolute).
    /// </summary>
    [NotNull, Required]
    public bool EnableCOM5003 { get; set; }

    /// <summary>
    /// Set to use the IP watchdog for the whole test system.
    /// </summary>
    [NotNull, Required]
    public bool EnableIPWatchDog { get; set; }

    /// <summary>
    /// Set to use the DTS100 keyboard.
    /// </summary>
    [NotNull, Required]
    public bool EnableDTS100 { get; set; }

    /// <summary>
    /// Get the device path of the barcode reader.
    /// </summary>
    [JsonIgnore]
    public string? BarcodeReaderDevicePath => BarcodeReader.HasValue ? $"/dev/input/event{BarcodeReader}" : null;
}