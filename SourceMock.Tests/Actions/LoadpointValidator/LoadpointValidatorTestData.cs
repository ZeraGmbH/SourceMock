using SourceMock.Extensions;
using SourceMock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMock.Tests.Actions.LoadpointValidator
{
// Can't be constant as there are non-constant members, must be public as it is used in the TestClass
#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static class LoadpointValidatorTestData
    {
        #region ValidLoadpoints
        public static IEnumerable<TestCaseData> ValidLoadpoints
        {
            get
            {
                yield return new TestCaseData(loadpoint001_3AC_valid);
                yield return new TestCaseData(loadpoint002_2AC_valid);
                yield return new TestCaseData(loadpoint003_1AC_valid);
            }
        }
        
        public static Loadpoint loadpoint001_3AC_valid
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

        public static Loadpoint loadpoint002_2AC_valid
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

        public static Loadpoint loadpoint003_1AC_valid
        {
            get { 
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
                yield return new TestCaseData(loadpoint101_invalid);
                yield return new TestCaseData(loadpoint102_invalid);
                yield return new TestCaseData(loadpoint103_invalid);
                yield return new TestCaseData(loadpoint104_invalid);
            }
        }

        public static Loadpoint loadpoint101_invalid
        {
            get
            {
                var ret = loadpoint001_3AC_valid;
                ret.Voltages = new[] { 230d, 230d };
                return ret;
            }
        }

        public static Loadpoint loadpoint102_invalid
        {
            get
            {
                var ret = loadpoint001_3AC_valid;
                ret.Currents = new[] { 60d, 60d };
                return ret;

            }
        }

        public static Loadpoint loadpoint103_invalid
        {
            get
            {
                var ret = loadpoint001_3AC_valid;
                ret.PhaseAnglesVoltage = new[] { 0d, 120d };
                return ret;
            }
        }

        public static Loadpoint loadpoint104_invalid
        {
            get
            {
                var ret = loadpoint001_3AC_valid;
                ret.PhaseAnglesCurrent = new[] { 5d, 125d };
                return ret;
            }
        }
        #endregion
    }
#pragma warning restore CA2211 // Non-constant fields should not be visible
}
