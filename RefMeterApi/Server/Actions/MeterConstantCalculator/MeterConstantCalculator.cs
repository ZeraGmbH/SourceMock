using Microsoft.Extensions.Logging;
using RefMeterApi.Models;
using SharedLibrary.DomainSpecific;

namespace RefMeterApi.Actions.MeterConstantCalculator;

/// <summary>
/// Calculate meter constants.
/// </summary>
public class MeterConstantCalculator(ILogger<MeterConstantCalculator> logger) : IMeterConstantCalculator
{
    private static readonly Dictionary<ReferenceMeters, Func<double, MeasurementModes, double, double, double>> _calculators = new() {
        {ReferenceMeters.COM3000, Com30AndEpz303},
        {ReferenceMeters.COM3003, Com30AndEpz303},
        {ReferenceMeters.COM3003x1x2, Com30AndEpz303},
        {ReferenceMeters.COM3003xDC, Com30AndEpz303},
        {ReferenceMeters.COM3003xDCx2x1, Com30AndEpz303},
        {ReferenceMeters.EPZ303x1, Com30AndEpz303},
        {ReferenceMeters.EPZ303x10, Com30AndEpz303},
        {ReferenceMeters.EPZ303x10x1, Com30AndEpz303},
        {ReferenceMeters.EPZ303x5, Com30AndEpz303},
        {ReferenceMeters.EPZ303x8, Com30AndEpz303},
        {ReferenceMeters.EPZ303x8x1, Com30AndEpz303},
        {ReferenceMeters.EPZ303x9, Com30AndEpz303},
     };

    private static readonly double _sqrt3 = Math.Sqrt(3d);

    /// <inheritdoc/>
    public MeterConstant GetMeterConstant(ReferenceMeters meter, double frequency, MeasurementModes mode, double voltageRange, double currentRange)
    {
        if (_calculators.TryGetValue(meter, out var algorithm)) return new(algorithm(frequency, mode, voltageRange, currentRange));

        logger.LogError("No meter constant algorithm for {Meter}", meter);

        throw new NotSupportedException($"no meter constant algorithm for {meter}");
    }

    private static double Com30AndEpz303(double frequency, MeasurementModes mode, double voltageRange, double currentRange)
    {
        /* Bias for four wire measurements. */
        var meterConstant = 1000d * 3600d * frequency / (3d * voltageRange * currentRange);

        /* Check measuring mode. */
        return mode switch
        {
            MeasurementModes.ThreeWireActivePower or
            MeasurementModes.ThreeWireApparentPower or
            MeasurementModes.ThreeWireReactivePower or
            MeasurementModes.ThreeWireReactivePowerCrossConnectedA or
            MeasurementModes.ThreeWireReactivePowerCrossConnectedB
                => meterConstant / _sqrt3,

            MeasurementModes.TwoWireActivePower or
            MeasurementModes.TwoWireApparentPower or
            MeasurementModes.TwoWireReactivePower
                => 3d * meterConstant,

            _ => meterConstant,
        };
    }
}