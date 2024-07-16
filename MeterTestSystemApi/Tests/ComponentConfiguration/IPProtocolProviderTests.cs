using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;

namespace MeterTestSystemApiTests.ComponentConfiguration;

[TestFixture]
public class IPProtocolProviderTests
{
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_MAD_Server(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolProvider.GetMadEndpoint(position, ServerTypes.STM6000));
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
        Assert.That(IPProtocolProvider.GetMadEndpoint(position, type).ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Update_Server(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolProvider.GetUpdateEndpoint(position, ServerTypes.STM6000));
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
        Assert.That(IPProtocolProvider.GetUpdateEndpoint(position, type).ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Direct_Dut_Connection(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolProvider.GetDirectDutConnectionEndpoint(position, ServerTypes.STM6000));
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
        Assert.That(IPProtocolProvider.GetDirectDutConnectionEndpoint(position, type).ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_UART(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolProvider.GetUARTEndpoint(position, ServerTypes.STM6000));
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
        Assert.That(IPProtocolProvider.GetUARTEndpoint(position, type).ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Legacy_OA(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolProvider.GetObjectAccessEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(ServerTypes.STM4000)]
    public void Can_Detect_Wrong_Server_Type_For_Legacy_OA(ServerTypes type)
    {
        Assert.Throws<ArgumentException>(() => IPProtocolProvider.GetObjectAccessEndpoint(1, type));
    }

    [TestCase(1, "192.168.32.101:14204")]
    [TestCase(10, "192.168.32.110:14204")]
    [TestCase(11, "192.168.32.111:14204")]
    [TestCase(80, "192.168.32.180:14204")]
    public void Can_Get_IP_Address_For_Legacy_OA(int position, string expected)
    {
        Assert.That(IPProtocolProvider.GetObjectAccessEndpoint(position, ServerTypes.STM6000).ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_COM_Server(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolProvider.GetCOMServerEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(ServerTypes.STM4000)]
    public void Can_Detect_Wrong_Server_Type_For_COM_Server(ServerTypes type)
    {
        Assert.Throws<ArgumentException>(() => IPProtocolProvider.GetCOMServerEndpoint(1, type));
    }

    [TestCase(1, "192.168.32.101:14201")]
    [TestCase(10, "192.168.32.110:14201")]
    [TestCase(11, "192.168.32.111:14201")]
    [TestCase(80, "192.168.32.180:14201")]
    public void Can_Get_IP_Address_For_COM_Server(int position, string expected)
    {
        Assert.That(IPProtocolProvider.GetCOMServerEndpoint(position, ServerTypes.STM6000).ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_SIM_Server_1(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolProvider.GetSIMServer1Endpoint(position, ServerTypes.STM6000));
    }

    [TestCase(ServerTypes.STM4000)]
    public void Can_Detect_Wrong_Server_Type_For_SIM_Server_1(ServerTypes type)
    {
        Assert.Throws<ArgumentException>(() => IPProtocolProvider.GetSIMServer1Endpoint(1, type));
    }

    [TestCase(1, "192.168.32.101:14208")]
    [TestCase(10, "192.168.32.110:14208")]
    [TestCase(11, "192.168.32.111:14208")]
    [TestCase(80, "192.168.32.180:14208")]
    public void Can_Get_IP_Address_For_SIM_Server_1(int position, string expected)
    {
        Assert.That(IPProtocolProvider.GetSIMServer1Endpoint(position, ServerTypes.STM6000).ToString(), Is.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(81)]
    [TestCase(1000)]
    public void Can_Validate_Parameters_For_Backend_Gateway(int position)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => IPProtocolProvider.GetBackendGatewayEndpoint(position, ServerTypes.STM6000));
    }

    [TestCase(ServerTypes.STM4000)]
    public void Can_Detect_Wrong_Server_Type_For_Backend_Gateway(ServerTypes type)
    {
        Assert.Throws<ArgumentException>(() => IPProtocolProvider.GetBackendGatewayEndpoint(1, type));
    }

    [TestCase(1, "192.168.32.101:14198")]
    [TestCase(10, "192.168.32.110:14198")]
    [TestCase(11, "192.168.32.111:14198")]
    [TestCase(80, "192.168.32.180:14198")]
    public void Can_Get_IP_Address_For_Backend_Gateway(int position, string expected)
    {
        Assert.That(IPProtocolProvider.GetBackendGatewayEndpoint(position, ServerTypes.STM6000).ToString(), Is.EqualTo(expected));
    }
}