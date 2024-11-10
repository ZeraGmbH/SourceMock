using MeterTestSystemApi.Models.ConfigurationNG.Dosage;
using MeterTestSystemApi.Models.ConfigurationNG.MeterTestSystem;
using MeterTestSystemApi.Models.ConfigurationNG.ReferenceMeter;
using MeterTestSystemApi.Models.ConfigurationNG.Source;
using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG;

/// <summary>
/// 
/// </summary>
public class MeterTestSystemConfiguration
{
    /// <summary>
    /// Overall configuration if the meter test system. The may be 
    /// need to configure the amplifiers and reference meter to be
    /// installed.
    /// </summary>
    public MeterTestSystemProviderConfiguration MeterTestSystem { get; set; } = null!;

    /// <summary>
    /// Optional source.
    /// </summary>
    public SourceProviderConfiguration? Source { get; set; }

    /// <summary>
    /// Optional dosage algorithm.
    /// </summary>
    public DosageProviderConfiguration? Dosage { get; set; }

    /// <summary>
    /// Optional reference meter.
    /// </summary>
    public ReferenceMeterProviderConfiguration? ReferenceMeter { get; set; }

    /// <summary>
    /// List of test positions, may be empty.
    /// </summary>
    public List<TestPositionConfiguration> TestPositions { get; set; } = [];

    /// <summary>
    /// List of one or more custom serial ports to connect to.
    /// </summary>
    public List<SerialPortConfiguration> SerialPorts { get; set; } = [];

    /// <summary>
    /// Optional burden.
    /// </summary>
    public BurdenConfiguration? Burden { get; set; }

    /// <summary>
    /// Optional barcode reader.
    /// </summary>
    public BarCodeReaderConfiguration? BarCodeReader { get; set; }
}