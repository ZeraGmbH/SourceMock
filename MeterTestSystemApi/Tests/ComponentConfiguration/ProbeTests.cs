using ErrorCalculatorApi.Models;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using MeterTestSystemApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MeterTestSystemApiTests.ComponentConfiguration;

[TestFixture]
public class ProbeTests
{
    private ServiceProvider Services = null!;

    private IProbeConfigurationService Prober = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IProbeConfigurationService, ProbeConfigurationService>();

        Services = services.BuildServiceProvider();

        Prober = Services.GetRequiredService<IProbeConfigurationService>();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    private static List<TestPositionConfiguration> MakeList(int count) => Enumerable.Range(0, count).Select(_ => new TestPositionConfiguration
    {
        Enabled = true,
        EnableBackendGateway = true,
        EnableCOMServer = true,
        EnableDirectDutConnection = true,
        EnableMAD = true,
        EnableObjectAccess = true,
        EnableSIMServer1 = true,
        EnableUART = true,
        EnableUpdateServer = true,
    }).ToList();

    [Test]
    public void Flag_Enums_All_Provide_Special_Selectors()
    {
        Assert.Multiple(() =>
        {
            Assert.That((int)DCComponents.None, Is.EqualTo(0));
            Assert.That((int)DCComponents.All, Is.EqualTo(0x1ff));

            Assert.That((int)TransformerComponents.None, Is.EqualTo(0));
            Assert.That((int)TransformerComponents.All, Is.EqualTo(0x3f));

            Assert.That((int)NBoxRouterTypes.None, Is.EqualTo(0));
            Assert.That((int)NBoxRouterTypes.All, Is.EqualTo(0x3));

            Assert.That((int)MT310s2Functions.None, Is.EqualTo(0));
            Assert.That((int)MT310s2Functions.All, Is.EqualTo(0x1f));
        });
    }

    [Test]
    public async Task Can_Create_Probing_Plan()
    {
        await Prober.StartProbe(new()
        {
            TestPositions = MakeList(TestPositionConfiguration.MaxPosition),
            DCComponents = DCComponents.All,
            TransformerComponents = TransformerComponents.All,
            NBoxRouterTypes = NBoxRouterTypes.All,
            MT310s2Functions = MT310s2Functions.All,
            EnableCOM5003 = true,
            EnableDTS100 = true,
            EnableIPWatchDog = true,
            EnableMP2020Control = true,
            EnableOmegaiBTHX = true,
        }, true);

        Assert.That(Prober.IsActive, Is.False);
        Assert.That(Prober.Result, Is.Not.Null);

        Assert.That(Prober.Result.Log, Has.Count.EqualTo(1146));
    }

    [TestCase(0, 21)]
    [TestCase(1, 35)]
    [TestCase(4, 77)]
    [TestCase(80, 1141)]
    public async Task Can_Restrict_Probing_Plan_By_Position_Count(int count, int expected)
    {
        await Prober.StartProbe(new()
        {
            TestPositions = MakeList(count),
            DCComponents = DCComponents.All,
            MT310s2Functions = MT310s2Functions.All,
            NBoxRouterTypes = NBoxRouterTypes.All,
            TransformerComponents = TransformerComponents.All,
        }, true);

        Assert.That(Prober.IsActive, Is.False);
        Assert.That(Prober.Result, Is.Not.Null);

        Assert.That(Prober.Result.Log, Has.Count.EqualTo(expected));
    }

    [TestCase(81)]
    public void Can_Detect_Bad_Position_Count(int count)
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Prober.StartProbe(new() { TestPositions = MakeList(count) }, true));
    }

    [TestCase(DCComponents.None, 0)]
    [TestCase(DCComponents.CurrentSCG06, 1)]
    [TestCase(DCComponents.CurrentSCG1000, 1)]
    [TestCase(DCComponents.CurrentSCG8, 1)]
    [TestCase(DCComponents.CurrentSCG80, 1)]
    [TestCase(DCComponents.FGControl, 1)]
    [TestCase(DCComponents.SPS, 1)]
    [TestCase(DCComponents.VoltageSVG1200, 1)]
    [TestCase(DCComponents.VoltageSVG150, 1)]
    [TestCase(DCComponents.CurrentSCG06 | DCComponents.VoltageSVG1200 | DCComponents.SPS, 3)]
    [TestCase(DCComponents.All, 9)]
    public async Task Can_Restrict_Probing_Plan_By_DC_Components(DCComponents components, int expected)
    {
        await Prober.StartProbe(new() { DCComponents = components }, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(expected));
    }

    [Test]
    public async Task Can_Restrict_By_STM_Settings()
    {
        var config = new MeterTestSystemComponentsConfiguration { TestPositions = { new() } };
        var pos = config.TestPositions.Single();

        /* Not enabled. */
        Assert.That(pos.Enabled, Is.False);

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(0));

        /* Enable configuration. */
        pos.Enabled = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(0));

        /* Enable direct DUT connection. */
        Assert.That(pos.EnableDirectDutConnection, Is.False);

        pos.EnableDirectDutConnection = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(2));

        /* Enable UART interface. */
        Assert.That(pos.EnableUART, Is.False);

        pos.EnableUART = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(4));

        /* Enable update server. */
        Assert.That(pos.EnableUpdateServer, Is.False);

        pos.EnableUpdateServer = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(6));

        /* Enable COM server. */
        Assert.That(pos.EnableCOMServer, Is.False);

        pos.EnableCOMServer = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(7));

        /* Enable backend gateway. */
        Assert.That(pos.EnableBackendGateway, Is.False);

        pos.EnableBackendGateway = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(8));

        /* Enable object access. */
        Assert.That(pos.EnableObjectAccess, Is.False);

        pos.EnableObjectAccess = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(9));

        /* Enable SIM server 1. */
        Assert.That(pos.EnableSIMServer1, Is.False);

        pos.EnableSIMServer1 = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(10));

        /* Enable MAD server. */
        Assert.That(pos.EnableMAD, Is.False);

        pos.EnableMAD = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(14));

        /* Forbidden protocol. */
        Assert.That(pos.MadProtocol, Is.Null);

        pos.MadProtocol = (ErrorCalculatorProtocols)33;

        Assert.ThrowsAsync<NotImplementedException>(() => Prober.StartProbe(config, true));

        /* Only old MAD protocol. */
        pos.MadProtocol = ErrorCalculatorProtocols.MAD_1;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(12));

        /* Only STM6000. */
        Assert.That(pos.STMServer, Is.Null);

        pos.STMServer = ServerTypes.STM6000;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(8));
    }

    [TestCase(TransformerComponents.None, 0)]
    [TestCase(TransformerComponents.STR260Phase1, 1)]
    [TestCase(TransformerComponents.STR260Phase2, 1)]
    [TestCase(TransformerComponents.STR260Phase3, 1)]
    [TestCase(TransformerComponents.CurrentWM3000or1000, 1)]
    [TestCase(TransformerComponents.VoltageWM3000or1000, 1)]
    [TestCase(TransformerComponents.SPS, 1)]
    [TestCase(TransformerComponents.CurrentWM3000or1000 | TransformerComponents.VoltageWM3000or1000, 2)]
    [TestCase(TransformerComponents.All, 6)]
    public async Task Can_Restrict_Probing_Plan_By_Transformer_Components(TransformerComponents components, int expected)
    {
        await Prober.StartProbe(new() { TransformerComponents = components }, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(expected));
    }


    [TestCase(NBoxRouterTypes.None, 0)]
    [TestCase(NBoxRouterTypes.Prime, 1)]
    [TestCase(NBoxRouterTypes.G3, 1)]
    [TestCase(NBoxRouterTypes.G3 | NBoxRouterTypes.Prime, 2)]
    [TestCase(NBoxRouterTypes.All, 2)]
    public async Task Can_Restrict_Probing_Plan_By_NBox_Router(NBoxRouterTypes types, int expected)
    {
        await Prober.StartProbe(new() { NBoxRouterTypes = types }, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(expected));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_COM5003()
    {
        await Prober.StartProbe(new() { EnableCOM5003 = true }, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_DTS100()
    {
        await Prober.StartProbe(new() { EnableDTS100 = true }, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_IP_Watchdog()
    {
        await Prober.StartProbe(new() { EnableIPWatchDog = true }, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_Omega_iBTHX()
    {
        await Prober.StartProbe(new() { EnableOmegaiBTHX = true }, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_MP2020()
    {
        await Prober.StartProbe(new() { EnableMP2020Control = true }, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(1));
    }
}