using MockDevices.ErrorCalculator;

namespace MockDeviceTests.ErrorCalculator;

public class CurrentCalculationTest
{

    [Test]
    public void Calculates_Dc_Current()
    {
        double current = 10;
        double voltage = 210;
        var expectedPower = current * voltage;

        var dcPhase = TestLoadpoints.GetDcLoadpointPhase();
        var actualPower = CurrentCalculation.CalculateDcPower(dcPhase);

        Assert.That((double)actualPower, Is.EqualTo(expectedPower));
    }

    [Test]
    public void Calculates_Ac_Current()
    {
        double current = 10;
        double voltage = 210;
        var expectedPower = current * voltage;  // ignore cos, since both angles are 0

        var dcPhase = TestLoadpoints.GetAcLoadpointPhase();
        var actualPower = CurrentCalculation.CalculateAcPower(dcPhase);

        Assert.That((double)actualPower, Is.EqualTo(expectedPower));
    }

    [Test]
    public void Not_Calculates_Ac_Current()
    {
        var expectedPower = 0;

        var acPhase = TestLoadpoints.GetAcLoadpointPhase();
        var actualPower = CurrentCalculation.CalculateDcPower(acPhase);

        Assert.That((double)actualPower, Is.EqualTo(expectedPower));
    }

    [Test]
    public void Not_Calculates_Dc_Current()
    {
        var expectedPower = 0;

        var dcPhase = TestLoadpoints.GetDcLoadpointPhase();
        var actualPower = CurrentCalculation.CalculateAcPower(dcPhase);

        Assert.That((double)actualPower, Is.EqualTo(expectedPower));
    }
}