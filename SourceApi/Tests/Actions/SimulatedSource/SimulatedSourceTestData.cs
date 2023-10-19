using WebSamDeviceApis.Model;

namespace WebSamDeviceApis.Tests.Actions.Source
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

        public static SourceCapabilities GetSourceCapabilitiesForNumberOfPhases(int numberOfPhases)
        {
            switch (numberOfPhases)
            {
                case 1:
                    return DefaultSinglePhaseSourceCapabilities;
                case 2:
                    return DefaultTwoPhaseSourceCapabilities;
                case 3:
                    return DefaultThreePhaseSourceCapabilities;
                default:
                    throw new NotImplementedException($"No Source Capablilities with {numberOfPhases} Phases found.");
            }
        }

        public static SourceCapabilities DefaultThreePhaseSourceCapabilities
        {
            get
            {
                return new()
                {
                    Phases = new() {
                        new() {
                            Voltage = new(10, 300, 0.01),
                            Current = new(0, 60, 0.01)
                        },
                        new() {
                            Voltage = new(10, 300, 0.01),
                            Current = new(0, 60, 0.01)
                        },
                        new() {
                            Voltage = new(10, 300, 0.01),
                            Current = new(0, 60, 0.01)
                        }
                    },
                    FrequencyRanges = new() {
                        new(40, 60, 0.1, FrequencyMode.SYNTHETIC)
                    }
                };
            }
        }

        public static SourceCapabilities DefaultTwoPhaseSourceCapabilities
        {
            get
            {
                return new()
                {
                    Phases = new() {
                        new() {
                            Voltage = new(10, 300, 0.01),
                            Current = new(0, 60, 0.01)
                        },
                        new() {
                            Voltage = new(10, 300, 0.01),
                            Current = new(0, 60, 0.01)
                        }
                    },
                    FrequencyRanges = new() {
                        new(40, 60, 0.1, FrequencyMode.SYNTHETIC)
                    }
                };
            }
        }

        public static SourceCapabilities DefaultSinglePhaseSourceCapabilities
        {
            get
            {
                return new()
                {
                    Phases = new() {
                        new() {
                            Voltage = new(10, 300, 0.01),
                            Current = new(0, 60, 0.01)
                        }
                    },
                    FrequencyRanges = new() {
                        new(40, 60, 0.1, FrequencyMode.SYNTHETIC)
                    }
                };
            }
        }
    }
}