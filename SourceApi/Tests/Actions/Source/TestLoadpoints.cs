
using ZERA.WebSam.Shared.Models.Source;

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
                        Current = new () { On = true, DcComponent = new(10) },
                        Voltage = new () { On = true, DcComponent = new(210) }
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
                        Current = new () { On = true, DcComponent = new(10) },
                        Voltage = new () { On = true, DcComponent = new(310) }
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
                        Current = new () { On = true, DcComponent = new(70 )},
                        Voltage = new () { On = true, DcComponent = new(210) }
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
                        Current = new () { On = true, AcComponent = new() {Angle = new(0), Rms = new(10),} },
                        Voltage = new () { On = true, AcComponent = new() {Angle = new(0), Rms = new(210)} }
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