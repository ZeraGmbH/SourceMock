using MeterTestSystemApi.Models.ConfigurationProviders;
using MeterTestSystemApi.Services;

namespace MeterTestSystemApiTests.ComponentConfiguration;

[TestFixture]
public class IPEndPointProviderTests
{
    [TestCase(113, 12823, "192.168.32.113:12823")]
    public void Can_Create_Address_From_Endpoint_Configuration(int ip, int port, string expected)
    {
        IPProbeEndPoint config = new IPEndPointProvider
        {
            IP = checked((byte)ip),
            Port = checked((ushort)port)
        };

        Assert.That(config.ToString(), Is.EqualTo(expected));
    }
}