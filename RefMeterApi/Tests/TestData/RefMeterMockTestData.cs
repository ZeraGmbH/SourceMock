using SourceApi.Model;

namespace RefMeterApiTests;

public static class RefMeterMockTestData {

    public static Loadpoint Loadpoint_OnlyActivePower { get {
        return new() {
            Phases = new() {
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = 0,
                            Rms = 230
                        },
                        On = true,
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = 0,
                            Rms = 100
                        },
                        On = true,
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = 120,
                            Rms = 235
                        },
                        On = true,
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = 120,
                            Rms = 80
                        },
                        On = true,
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = 240,
                            Rms = 240
                        },
                        On = true,

                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = 240,
                            Rms = 60
                        },
                        On = true,
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
                        AcComponent = new() {
                            Angle = 0,
                            Rms = 230
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = 90,
                            Rms = 100
                        },
                        On = true
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = 120,
                            Rms = 235
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = 210,
                            Rms = 80
                        },
                        On = true
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = 240,
                            Rms = 240
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = 330,
                            Rms = 60
                        },
                        On = true
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
                        AcComponent = new() {
                            Angle = 0,
                            Rms = 230
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = 60,
                            Rms = 100
                        },
                        On = true
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = 120,
                            Rms = 235
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = 180,
                            Rms = 80
                        },
                        On = true
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = 240,
                            Rms = 240
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = 300,
                            Rms = 60
                        },
                        On = true
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