using MeterTestSystemApi.Models.Configuration;

namespace MeterTestSystemApiTests.ComponentConfiguration;

[TestFixture]
public class IPProtocolConfigurationsTests
{
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_MAD_Server(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetMadEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(1, ServerTypes.STM6000, "192.168.32.101:14207")]
    [TestCase(10, ServerTypes.STM6000, "192.168.32.110:14207")]
    [TestCase(11, ServerTypes.STM6000, "192.168.32.111:14207")]
    [TestCase(80, ServerTypes.STM6000, "192.168.32.180:14207")]
    [TestCase(1, ServerTypes.STM4000, "192.168.32.181:14007")]
    [TestCase(10, ServerTypes.STM4000, "192.168.32.181:14907")]
    [TestCase(11, ServerTypes.STM4000, "192.168.32.182:14007")]
    [TestCase(80, ServerTypes.STM4000, "192.168.32.188:14907")]
    public void Can_Get_IP_Address_For_MAD_Server(int position, ServerTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetMadEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Update_Server(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetUpdateEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(1, ServerTypes.STM6000, "192.168.32.101:14196")]
    [TestCase(10, ServerTypes.STM6000, "192.168.32.110:14196")]
    [TestCase(11, ServerTypes.STM6000, "192.168.32.111:14196")]
    [TestCase(80, ServerTypes.STM6000, "192.168.32.180:14196")]
    [TestCase(1, ServerTypes.STM4000, "192.168.32.181:14196")]
    [TestCase(10, ServerTypes.STM4000, "192.168.32.181:14196")]
    [TestCase(11, ServerTypes.STM4000, "192.168.32.182:14196")]
    [TestCase(80, ServerTypes.STM4000, "192.168.32.188:14196")]
    public void Can_Get_IP_Address_For_Update_Server(int position, ServerTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetUpdateEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Direct_Dut_Connection(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetDirectDutConnectionEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(1, ServerTypes.STM6000, "192.168.32.101:14202")]
    [TestCase(10, ServerTypes.STM6000, "192.168.32.110:14202")]
    [TestCase(11, ServerTypes.STM6000, "192.168.32.111:14202")]
    [TestCase(80, ServerTypes.STM6000, "192.168.32.180:14202")]
    [TestCase(1, ServerTypes.STM4000, "192.168.32.181:14002")]
    [TestCase(10, ServerTypes.STM4000, "192.168.32.181:14902")]
    [TestCase(11, ServerTypes.STM4000, "192.168.32.182:14002")]
    [TestCase(80, ServerTypes.STM4000, "192.168.32.188:14902")]
    public void Can_Get_IP_Address_For_Direct_Dut_Connection(int position, ServerTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetDirectDutConnectionEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Old_UART(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetLegacyUARTEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(1, ServerTypes.STM6000, "192.168.32.101:14203")]
    [TestCase(10, ServerTypes.STM6000, "192.168.32.110:14203")]
    [TestCase(11, ServerTypes.STM6000, "192.168.32.111:14203")]
    [TestCase(80, ServerTypes.STM6000, "192.168.32.180:14203")]
    [TestCase(1, ServerTypes.STM4000, "192.168.32.181:14003")]
    [TestCase(10, ServerTypes.STM4000, "192.168.32.181:14903")]
    [TestCase(11, ServerTypes.STM4000, "192.168.32.182:14003")]
    [TestCase(80, ServerTypes.STM4000, "192.168.32.188:14903")]
    public void Can_Get_IP_Address_For_Old_UART(int position, ServerTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetLegacyUARTEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_UART(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetUARTEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(1, ServerTypes.STM6000, "192.168.32.101:14209")]
    [TestCase(10, ServerTypes.STM6000, "192.168.32.110:14209")]
    [TestCase(11, ServerTypes.STM6000, "192.168.32.111:14209")]
    [TestCase(80, ServerTypes.STM6000, "192.168.32.180:14209")]
    [TestCase(1, ServerTypes.STM4000, "192.168.32.181:14009")]
    [TestCase(10, ServerTypes.STM4000, "192.168.32.181:14909")]
    [TestCase(11, ServerTypes.STM4000, "192.168.32.182:14009")]
    [TestCase(80, ServerTypes.STM4000, "192.168.32.188:14909")]
    public void Can_Get_IP_Address_For_UART(int position, ServerTypes type, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetUARTEndpoint(position, type).Address.ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Legacy_OA(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolConfigurations.GetLegacyOAEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(ServerTypes.STM4000)]
    public void Can_Detect_Wrong_Server_Type_For_Legacy_OA(ServerTypes type)
    {
        Assert.Throws<ArgumentException>(() => IPProtocolConfigurations.GetLegacyOAEndpoint(1, type));
    }

    [TestCase(1, "192.168.32.101:14204")]
    [TestCase(10, "192.168.32.110:14204")]
    [TestCase(11, "192.168.32.111:14204")]
    [TestCase(80, "192.168.32.180:14204")]
    public void Can_Get_IP_Address_For_Legacy_OA(int position, string expected)
    {
        Assert.That(IPProtocolConfigurations.GetLegacyOAEndpoint(position, ServerTypes.STM6000).Address.ToString(), Is.EqualTo(expected));
    }
}