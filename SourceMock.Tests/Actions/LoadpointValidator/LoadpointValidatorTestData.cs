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
                    Voltages = new() {
                        new ElectricalVectorQuantity() {
                            Rms = 230d,
                            Angle = 0d,
                            Harmonics = new()
                        },
                        new ElectricalVectorQuantity() {
                            Rms = 230d,
                            Angle = 120d,
                            Harmonics = new()
                        },
                        new ElectricalVectorQuantity() {
                            Rms = 230d,
                            Angle = 240d,
                            Harmonics = new()
                        }
                    },
                    Currents = new() {
                        new ElectricalVectorQuantity() {
                            Rms = 60d,
                            Angle = 5d,
                            Harmonics = new()
                        },
                        new ElectricalVectorQuantity() {
                            Rms = 60d,
                            Angle = 125d,
                            Harmonics = new()
                        },
                        new ElectricalVectorQuantity() {
                            Rms = 60d,
                            Angle = 245d,
                            Harmonics = new()
                        }
                    },
                    VoltageNeutralConnected = false,
                    Frequency = new()
                    {
                        Mode = FrequencyMode.SYNTHETIC,
                        Value = 50d
                    },
                    AuxilliaryVoltage = null
                };
            }
        }

        public static Loadpoint Loadpoint002_2AC_valid
        {
            get
            {
                return new()
                {
                    Voltages = new() {
                        new ElectricalVectorQuantity() {
                            Rms = 230d,
                            Angle = 0d,
                            Harmonics = new()
                        },
                        new ElectricalVectorQuantity() {
                            Rms = 230d,
                            Angle = 180d,
                            Harmonics = new()
                        }
                    },
                    Currents = new() {
                        new ElectricalVectorQuantity() {
                            Rms = 60d,
                            Angle = 5d,
                            Harmonics = new()
                        },
                        new ElectricalVectorQuantity() {
                            Rms = 60d,
                            Angle = 185d,
                            Harmonics = new()
                        }
                    },
                    VoltageNeutralConnected = false,
                    Frequency = new()
                    {
                        Mode = FrequencyMode.SYNTHETIC,
                        Value = 50d
                    },
                    AuxilliaryVoltage = null
                };
            }
        }

        public static Loadpoint Loadpoint003_1AC_valid
        {
            get
            {
                return new()
                {
                    Voltages = new() {
                        new ElectricalVectorQuantity() {
                            Rms = 230d,
                            Angle = 0d,
                            Harmonics = new()
                        }
                    },
                    Currents = new() {
                        new ElectricalVectorQuantity() {
                            Rms = 60d,
                            Angle = 5d,
                            Harmonics = new()
                        }
                    },
                    VoltageNeutralConnected = false,
                    Frequency = new()
                    {
                        Mode = FrequencyMode.SYNTHETIC,
                        Value = 50d
                    },
                    AuxilliaryVoltage = null
                };
            }
        }
        #endregion

        #region InvalidLoadpoints_MissingPhase
        public static IEnumerable<TestCaseData> InvalidLoadPoints_MissingPhase
        {
            get
            {
                yield return new TestCaseData(Loadpoint101_invalid_tooFewVoltages);
                yield return new TestCaseData(Loadpoint102_invalid_tooFewCurrents);
            }
        }

        public static Loadpoint Loadpoint101_invalid_tooFewVoltages
        {
            get
            {
                var ret = Loadpoint001_3AC_valid;
                ret.Voltages = new() {
                    new ElectricalVectorQuantity() {
                        Rms = 230d,
                        Angle = 0d,
                        Harmonics = new()
                    },
                    new ElectricalVectorQuantity() {
                        Rms = 230d,
                        Angle = 120d,
                        Harmonics = new()
                    }
                };
                return ret;
            }
        }

        public static Loadpoint Loadpoint102_invalid_tooFewCurrents
        {
            get
            {
                var ret = Loadpoint001_3AC_valid;
                ret.Currents = new() {
                    new ElectricalVectorQuantity() {
                        Rms = 60d,
                        Angle = 0d,
                        Harmonics = new()
                    },
                    new ElectricalVectorQuantity() {
                        Rms = 60d,
                        Angle = 120d,
                        Harmonics = new()
                    }
                };
                return ret;

            }
        }
        #endregion
    }
}
