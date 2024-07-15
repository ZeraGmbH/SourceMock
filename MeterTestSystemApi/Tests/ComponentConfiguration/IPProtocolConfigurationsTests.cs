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
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetMadEndpoint(position, STMTypes.STM6000));
    }

    [TestCase(1, STMTypes.STM6000, "192.168.32.101:14207")]
    [TestCase(10, STMTypes.STM6000, "192.168.32.110:14207")]
    [TestCase(11, STMTypes.STM6000, "192.168.32.111:14207")]
    [TestCase(100, STMTypes.STM6000, "192.168.32.200:14207")]
    [TestCase(1, STMTypes.STM4000, "192.168.32.181:14007")]
    [TestCase(10, STMTypes.STM4000, "192.168.32.181:14907")]
    [TestCase(11, STMTypes.STM4000, "192.168.32.182:14007")]
    [TestCase(100, STMTypes.STM4000, "192.168.32.190:14907")]
    public void Can_Get_IP_Address_For_MAD_Server(int position, STMTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetMadEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(101)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Update_Server(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetUpdateEndpoint(position, STMTypes.STM6000));
    }

    [TestCase(1, STMTypes.STM6000, "192.168.32.101:14196")]
    [TestCase(10, STMTypes.STM6000, "192.168.32.110:14196")]
    [TestCase(11, STMTypes.STM6000, "192.168.32.111:14196")]
    [TestCase(100, STMTypes.STM6000, "192.168.32.200:14196")]
    [TestCase(1, STMTypes.STM4000, "192.168.32.181:14196")]
    [TestCase(10, STMTypes.STM4000, "192.168.32.181:14196")]
    [TestCase(11, STMTypes.STM4000, "192.168.32.182:14196")]
    [TestCase(100, STMTypes.STM4000, "192.168.32.190:14196")]
    public void Can_Get_IP_Address_For_Update_Server(int position, STMTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetUpdateEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }
}