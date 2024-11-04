using ZERA.WebSam.Shared.DomainSpecific;

namespace BurdenApiTests;

// Übergangsläsung im Rahmen von TDD: alle Klassen und Schnittstellen werden nachträglich in die Bibliothek wandern!

/// <summary>
/// A target pair of values.
/// </summary>
public class GoalValue(ApparentPower power, PowerFactor factor)
{
    /// <summary>
    /// Für die Serialisierung.
    /// </summary>
    public GoalValue() : this(new(0), new(1)) { }

    /// <summary>
    /// Apparent power in W.
    /// </summary>
    public ApparentPower ApparentPower { get; private set; } = power;

    /// <summary>
    /// Powerfactor between as cos of angle difference.
    /// </summary>
    public PowerFactor PowerFactor { get; private set; } = factor;

    /// <summary>
    /// Calcluate the deviation of a goal value.
    /// </summary>
    /// <param name="actual">Meaured value.</param>
    /// <param name="expected">Expected value.</param>
    /// <returns>Relative deviation.</returns>
    public static GoalDeviation operator /(GoalValue actual, GoalValue expected)
        => new(
            (actual.ApparentPower - expected.ApparentPower) / expected.ApparentPower,
            (actual.PowerFactor - expected.PowerFactor) / expected.PowerFactor);
}

/// <summary>
/// A relative target pair of values, all calculated
/// as (ACTUAL - EXPECTED) / EXPECTED.
/// </summary>
public class GoalDeviation(double power, double factor)
{
    /// <summary>
    /// Für die Serialisierung.
    /// </summary>
    public GoalDeviation() : this(0, 0) { }

    /// <summary>
    /// Deviation on apparent power, positive if the 
    /// measure value is too large.
    /// </summary>
    public double DeltaPower { get; private set; } = power;

    /// <summary>
    /// Deviation on power factor, positive if the 
    /// measure value is too large.
    /// </summary>
    public double DeltaFactor { get; private set; } = factor;
}

/// <summary>
/// Single pair of calibration values with coarse
/// and fine values. 
/// </summary>
public class CalibrationPair()
{
    /// <summary>
    /// Initialize a new pair.
    /// </summary>
    /// <param name="major">Major calibration between 0 and 127.</param>
    /// <param name="minor">Minor calibration between 0 and 127.</param>
    public CalibrationPair(byte major, byte minor) : this()
    {
        // Validate and remember.
        ArgumentOutOfRangeException.ThrowIfGreaterThan(major, 127, nameof(major));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minor, 127, nameof(minor));

        Major = major;
        Minor = minor;
    }

    /// <summary>
    /// Major calibration value between 0 and 127.
    /// </summary>
    public byte Major { get; private set; }

    /// <summary>
    /// Minor calibration value between 0 and 127.
    /// </summary>
    public byte Minor { get; private set; }

    /// <summary>
    /// Change the coarse calibration.
    /// </summary>
    /// <param name="delta">May increment or decrement by one or keep change.</param>
    /// <returns>New calibration pair if changes are applied or null if nothing changed.</returns>
    public CalibrationPair? ChangeMajor(int delta)
        => delta switch
        {
            0 => null,
            +1 => Major < 127 ? new CalibrationPair((byte)(Major + 1), Minor) : null,
            -1 => Major > 0 ? new CalibrationPair((byte)(Major - 1), Minor) : null,
            _ => throw new ArgumentOutOfRangeException(nameof(delta), "delta must be an integral number between -1 and 1 (both inclusive)"),
        };

    /// <summary>
    /// Change the fine calibration.
    /// </summary>
    /// <param name="delta">May increment or decrement by one or keep change.</param>
    /// <returns>New calibration pair if changes are applied or null if nothing changed.</returns>
    public CalibrationPair? ChangeMinor(int delta)
        => delta switch
        {
            0 => null,
            +1 => Minor < 127 ? new CalibrationPair(Major, (byte)(Minor + 1)) : null,
            -1 => Minor > 0 ? new CalibrationPair(Major, (byte)(Minor - 1)) : null,
            _ => throw new ArgumentOutOfRangeException(nameof(delta), "delta must be an integral number between -1 and 1 (both inclusive)"),
        };
}

/// <summary>
/// Full calibration information.
/// </summary>
/// <param name="resistive">Resistive calibration.</param>
/// <param name="inductive">Inductive calibration.</param>
public class Calibration(CalibrationPair resistive, CalibrationPair inductive)
{
    /// <summary>
    /// For serialisation only.
    /// </summary>
    public Calibration() : this(new(), new()) { }

    /// <summary>
    /// Calibration on the resistive load.
    /// </summary>
    public CalibrationPair Resistive { get; private set; } = resistive;

    /// <summary>
    /// Calibration on the inductive load.
    /// </summary>
    public CalibrationPair Inductive { get; private set; } = inductive;
}

[TestFixture]
public class AlgorithmTests
{
    [TestCase(0, 0)]
    [TestCase(0, 127)]
    [TestCase(127, 0)]
    [TestCase(127, 127)]
    public void Calibration_Pair_Can_Be_Created(byte major, byte minor)
    {
        var pair = new CalibrationPair(major, minor);

        Assert.Multiple(() =>
        {
            Assert.That(pair.Major, Is.EqualTo(major));
            Assert.That(pair.Minor, Is.EqualTo(minor));
        });
    }

    [TestCase(128, 0)]
    [TestCase(0, 128)]
    [TestCase(128, 128)]
    [TestCase(255, 0)]
    [TestCase(0, 255)]
    [TestCase(255, 255)]
    public void Calibration_Pair_Can_Not_Be_Created(byte major, byte minor)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CalibrationPair(major, minor));
    }

    [TestCase(0, 0, 0, false)]
    [TestCase(0, 0, -1, false)]
    [TestCase(0, 0, 1, true)]
    [TestCase(127, 0, 0, false)]
    [TestCase(127, 0, -1, true)]
    [TestCase(127, 0, 1, false)]
    public void Calibration_Pair_Major_Can_Be_Changed(byte major, byte minor, int delta, bool change)
    {
        var pair = new CalibrationPair(major, minor);
        var newPair = pair.ChangeMajor(delta);

        if (change)
            Assert.Multiple(() =>
            {
                Assert.That(newPair, Is.Not.Null);
                Assert.That(newPair, Is.Not.SameAs(pair));
                Assert.That(newPair!.Major, Is.EqualTo(major + delta));
                Assert.That(newPair.Minor, Is.EqualTo(minor));
            });
        else
            Assert.That(newPair, Is.Null);
    }

    [TestCase(64, 64, -2)]
    [TestCase(64, 64, 2)]
    public void Calibration_Pair_Major_Can_Not_Be_Changed(byte major, byte minor, int delta)
    {
        var pair = new CalibrationPair(major, minor);

        Assert.Throws<ArgumentOutOfRangeException>(() => pair.ChangeMajor(delta));

    }

    [TestCase(0, 0, 0, false)]
    [TestCase(0, 0, -1, false)]
    [TestCase(0, 0, 1, true)]
    [TestCase(0, 127, 0, false)]
    [TestCase(0, 127, -1, true)]
    [TestCase(0, 127, 1, false)]
    public void Calibration_Pair_Minor_Can_Be_Changed(byte major, byte minor, int delta, bool change)
    {
        var pair = new CalibrationPair(major, minor);
        var newPair = pair.ChangeMinor(delta);

        if (change)
            Assert.Multiple(() =>
            {
                Assert.That(newPair, Is.Not.Null);
                Assert.That(newPair, Is.Not.SameAs(pair));
                Assert.That(newPair!.Major, Is.EqualTo(major));
                Assert.That(newPair.Minor, Is.EqualTo(minor + delta));
            });
        else
            Assert.That(newPair, Is.Null);
    }

    [TestCase(64, 64, -2)]
    [TestCase(64, 64, 2)]
    public void Calibration_Pair_Minor_Can_Not_Be_Changed(byte major, byte minor, int delta)
    {
        var pair = new CalibrationPair(major, minor);

        Assert.Throws<ArgumentOutOfRangeException>(() => pair.ChangeMinor(delta));

    }
}