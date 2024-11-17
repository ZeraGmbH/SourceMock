using BurdenApi.Actions.Algorithms;
using BurdenApi.Actions.Device;
using BurdenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models.Logging;

namespace BurdenApiTests
{
    [TestFixture]
    public class AlgorithmTests
    {
        private ICalibrator Calibrator = null!;

        private ServiceProvider Services = null!;

        private CalibrationHardwareMock Hardware = null!;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();

            services.AddTransient<IInterfaceLogger, NoopInterfaceLogger>();

            services.AddTransient<ICalibrator, Calibrator>();

            services.AddKeyedTransient<ICalibrationAlgorithm, SingleStepCalibrator>(CalibrationAlgorithms.Default);
            services.AddKeyedTransient<ICalibrationAlgorithm, SingleStepCalibrator>(CalibrationAlgorithms.SingleStep);
            services.AddKeyedTransient<ICalibrationAlgorithm, IntervalCalibrator>(CalibrationAlgorithms.Interval);

            services.AddSingleton<ICalibrationHardware, CalibrationHardwareMock>();

            Services = services.BuildServiceProvider();

            Calibrator = Services.GetRequiredService<ICalibrator>();
            Hardware = (CalibrationHardwareMock)Services.GetRequiredService<ICalibrationHardware>();
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

        [TestCase(0, 0, -1, false)]
        [TestCase(0, 0, +1, true)]
        [TestCase(127, 0, -1, true)]
        [TestCase(127, 0, +1, false)]
        public void Calibration_Pair_Major_Can_Be_Changed(byte major, byte minor, int increment, bool change)
        {
            var pair = new CalibrationPair(major, minor);
            var newPair = pair.ChangeCoarse(increment);

            if (change)
                Assert.Multiple(() =>
                {
                    Assert.That(newPair, Is.Not.Null);
                    Assert.That(newPair, Is.Not.SameAs(pair));
                    Assert.That(newPair!.Coarse, Is.EqualTo(major + increment));
                    Assert.That(newPair.Fine, Is.EqualTo(minor));
                });
            else
                Assert.That(newPair, Is.Null);
        }

        [TestCase(0, 0, -1, false)]
        [TestCase(0, 0, +1, true)]
        [TestCase(0, 127, -1, true)]
        [TestCase(0, 127, +1, false)]
        public void Calibration_Pair_Minor_Can_Be_Changed(byte major, byte minor, int increment, bool change)
        {
            var pair = new CalibrationPair(major, minor);
            var newPair = pair.ChangeFine(increment);

            if (change)
                Assert.Multiple(() =>
                {
                    Assert.That(newPair, Is.Not.Null);
                    Assert.That(newPair, Is.Not.SameAs(pair));
                    Assert.That(newPair!.Coarse, Is.EqualTo(major));
                    Assert.That(newPair.Fine, Is.EqualTo(minor + increment));
                });
            else
                Assert.That(newPair, Is.Null);
        }

        [TestCase(0, 0, 0, 0, 0, 1)]
        [TestCase(127, 127, 127, 127, 24.1813, 0)]
        [TestCase(64, 64, 64, 64, 12.1858, 0.496)]
        [TestCase(64, 32, 32, 64, 12.0746, 0.7434)]
        [TestCase(64, 0, 64, 0, 12.082, 0.5003)]
        [TestCase(64, 0, 64, 32, 12.0825, 0.4982)]
        [TestCase(64, 32, 64, 0, 12.1334, 0.5003)]
        [TestCase(0, 0, 110, 20, 0.2059, 0.1484)]
        [TestCase(127, 127, 0, 0, 23.9419, 0.99)]
        [TestCase(0, 0, 127, 127, 0.2394, 0.0099)]
        public async Task Mock_Hardware_Will_Produce_Values_Async(byte rMajor, byte rMinor, byte iMajor, byte iMinor, double apparentPower, double powerFactor)
        {
            var hardware = Services.GetRequiredService<ICalibrationHardware>();

            await hardware.PrepareAsync("200", 1, new(50), false, new(10));

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

        [TestCase(null)]
        [TestCase(CalibrationAlgorithms.Default)]
        [TestCase(CalibrationAlgorithms.SingleStep)]
        public async Task Can_Run_Calibration_Async(CalibrationAlgorithms? algorithm)
        {
            Hardware.AddCalibration("IEC50", "200", "50;0.75", new(new(64, 32), new(32, 64)));

            if (algorithm.HasValue)
                await Calibrator.RunAsync(new() { Burden = "IEC50", Range = "200", Step = "50;0.75", Algorithm = algorithm.Value }, CancellationToken.None);
            else
                await Calibrator.RunAsync(new() { Burden = "IEC50", Range = "200", Step = "50;0.75" }, CancellationToken.None);

            var step = Calibrator.LastStep;

            Assert.That(step, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(Calibrator.Steps, Has.Length.EqualTo(86));

                Assert.That(step!.Calibration.Resistive.Coarse, Is.EqualTo(53));
                Assert.That(step.Calibration.Resistive.Fine, Is.EqualTo(21));
                Assert.That(step.Calibration.Inductive.Coarse, Is.EqualTo(31));
                Assert.That(step.Calibration.Inductive.Fine, Is.EqualTo(94));

                Assert.That((double)step.Values.ApparentPower, Is.EqualTo(50).Within(0.1));
                Assert.That((double)step.Values.PowerFactor, Is.EqualTo(0.75).Within(0.001));

                Assert.That(Math.Abs(step.Deviation.DeltaPower), Is.LessThan(0.1));
                Assert.That(Math.Abs(step.Deviation.DeltaFactor), Is.LessThan(0.1));
            });
        }

        [Test]
        public async Task Can_Run_Interval_Calibration_Async()
        {
            Hardware.AddCalibration("IEC50", "200", "50;0.75", new(new(64, 32), new(32, 64)));

            await Calibrator.RunAsync(new() { Burden = "IEC50", Range = "200", Step = "50;0.75", Algorithm = CalibrationAlgorithms.Interval }, CancellationToken.None);

            var step = Calibrator.LastStep;

            Assert.That(step, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(Calibrator.Steps, Has.Length.EqualTo(32));

                Assert.That(step!.Calibration.Resistive.Coarse, Is.EqualTo(53));
                Assert.That(step.Calibration.Resistive.Fine, Is.EqualTo(59));
                Assert.That(step.Calibration.Inductive.Coarse, Is.EqualTo(31));
                Assert.That(step.Calibration.Inductive.Fine, Is.EqualTo(95));

                //[algorithm not yet complete] Assert.That((double)step.Values.ApparentPower, Is.EqualTo(50).Within(0.1));
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
        [TestCase(50, 0.9706, 64, 32)]
        [TestCase(50, 0.1, 64, 32)]
        [TestCase(50, 0.8, 20, 10)]
        [TestCase(50, 0.8, 0, 0)]
        [TestCase(50, 0.8, 127, 127)]
        public async Task Can_Run_Calibrations_Async(double power, double factor, byte big, byte small)
        {
            var step = $"{power};{factor}";

            Hardware.AddCalibration("IEC50", "200", step, new(new(big, small), new(small, big)));

            await Calibrator.RunAsync(new() { Burden = "IEC50", Range = "200", Step = step }, CancellationToken.None);

            var lastStep = Calibrator.LastStep;

            Assert.That(lastStep, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That((double)lastStep.Values.ApparentPower, Is.EqualTo(power).Within(0.1));
                Assert.That((double)lastStep.Values.PowerFactor, Is.EqualTo(factor).Within(0.01));

                Assert.That(Math.Abs(lastStep.Deviation.DeltaPower), Is.LessThan(0.015));
                Assert.That(Math.Abs(lastStep.Deviation.DeltaFactor), Is.LessThan(0.015));
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
        public async Task Can_Run_Full_Calibrations_Async(double power, double factor, byte big, byte small)
        {
            var step = $"{power};{factor}";

            Hardware.AddCalibration("IEC50", "200", step, new(new(big, small), new(small, big)));

            var result = await Calibrator.CalibrateStepAsync(new() { Burden = "IEC50", Range = "200", Step = step }, CancellationToken.None);

            Assert.That(result, Has.Length.EqualTo(3));

            foreach (var oneStep in result)
            {
                Assert.That(oneStep, Is.Not.Null);

                Assert.Multiple(() =>
                {
                    Assert.That((double)oneStep.Values.ApparentPower, Is.EqualTo(power * oneStep.Factor * oneStep.Factor).Within(0.1));
                    Assert.That((double)oneStep.Values.PowerFactor, Is.EqualTo(factor).Within(0.01));
                    Assert.That(Math.Abs(oneStep.Deviation.DeltaFactor), Is.LessThan(0.015));
                });
            }
        }
    }
}