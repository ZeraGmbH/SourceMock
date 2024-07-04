using SourceApi.Model;

namespace SourceApi.Tests.Actions.Source
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
                            AcVoltage = new(new(10), new(300), new(0.01)),
                            AcCurrent = new(new(0), new(60), new(0.01))
                        },
                        new() {
                            AcVoltage = new(new(10), new(300), new(0.01)),
                            AcCurrent = new(new(0), new(60), new(0.01))
                        },
                        new() {
                            AcVoltage = new(new(10), new(300), new(0.01)),
                            AcCurrent = new(new(0), new(60), new(0.01))
                        }
                    },
                    FrequencyRanges = new() {
                        new(new(40), new(60), new(0.1), FrequencyMode.SYNTHETIC)
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
                            AcVoltage = new(new(10), new(300), new(0.01)),
                            AcCurrent = new(new(0), new(60), new(0.01))
                        },
                        new() {
                            AcVoltage = new(new(10), new(300), new(0.01)),
                            AcCurrent = new(new(0), new(60), new(0.01))
                        }
                    },
                    FrequencyRanges = new() {
                        new(new(40), new(60), new(0.1), FrequencyMode.SYNTHETIC)
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
                            AcVoltage = new(new(10), new(300), new(0.01)),
                            AcCurrent = new(new(0), new(60), new(0.01))
                        }
                    },
                    FrequencyRanges = new() {
                        new(new(40), new(60), new(0.1), FrequencyMode.SYNTHETIC)
                    }
                };
            }
        }
    }
}