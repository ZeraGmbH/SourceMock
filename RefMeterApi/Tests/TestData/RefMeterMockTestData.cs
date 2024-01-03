using SourceApi.Model;

namespace RefMeterApiTests;

public static class RefMeterMockTestData {

    public static Loadpoint Loadpoint_OnlyActivePower { get {
        return new() {
            Phases = new() {
                new() {
                    Voltage = new() {
                        Angle = 0,
                        On = true,
                        Rms = 230
                    },
                    Current = new() {
                        Angle = 0,
                        On = true,
                        Rms = 100
                    }
                },
                new() {
                    Voltage = new() {
                        Angle = 120,
                        On = true,
                        Rms = 235
                    },
                    Current = new() {
                        Angle = 120,
                        On = true,
                        Rms = 80
                    }
                },
                new() {
                    Voltage = new() {
                        Angle = 240,
                        On = true,
                        Rms = 240

                    },
                    Current = new() {
                        Angle = 240,
                        On = true,
                        Rms = 60
                    }
                }
            },
            VoltageNeutralConnected = false,
            Frequency = new() {
                Mode = FrequencyMode.SYNTHETIC,
                Value = 50d
            }
        };
    }}

    public static Loadpoint Loadpoint_OnlyReactivePower { get {
        return new() {
            Phases = new() {
                new() {
                    Voltage = new() {
                        Angle = 0,
                        On = true,
                        Rms = 230
                    },
                    Current = new() {
                        Angle = 90,
                        On = true,
                        Rms = 100
                    }
                },
                new() {
                    Voltage = new() {
                        Angle = 120,
                        On = true,
                        Rms = 235
                    },
                    Current = new() {
                        Angle = 210,
                        On = true,
                        Rms = 80
                    }
                },
                new() {
                    Voltage = new() {
                        Angle = 240,
                        On = true,
                        Rms = 240

                    },
                    Current = new() {
                        Angle = 330,
                        On = true,
                        Rms = 60
                    }
                }
            },
            VoltageNeutralConnected = false,
            Frequency = new() {
                Mode = FrequencyMode.SYNTHETIC,
                Value = 50d
            }
        };
    }}

    public static Loadpoint Loadpoint_CosPhi0_5{ get {
        return new() {
            Phases = new() {
                new() {
                    Voltage = new() {
                        Angle = 0,
                        On = true,
                        Rms = 230
                    },
                    Current = new() {
                        Angle = 60,
                        On = true,
                        Rms = 100
                    }
                },
                new() {
                    Voltage = new() {
                        Angle = 120,
                        On = true,
                        Rms = 235
                    },
                    Current = new() {
                        Angle = 180,
                        On = true,
                        Rms = 80
                    }
                },
                new() {
                    Voltage = new() {
                        Angle = 240,
                        On = true,
                        Rms = 240

                    },
                    Current = new() {
                        Angle = 300,
                        On = true,
                        Rms = 60
                    }
                }
            },
            VoltageNeutralConnected = false,
            Frequency = new() {
                Mode = FrequencyMode.SYNTHETIC,
                Value = 50d
            }
        };
    }}

}