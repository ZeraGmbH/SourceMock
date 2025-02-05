using ZERA.WebSam.Shared.Models.Source;

namespace MockTest.CurrentCalculationTest;

public static class TestLoadpoints
{
    public static TargetLoadpointPhase GetDcLoadpointPhase()
    {
        return new()
        {
            Current = new() { On = true, DcComponent = new(10) },
            Voltage = new() { On = true, DcComponent = new(210) }
        };
    }

    public static TargetLoadpointPhase GetAcLoadpointPhase()
    {
        return new()
        {
            Current = new() { On = true, AcComponent = new() { Angle = new(0), Rms = new(10), } },
            Voltage = new() { On = true, AcComponent = new() { Angle = new(0), Rms = new(210) } }
        };
    }

    public static TargetLoadpoint GetNullLoadpointPhase()
    {
        return new()
        { };
    }
}