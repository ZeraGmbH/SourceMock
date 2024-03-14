
using SourceApi.Model;

namespace SourceApi.Tests.Actions.Source
{
    public static class TestCapabilities
    {
        public static SourceCapabilities GetDcSourceCapabilities()
        {
            return new()
            {
                Phases = new() {
                        new() {
                            DcVoltage = new(10, 300, 0.01),
                            DcCurrent = new(0, 60, 0.01)
                        },
                    },
            };
        }
        public static SourceCapabilities GetAcSourceCapabilities()
        {
            return new()
            {
                Phases = new() {
                        new() {
                            AcCurrent = new(10, 300, 0.01),
                            AcVoltage = new(0, 60, 0.01)
                        },
                    },
            };
        }
        public static SourceCapabilities GetNullCapabilities()
        {
            return new()
            {
            };
        }
    }
}