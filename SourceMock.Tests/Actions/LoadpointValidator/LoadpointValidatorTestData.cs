using SourceMock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMock.Tests.Actions.LoadpointValidator
{
    public static class LoadpointValidatorTestData
    {
        #region ValidLoadpoints
        public static IEnumerable<TestCaseData> ValidLoadpoints
        {
            get
            {
                yield return new TestCaseData(loadpoint001);
                yield return new TestCaseData(loadpoint002);
            }
        }
        
        public static Loadpoint loadpoint001 = new Loadpoint()
        {
            Voltages = new[] { 230d, 230d, 230d },
            Currents = new[] { 60d, 60d, 60d },
            Frequency = 50,
            PhaseAnglesVoltage = new[] { 0d, 120d, 240d },
            PhaseAnglesCurrent = new[] { 5d, 125d, 245d }
        };

        public static Loadpoint loadpoint002 = new Loadpoint()
        {
            Voltages = new[] { 110d, 110d },
            Currents = new[] { 60d, 60d },
            Frequency = 50,
            PhaseAnglesVoltage = new[] { 0d, 180d },
            PhaseAnglesCurrent = new[] { 5d, 185d }
        };
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

        public static Loadpoint loadpoint101_invalid = new Loadpoint()
        {
            Voltages = new[] { 230d, 230d },
            Currents = new[] { 60d, 60d, 60d },
            Frequency = 50,
            PhaseAnglesVoltage = new[] { 0d, 120d, 240d },
            PhaseAnglesCurrent = new[] { 5d, 125d, 245d }
        };

        public static Loadpoint loadpoint102_invalid = new Loadpoint()
        {
            Voltages = new[] { 230d, 230d, 230d },
            Currents = new[] { 60d, 60d },
            Frequency = 50,
            PhaseAnglesVoltage = new[] { 0d, 120d, 240d },
            PhaseAnglesCurrent = new[] { 5d, 125d, 245d }
        };

        public static Loadpoint loadpoint103_invalid = new Loadpoint()
        {
            Voltages = new[] { 230d, 230d, 230d },
            Currents = new[] { 60d, 60d, 60d },
            Frequency = 50,
            PhaseAnglesVoltage = new[] { 0d, 120d },
            PhaseAnglesCurrent = new[] { 5d, 125d, 245d }
        };

        public static Loadpoint loadpoint104_invalid = new Loadpoint()
        {
            Voltages = new[] { 230d, 230d, 230d },
            Currents = new[] { 60d, 60d, 60d },
            Frequency = 50,
            PhaseAnglesVoltage = new[] { 0d, 120d, 240d },
            PhaseAnglesCurrent = new[] { 5d, 125d }
        };
        #endregion
    }
}
