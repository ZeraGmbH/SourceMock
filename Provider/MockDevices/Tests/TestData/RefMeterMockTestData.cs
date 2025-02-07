using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Source;

namespace RefMeterApiTests;

public static class RefMeterMockTestData
{

    public static TargetLoadpoint Loadpoint_OnlyActivePower
    {
        get
        {
            return new()
            {
                Phases = new() {
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = new(0),
                            Rms = new(230)
                        },
                        On = true,
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = new(0),
                            Rms = new(100)
                        },
                        On = true,
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = new(120),
                            Rms = new(235)
                        },
                        On = true,
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = new(120),
                            Rms = new(80)
                        },
                        On = true,
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = new(240),
                            Rms = new(240)
                        },
                        On = true,

                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = new(240),
                            Rms = new(60)
                        },
                        On = true,
                    }
                }
            },
                VoltageNeutralConnected = false,
                Frequency = new()
                {
                    Mode = FrequencyMode.SYNTHETIC,
                    Value = new(50d)
                }
            };
        }
    }

    public static TargetLoadpoint Loadpoint_OnlyReactivePower
    {
        get
        {
            return new()
            {
                Phases = new() {
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = new(0),
                            Rms = new(230)
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = new(90),
                            Rms = new(100)
                        },
                        On = true
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = new(120),
                            Rms = new(235)
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = new(210),
                            Rms = new(80)
                        },
                        On = true
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = new(240),
                            Rms = new(240)
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = new(330),
                            Rms = new(60)
                        },
                        On = true
                    }
                }
            },
                VoltageNeutralConnected = false,
                Frequency = new()
                {
                    Mode = FrequencyMode.SYNTHETIC,
                    Value = new(50d)
                }
            };
        }
    }

    public static TargetLoadpoint Loadpoint_CosPhi0_5
    {
        get
        {
            return new()
            {
                Phases = new() {
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = new(0),
                            Rms = new(230)
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = new(60),
                            Rms = new(100)
                        },
                        On = true
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = new(120),
                            Rms = new(235)
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = new(180),
                            Rms = new(80)
                        },
                        On = true
                    }
                },
                new() {
                    Voltage = new() {
                        AcComponent = new() {
                            Angle = new(240),
                            Rms = new(240)
                        },
                        On = true
                    },
                    Current = new() {
                        AcComponent = new() {
                            Angle = new(300),
                            Rms = new(60)
                        },
                        On = true
                    }
                }
            },
                VoltageNeutralConnected = false,
                Frequency = new()
                {
                    Mode = FrequencyMode.SYNTHETIC,
                    Value = new(50d)
                }
            };
        }
    }

}