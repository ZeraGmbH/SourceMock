using SourceMock.Model;

namespace SourceMock.Tests.Actions.Source
{
    public static class SimulatedSourceTestData
    {
        public static IEnumerable<TestCaseData> ValidLoadpointsWithOneOrThreePhases
        {
            get
            {
                yield return new TestCaseData(LoadpointValidator.LoadpointValidatorTestData.Loadpoint001_3AC_valid);
                yield return new TestCaseData(LoadpointValidator.LoadpointValidatorTestData.Loadpoint003_1AC_valid);
            }
        }

        public static SourceCapabilities DefaultThreePhaseSourceCapabilities
        {
            get
            {
                return new()
                {
                    NumberOfPhases = 3,
                    VoltageRanges = new() {
                        new(0, 300)
                    },
                    CurrentRanges = new() {
                        new(0, 60)
                    },
                    MaxHarmonic = 20
                };
            }
        }

        public static SourceCapabilities DefaultSinglePhaseSourceCapabilities
        {
            get
            {
                return new()
                {
                    NumberOfPhases = 1,
                    VoltageRanges = new() {
                        new(0, 300)
                    },
                    CurrentRanges = new() {
                        new(0, 60)
                    },
                    MaxHarmonic = 20
                };
            }
        }
    }
}