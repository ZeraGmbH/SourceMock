using SourceMock.Model;

namespace SourceMock.Tests.Actions.LoadpointValidator
{
    public static class LoadpointValidatorTestData
    {
        #region ValidLoadpoints
        public static IEnumerable<TestCaseData> ValidLoadpoints
        {
            get
            {
                yield return new TestCaseData(Loadpoint001_3AC_valid);
                yield return new TestCaseData(Loadpoint002_2AC_valid);
                yield return new TestCaseData(Loadpoint003_1AC_valid);
            }
        }

        public static Loadpoint Loadpoint001_3AC_valid
        {
            get
            {
                return new()
                {
                    Phases = new() {
                        new() {
                            Voltage = new() {
                                Rms = 230d,
                                Angle = 0d,
                                On = true
                            },
                            Current = new() {
                                Rms = 60d,
                                Angle = 5d,
                                On = true
                            }
                        },
                        new() {
                            Voltage = new() {
                                Rms = 230d,
                                Angle = 120d,
                                On = true
                            },
                            Current = new() {
                                Rms = 60d,
                                Angle = 125d,
                                On = true
                            }
                        },
                        new() {
                            Voltage = new() {
                                Rms = 230d,
                                Angle = 240d,
                                On = true
                            },
                            Current = new() {
                                Rms = 60d,
                                Angle = 245d,
                                On = true
                            }
                        }
                    },
                    VoltageNeutralConnected = false,
                    Frequency = new()
                    {
                        Mode = FrequencyMode.SYNTHETIC,
                        Value = 50d
                    }
                };
            }
        }

        public static Loadpoint Loadpoint002_2AC_valid
        {
            get
            {
                return new()
                {
                    Phases = new() {
                        new() {
                            Voltage = new() {
                                Rms = 230d,
                                Angle = 0d,
                                On = true
                            },
                            Current = new() {
                                Rms = 60d,
                                Angle = 5d,
                                On = true
                            }
                        },
                        new() {
                            Voltage = new() {
                                Rms = 230d,
                                Angle = 180d,
                                On = true
                            },
                            Current = new() {
                                Rms = 60d,
                                Angle = 185d,
                                On = true
                            }
                        }
                    },
                    VoltageNeutralConnected = false,
                    Frequency = new()
                    {
                        Mode = FrequencyMode.SYNTHETIC,
                        Value = 50d
                    }
                };
            }
        }

        public static Loadpoint Loadpoint003_1AC_valid
        {
            get
            {
                return new()
                {
                    Phases = new() {
                        new() {
                            Voltage = new() {
                                Rms = 230d,
                                Angle = 0d,
                                On = true
                            },
                            Current = new() {
                                Rms = 60d,
                                Angle = 5d,
                                On = true
                            }
                        }
                    },
                    VoltageNeutralConnected = false,
                    Frequency = new()
                    {
                        Mode = FrequencyMode.SYNTHETIC,
                        Value = 50d
                    }
                };
            }
        }
        #endregion
    }
}
