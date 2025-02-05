using ZERA.WebSam.Shared.Models.Source;

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
                            DcVoltage = new(new(10), new(300), new(0.01)),
                            DcCurrent = new(new(0), new(60), new(0.01))
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
                            AcCurrent = new(new(10), new(300), new(0.01)),
                            AcVoltage = new(new(0), new(60), new(0.01))
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