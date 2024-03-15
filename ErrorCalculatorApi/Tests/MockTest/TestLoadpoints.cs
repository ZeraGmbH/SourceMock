using SourceApi.Model;

namespace MockTest.CurrentCalculationTest;

public static class TestLoadpoints
{
    public static TargetLoadpointPhase GetDcLoadpointPhase()
    {
        return new()
        {
            Current = new() { On = true, DcComponent = 10 },
            Voltage = new() { On = true, DcComponent = 210 }
        };
    }

    public static TargetLoadpointPhase GetAcLoadpointPhase()
    {
        return new()
        {
            Current = new() { On = true, AcComponent = new() { Angle = 0, Rms = 10, } },
            Voltage = new() { On = true, AcComponent = new() { Angle = 0, Rms = 210 } }
        };
    }

    public static TargetLoadpoint GetNullLoadpointPhase()
    {
        return new()
        { };
    }
}