using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApiTests.ComponentConfiguration;

[TestFixture]
public class IPProtocolConfigurationsTests
{
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(101)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_MAD_Server(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetMadEndpoint(position, MadServerTypes.STM6000));
    }

    [TestCase(1, MadServerTypes.STM6000, "192.168.32.101:14207")]
    [TestCase(10, MadServerTypes.STM6000, "192.168.32.110:14207")]
    [TestCase(11, MadServerTypes.STM6000, "192.168.32.111:14207")]
    [TestCase(100, MadServerTypes.STM6000, "192.168.32.200:14207")]
    [TestCase(1, MadServerTypes.STM4000, "192.168.32.181:14007")]
    [TestCase(10, MadServerTypes.STM4000, "192.168.32.181:14907")]
    [TestCase(11, MadServerTypes.STM4000, "192.168.32.182:14007")]
    [TestCase(100, MadServerTypes.STM4000, "192.168.32.190:14907")]
    public void Can_Get_IP_Address_For_MAD_Server(int position, MadServerTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetMadEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }
}