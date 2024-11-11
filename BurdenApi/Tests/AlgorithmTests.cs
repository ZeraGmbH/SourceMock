using BurdenApi.Actions;
using BurdenApi.Actions.Device;
using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BurdenApiTests
{
    [TestFixture]
    public class AlgorithmTests
    {
        private ICalibrator Calibrator = null!;

        private ServiceProvider Services = null!;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddTransient<ICalibrator, Calibrator>();
            services.AddTransient<ICalibrationHardware, CalibrationHardwareMock>();

            Services = services.BuildServiceProvider();

            Calibrator = Services.GetRequiredService<ICalibrator>();
        }

        [TearDown]
        public void Teardown()
        {
            Services.Dispose();
        }

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

        [TestCase(0, 0, false, false)]
        [TestCase(0, 0, true, true)]
        [TestCase(127, 0, false, true)]
        [TestCase(127, 0, true, false)]
        public void Calibration_Pair_Major_Can_Be_Changed(byte major, byte minor, bool increment, bool change)
        {
            var pair = new CalibrationPair(major, minor);
            var newPair = pair.ChangeCoarse(increment);

            if (change)
                Assert.Multiple(() =>
                {
                    Assert.That(newPair, Is.Not.Null);
                    Assert.That(newPair, Is.Not.SameAs(pair));
                    Assert.That(newPair!.Coarse, Is.EqualTo(major + (increment ? +1 : -1)));
                    Assert.That(newPair.Fine, Is.EqualTo(minor));
                });
            else
                Assert.That(newPair, Is.Null);
        }

        [TestCase(0, 0, false, false)]
        [TestCase(0, 0, true, true)]
        [TestCase(0, 127, false, true)]
        [TestCase(0, 127, true, false)]
        public void Calibration_Pair_Minor_Can_Be_Changed(byte major, byte minor, bool increment, bool change)
        {
            var pair = new CalibrationPair(major, minor);
            var newPair = pair.ChangeFine(increment);

            if (change)
                Assert.Multiple(() =>
                {
                    Assert.That(newPair, Is.Not.Null);
                    Assert.That(newPair, Is.Not.SameAs(pair));
                    Assert.That(newPair!.Coarse, Is.EqualTo(major));
                    Assert.That(newPair.Fine, Is.EqualTo(minor + (increment ? +1 : -1)));
                });
            else
                Assert.That(newPair, Is.Null);
        }

        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(127, 127, 127, 127, 133.3283, 1.0009)]
        [TestCase(64, 64, 64, 64, 67.189, 0.5043)]
        [TestCase(64, 32, 32, 64, 66.8956, 0.2543)]
        [TestCase(64, 0, 64, 0, 66.6682, 0.5004)]
        [TestCase(64, 0, 64, 32, 66.6684, 0.5024)]
        [TestCase(64, 32, 64, 0, 66.9283, 0.5004)]
        [TestCase(0, 0, 110, 20, 0.1146, 0.8606)]
        [TestCase(127, 127, 0, 0, 133.1951, 0.001)]
        [TestCase(0, 0, 127, 127, 0.1331, 1)]
        public async Task Mock_Hardware_Will_Produce_Values_Async(byte rMajor, byte rMinor, byte iMajor, byte iMinor, double apparentPower, double powerFactor)
        {
            var hardware = Services.GetRequiredService<ICalibrationHardware>();

            var values = await hardware.MeasureAsync(new(new(rMajor, rMinor), new(iMajor, iMinor)));

            Assert.Multiple(() =>
            {
                Assert.That((double)values.ApparentPower, Is.EqualTo(apparentPower).Within(0.0001));
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
            await Calibrator.RunAsync(new() { Burden = "IEC50", Goal = new(new(50), new(0.75)), InitialCalibration = new(new(64, 32), new(32, 64)) });

            var step = Calibrator.LastStep;

            Assert.That(step, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(Calibrator.Steps, Has.Length.EqualTo(169));

                Assert.That(step.Calibration.Resistive.Coarse, Is.EqualTo(48));
                Assert.That(step.Calibration.Resistive.Fine, Is.EqualTo(0));
                Assert.That(step.Calibration.Inductive.Coarse, Is.EqualTo(95));
                Assert.That(step.Calibration.Inductive.Fine, Is.EqualTo(122));

                Assert.That((double)step.Values.ApparentPower, Is.EqualTo(50).Within(0.1));
                Assert.That((double)step.Values.PowerFactor, Is.EqualTo(0.75).Within(0.001));

                Assert.That(Math.Abs(step.Deviation.DeltaPower), Is.LessThan(0.1));
                Assert.That(Math.Abs(step.Deviation.DeltaFactor), Is.LessThan(0.1));
            });
        }

        [TestCase(50, 0.75, 64, 32)]
        [TestCase(60, 0.75, 64, 32)]
        [TestCase(40, 0.75, 64, 32)]
        [TestCase(70, 0.75, 64, 32)]
        [TestCase(35, 0.75, 64, 32)]
        [TestCase(50, 0.8, 64, 32)]
        [TestCase(50, 0.9, 64, 32)]
        [TestCase(50, 1, 64, 32)]
        [TestCase(50, 0.1, 64, 32)]
        [TestCase(50, 0.8, 20, 10)]
        [TestCase(50, 0.8, 0, 0)]
        [TestCase(50, 0.8, 127, 127)]
        public async Task Can_Run_Calibrations_Async(double power, double factor, byte big, byte small)
        {
            await Calibrator.RunAsync(new() { Burden = "IEC50", Goal = new(new(power), new(factor)), InitialCalibration = new(new(big, small), new(small, big)) });

            var step = Calibrator.LastStep;

            Assert.That(step, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That((double)step.Values.ApparentPower, Is.EqualTo(power).Within(0.1));
                Assert.That((double)step.Values.PowerFactor, Is.EqualTo(factor).Within(0.01));

                Assert.That(Math.Abs(step.Deviation.DeltaPower), Is.LessThan(0.015));
                Assert.That(Math.Abs(step.Deviation.DeltaFactor), Is.LessThan(0.015));
            });
        }
    }
}