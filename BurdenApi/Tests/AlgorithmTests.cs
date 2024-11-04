using BurdenApi.Actions;
using BurdenApi.Models;

namespace BurdenApiTests
{
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
                Assert.That(pair.Coarse, Is.EqualTo(major));
                Assert.That(pair.Fine, Is.EqualTo(minor));
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
            var newPair = pair.ChangeCoarse(delta);

            if (change)
                Assert.Multiple(() =>
                {
                    Assert.That(newPair, Is.Not.Null);
                    Assert.That(newPair, Is.Not.SameAs(pair));
                    Assert.That(newPair!.Coarse, Is.EqualTo(major + delta));
                    Assert.That(newPair.Fine, Is.EqualTo(minor));
                });
            else
                Assert.That(newPair, Is.Null);
        }

        [TestCase(64, 64, -2)]
        [TestCase(64, 64, 2)]
        public void Calibration_Pair_Major_Can_Not_Be_Changed(byte major, byte minor, int delta)
        {
            var pair = new CalibrationPair(major, minor);

            Assert.Throws<ArgumentOutOfRangeException>(() => pair.ChangeCoarse(delta));
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
            var newPair = pair.ChangeFine(delta);

            if (change)
                Assert.Multiple(() =>
                {
                    Assert.That(newPair, Is.Not.Null);
                    Assert.That(newPair, Is.Not.SameAs(pair));
                    Assert.That(newPair!.Coarse, Is.EqualTo(major));
                    Assert.That(newPair.Fine, Is.EqualTo(minor + delta));
                });
            else
                Assert.That(newPair, Is.Null);
        }

        [TestCase(64, 64, -2)]
        [TestCase(64, 64, 2)]
        public void Calibration_Pair_Minor_Can_Not_Be_Changed(byte major, byte minor, int delta)
        {
            var pair = new CalibrationPair(major, minor);

            Assert.Throws<ArgumentOutOfRangeException>(() => pair.ChangeFine(delta));
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(127, 127, 127, 127, 133.195, 1)]
        [TestCase(64, 64, 64, 64, 67.1219, 0.5039)]
        [TestCase(64, 32, 32, 64, 66.86178, 0.2539)]
        [TestCase(64, 0, 64, 0, 66.6016, 0.5)]
        [TestCase(64, 0, 64, 32, 66.6016, 0.5019)]
        [TestCase(64, 32, 64, 0, 66.8617, 0.5)]
        [TestCase(0, 0, 110, 20, 0, 0.8606)]
        [TestCase(127, 127, 0, 0, 133.195, 0)]
        [TestCase(0, 0, 127, 127, 0, 1)]
        public async Task Mock_Hardware_Will_Produce_Values_Async(byte rMajor, byte rMinor, byte iMajor, byte iMinor, double apparentPower, double powerFactor)
        {
            var hardware = new CalibrationHardware();

            var values = await hardware.MeasureAsync(new(new(rMajor, rMinor), new(iMajor, iMinor)));

            Assert.Multiple(() =>
            {
                Assert.That((double)values.ApparentPower, Is.EqualTo(apparentPower).Within(0.001));
                Assert.That((double)values.PowerFactor, Is.EqualTo(powerFactor).Within(0.0001));
            });
        }

        [TestCase(0, 0, 100, 0.5, -1, -1)]
        [TestCase(100, 0.5, 100, 0.5, 0, 0)]
        [TestCase(120, 0.6, 100, 0.5, 0.2, 0.2)]
        [TestCase(80, 0.4, 100, 0.5, -0.2, -0.2)]
        public void Can_Calculate_Relative_Goals(double pow1, double fac1, double pow2, double fac2, double deltaPower, double deltaFactor)
        {
            var current = new GoalValue(new(pow1), new(fac1));
            var goal = new GoalValue(new(pow2), new(fac2));

            var delta = current / goal;

            Assert.Multiple(() =>
            {
                Assert.That(delta.DeltaPower, Is.EqualTo(deltaPower).Within(0.01));
                Assert.That(delta.DeltaFactor, Is.EqualTo(deltaFactor).Within(0.01));
            });
        }

        [Test]
        public async Task Can_Run_Calibration_Async()
        {
            var hardware = new CalibrationHardware();
            var calibrator = new Calibrator();

            await calibrator.RunAsync(
                new(new(50), new(0.75)),
                new(new(64, 32), new(32, 64)),
                hardware
            );

            var step = calibrator.LastStep;

            Assert.That(step, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(calibrator.Steps, Has.Length.EqualTo(168));

                Assert.That(step.Calibration.Resistive.Coarse, Is.EqualTo(48));
                Assert.That(step.Calibration.Resistive.Fine, Is.EqualTo(6));
                Assert.That(step.Calibration.Inductive.Coarse, Is.EqualTo(95));
                Assert.That(step.Calibration.Inductive.Fine, Is.EqualTo(127));

                Assert.That((double)step.Values.ApparentPower, Is.EqualTo(50).Within(0.1));
                Assert.That((double)step.Values.PowerFactor, Is.EqualTo(0.75).Within(0.001));

                Assert.That(Math.Abs(step.Deviation.DeltaPower), Is.LessThan(0.1));
                Assert.That(Math.Abs(step.Deviation.DeltaFactor), Is.LessThan(0.1));
            });
        }

        [TestCase(50, 0.75)]
        [TestCase(60, 0.75)]
        [TestCase(40, 0.75)]
        [TestCase(70, 0.75)]
        [TestCase(35, 0.75)]
        [TestCase(50, 0.8)]
        [TestCase(50, 0.9)]
        [TestCase(50, 1)]
        [TestCase(50, 0.1)]
        public async Task Can_Run_Calibrations_Async(double power, double factor)
        {
            var hardware = new CalibrationHardware();
            var calibrator = new Calibrator();

            await calibrator.RunAsync(
                new(new(power), new(factor)),
                new(new(64, 32), new(32, 64)),
                hardware
            );

            var step = calibrator.LastStep;

            Assert.That(step, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That((double)step.Values.ApparentPower, Is.EqualTo(power).Within(0.1));
                Assert.That((double)step.Values.PowerFactor, Is.EqualTo(factor).Within(0.001));

                Assert.That(Math.Abs(step.Deviation.DeltaPower), Is.LessThan(0.1));
                Assert.That(Math.Abs(step.Deviation.DeltaFactor), Is.LessThan(0.1));
            });
        }
    }
}