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
                    Voltages = new[] { 230d, 230d, 230d },
                    Currents = new[] { 60d, 60d, 60d },
                    Frequency = 50,
                    PhaseAnglesVoltage = new[] { 0d, 120d, 240d },
                    PhaseAnglesCurrent = new[] { 5d, 125d, 245d }
                };
            }
        }

        public static Loadpoint Loadpoint002_2AC_valid
        {
            get
            {
                return new()
                {
                    Voltages = new[] { 110d, 110d },
                    Currents = new[] { 60d, 60d },
                    Frequency = 50,
                    PhaseAnglesVoltage = new[] { 0d, 180d },
                    PhaseAnglesCurrent = new[] { 5d, 185d }
                };
            }
        }

        public static Loadpoint Loadpoint003_1AC_valid
        {
            get
            {
                return new()
                {
                    Voltages = new[] { 110d },
                    Currents = new[] { 60d },
                    Frequency = 50,
                    PhaseAnglesVoltage = new[] { 0d },
                    PhaseAnglesCurrent = new[] { 5d }
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
                yield return new TestCaseData(Loadpoint103_invalid_tooFewPhaseAnglesVoltage);
                yield return new TestCaseData(Loadpoint104_invalid_tooFewPhaseAnglesCurrent);
            }
        }

        public static Loadpoint Loadpoint101_invalid_tooFewVoltages
        {
            get
            {
                var ret = Loadpoint001_3AC_valid;
                ret.Voltages = new[] { 230d, 230d };
                return ret;
            }
        }

        public static Loadpoint Loadpoint102_invalid_tooFewCurrents
        {
            get
            {
                var ret = Loadpoint001_3AC_valid;
                ret.Currents = new[] { 60d, 60d };
                return ret;

            }
        }

        public static Loadpoint Loadpoint103_invalid_tooFewPhaseAnglesVoltage
        {
            get
            {
                var ret = Loadpoint001_3AC_valid;
                ret.PhaseAnglesVoltage = new[] { 0d, 120d };
                return ret;
            }
        }

        public static Loadpoint Loadpoint104_invalid_tooFewPhaseAnglesCurrent
        {
            get
            {
                var ret = Loadpoint001_3AC_valid;
                ret.PhaseAnglesCurrent = new[] { 5d, 125d };
                return ret;
            }
        }
        #endregion
    }
}
