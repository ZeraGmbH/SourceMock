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
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetMadEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(1, ServerTypes.STM6000, "192.168.32.101:14207")]
    [TestCase(10, ServerTypes.STM6000, "192.168.32.110:14207")]
    [TestCase(11, ServerTypes.STM6000, "192.168.32.111:14207")]
    [TestCase(100, ServerTypes.STM6000, "192.168.32.200:14207")]
    [TestCase(1, ServerTypes.STM4000, "192.168.32.181:14007")]
    [TestCase(10, ServerTypes.STM4000, "192.168.32.181:14907")]
    [TestCase(11, ServerTypes.STM4000, "192.168.32.182:14007")]
    [TestCase(100, ServerTypes.STM4000, "192.168.32.190:14907")]
    public void Can_Get_IP_Address_For_MAD_Server(int position, ServerTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetMadEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(101)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Update_Server(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetUpdateEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(1, ServerTypes.STM6000, "192.168.32.101:14196")]
    [TestCase(10, ServerTypes.STM6000, "192.168.32.110:14196")]
    [TestCase(11, ServerTypes.STM6000, "192.168.32.111:14196")]
    [TestCase(100, ServerTypes.STM6000, "192.168.32.200:14196")]
    [TestCase(1, ServerTypes.STM4000, "192.168.32.181:14196")]
    [TestCase(10, ServerTypes.STM4000, "192.168.32.181:14196")]
    [TestCase(11, ServerTypes.STM4000, "192.168.32.182:14196")]
    [TestCase(100, ServerTypes.STM4000, "192.168.32.190:14196")]
    public void Can_Get_IP_Address_For_Update_Server(int position, ServerTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetUpdateEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(101)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Direct_Dut_Connection(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetDirectDutConnectionEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(1, ServerTypes.STM6000, "192.168.32.101:14202")]
    [TestCase(10, ServerTypes.STM6000, "192.168.32.110:14202")]
    [TestCase(11, ServerTypes.STM6000, "192.168.32.111:14202")]
    [TestCase(100, ServerTypes.STM6000, "192.168.32.200:14202")]
    [TestCase(1, ServerTypes.STM4000, "192.168.32.181:14002")]
    [TestCase(10, ServerTypes.STM4000, "192.168.32.181:14902")]
    [TestCase(11, ServerTypes.STM4000, "192.168.32.182:14002")]
    [TestCase(100, ServerTypes.STM4000, "192.168.32.190:14902")]
    public void Can_Get_IP_Address_For_Direct_Dut_Connection(int position, ServerTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetDirectDutConnectionEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }
}