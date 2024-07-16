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

    [Test]
    public void Can_Get_IP_Address_For_MP2020_Control()
    {
        Assert.That(IPProtocolProvider.Get2020ControlEndpoint().ToString(), Is.EqualTo("192.168.32.100:14200"));
    }

    [Test]
    public void Can_Get_IP_Address_For_Omega_iBHTX()
    {
        Assert.That(IPProtocolProvider.GetOmegaiBTHXEndpoint().ToString(), Is.EqualTo("192.168.32.12:2000"));
    }

    [Test]
    public void Can_Get_IP_Address_For_COM5003()
    {
        Assert.That(IPProtocolProvider.GetCOM5003Endpoint().ToString(), Is.EqualTo("192.168.32.13:6320"));
    }

    [Test]
    public void Can_Get_IP_Address_For_IP_Watchdog()
    {
        Assert.That(IPProtocolProvider.GetIPWatchDogEndpoint().ToString(), Is.EqualTo("192.168.32.16:80"));
    }

    [Test]
    public void Can_Get_IP_Address_For_DTS100()
    {
        Assert.That(IPProtocolProvider.GetDTS100Endpoint().ToString(), Is.EqualTo("192.168.32.17:4001"));
    }

    [TestCase(DCComponents.CurrentSCG8, "192.168.32.80:10001")]
    [TestCase(DCComponents.CurrentSCG80, "192.168.32.81:10001")]
    [TestCase(DCComponents.CurrentSCG750, "192.168.32.82:10001")]
    [TestCase(DCComponents.CurrentSCG06, "192.168.32.83:10001")]
    [TestCase(DCComponents.CurrentSCG8, "192.168.32.80:10001")]
    [TestCase(DCComponents.CurrentSCG1000, "192.168.32.84:10001")]
    [TestCase(DCComponents.VoltageSVG1200, "192.168.32.85:10001")]
    [TestCase(DCComponents.VoltageSVG150, "192.168.32.89:10001")]
    [TestCase(DCComponents.SPS, "192.168.32.200:0")]
    [TestCase(DCComponents.FGControl, "192.168.32.91:13000")]
    public void Can_Get_IP_Address_For_DC_Component(DCComponents component, string expected)
    {
        Assert.That(IPProtocolProvider.GetDCComponentEndpoint(component).ToString(), Is.EqualTo(expected));
    }

    [TestCase(DCComponents.None)]
    [TestCase(DCComponents.CurrentSCG8 | DCComponents.VoltageSVG150)]
    public void Can_Detect_Invalid_DC_Component(DCComponents component)
    {
        Assert.Throws<ArgumentException>(() => IPProtocolProvider.GetDCComponentEndpoint(component));
    }

    [TestCase(TransformerComponents.SPS, "192.168.32.200:0")]
    [TestCase(TransformerComponents.STR260Phase1, "192.168.32.201:0")]
    [TestCase(TransformerComponents.STR260Phase2, "192.168.32.202:0")]
    [TestCase(TransformerComponents.STR260Phase3, "192.168.32.203:0")]
    [TestCase(TransformerComponents.CurrentWM3000or1000, "192.168.32.211:6315")]
    [TestCase(TransformerComponents.VoltageWM3000or1000, "192.168.32.221:6315")]
    public void Can_Get_IP_Address_For_Transformer_Component(TransformerComponents component, string expected)
    {
        Assert.That(IPProtocolProvider.GetTransformerComponentEndpoint(component).ToString(), Is.EqualTo(expected));
    }

    [TestCase(TransformerComponents.None)]
    [TestCase(TransformerComponents.STR260Phase3 | TransformerComponents.STR260Phase2)]
    public void Can_Detect_Invalid_Transformer_Component(TransformerComponents component)
    {
        Assert.Throws<ArgumentException>(() => IPProtocolProvider.GetTransformerComponentEndpoint(component));
    }

    [TestCase(MT310s2Functions.EMobReferenceMeter, "192.168.32.14:6320")]
    [TestCase(MT310s2Functions.RemoteGUI, "192.168.32.14:8080")]
    [TestCase(MT310s2Functions.DCReferenceMeter1, "192.168.32.20:6320")]
    [TestCase(MT310s2Functions.DCReferenceMeter2, "192.168.32.21:6320")]
    [TestCase(MT310s2Functions.DCCalibration, "192.168.32.22:6320")]
    public void Can_Get_IP_Address_For_MT310s2_Functions(MT310s2Functions function, string expected)
    {
        Assert.That(IPProtocolProvider.GetMT310s2FunctionEndpoint(function).ToString(), Is.EqualTo(expected));
    }

    [TestCase(MT310s2Functions.None)]
    [TestCase(MT310s2Functions.DCReferenceMeter1 | MT310s2Functions.DCReferenceMeter2)]
    public void Can_Detect_Invalid_MT310s2_Functions(MT310s2Functions funct)
    {
        Assert.Throws<ArgumentException>(() => IPProtocolProvider.GetMT310s2FunctionEndpoint(funct));
    }

    [TestCase(NBoxRouterTypes.Prime, "192.168.32.18:80")]
    [TestCase(NBoxRouterTypes.G3, "192.168.32.19:80")]
    public void Can_Get_IP_Address_NBox_Router(NBoxRouterTypes type, string expected)
    {
        Assert.That(IPProtocolProvider.GetNBoxRouterEndpoint(type).ToString(), Is.EqualTo(expected));
    }

    [TestCase(NBoxRouterTypes.None)]
    [TestCase(NBoxRouterTypes.Prime | NBoxRouterTypes.G3)]
    public void Can_Detect_Invalid_NBox_Router(NBoxRouterTypes type)
    {
        Assert.Throws<ArgumentException>(() => IPProtocolProvider.GetNBoxRouterEndpoint(type));
    }
}