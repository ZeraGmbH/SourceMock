
using SourceApi.Model;

namespace SourceApi.Tests.Actions.Source
{
    public static class TestLoadpoints
    {
        public static TargetLoadpoint GetDcLoadpoint()
        {
            return new()
            {
                Phases = new List<TargetLoadpointPhase>(){
                    new TargetLoadpointPhase(){
                        Current = new () { On = true, DcComponent = 10 },
                        Voltage = new () { On = true, DcComponent = 210 }
                    },
                }
            };
        }

        public static TargetLoadpoint GetTooHighVoltageDcLoadpoint()
        {
            return new()
            {
                Phases = new List<TargetLoadpointPhase>(){
                    new TargetLoadpointPhase(){
                        Current = new () { On = true, DcComponent = 10 },
                        Voltage = new () { On = true, DcComponent = 310 }
                    },
                }
            };
        }

        public static TargetLoadpoint GetTooHighCurrentDcLoadpoint()
        {
            return new()
            {
                Phases = new List<TargetLoadpointPhase>(){
                    new TargetLoadpointPhase(){
                        Current = new () { On = true, DcComponent = 70 },
                        Voltage = new () { On = true, DcComponent = 210 }
                    },
                }
            };
        }
        public static TargetLoadpoint GetACLoadpoint()
        {
            return new()
            {
                Phases = new List<TargetLoadpointPhase>(){
                    new TargetLoadpointPhase(){
                        Current = new () { On = true, AcComponent = new() {Angle = 0, Rms = 10,} },
                        Voltage = new () { On = true, AcComponent = new() {Angle = 0, Rms = 210} }
                    },
                }
            };
        }
        public static TargetLoadpoint GetNullLoadpoint()
        {
            return new()
            { };
        }
    }
}