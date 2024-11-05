using System.Text.Json.Serialization;

namespace SourceApi.Model;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PllChannel
{
    U1 = 0,
    U2 = 1,
    U3 = 2,
    I1 = 3,
    I2 = 4,
    I3 = 5,
}
