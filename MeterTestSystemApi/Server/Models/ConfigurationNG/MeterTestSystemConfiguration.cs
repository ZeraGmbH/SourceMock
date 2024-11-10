using MeterTestSystemApi.Models.ConfigurationNG.Dosage;
using MeterTestSystemApi.Models.ConfigurationNG.MeterTestSystem;
using MeterTestSystemApi.Models.ConfigurationNG.ReferenceMeter;
using MeterTestSystemApi.Models.ConfigurationNG.Source;
using SourceApi.Model.Configuration;

namespace MeterTestSystemApi.Models.ConfigurationNG;

internal class AmplifiersAndReferenceMeterConfiguration { }

internal class MeterTestSystemConfiguration
{
    /// <summary>
    /// Overall configuration if the meter test system. The may be 
    /// need to configure the amplifiers and reference meter to be
    /// installed.
    /// </summary>
    internal MeterTestSystemProviderConfiguration MeterTestSystem { get; set; } = null!;

    /// <summary>
    /// Optional source.
    /// </summary>
    internal SourceProviderConfiguration? Source { get; set; }

    /// <summary>
    /// Optional dosage algorithm.
    /// </summary>
    internal DosageProviderConfiguration? Dosage { get; set; }

    /// <summary>
    /// Optional reference meter.
    /// </summary>
    internal ReferenceMeterProviderConfiguration? ReferenceMeter { get; set; }

    /// <summary>
    /// List of test positions, may be empty.
    /// </summary>
    internal List<TestPositionConfiguration> TestPositions { get; set; } = [];

    /// <summary>
    /// List of one or more custom serial ports to connect to.
    /// </summary>
    internal List<SerialPortConfiguration> SerialPorts { get; set; } = [];

    /// <summary>
    /// Optional burden.
    /// </summary>
    internal BurdenConfiguration? Burden { get; set; }

    /// <summary>
    /// Optional barcode reader.
    /// </summary>
    internal BarCodeReaderConfiguration? BarCodeReader { get; set; }
}