using ErrorCalculatorApi.Models;
using MeterTestSystemApi.Models.Configuration;
using MeterTestSystemApi.Models.ConfigurationProviders;
using MeterTestSystemApi.Services;
using MeterTestSystemApi.Services.Probing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ZERA.WebSam.Shared.Actions;
using ZERA.WebSam.Shared.Models;
using ZERA.WebSam.Shared.Models.Logging;

namespace MeterTestSystemApiTests.ComponentConfiguration;

[TestFixture]
public class ProbeTests
{
    private ServiceProvider Services = null!;

    private IProbeConfigurationService Prober = null!;

    private readonly List<ProbingOperation> _Operations = [];

    [SetUp]
    public void Setup()
    {
        _Operations.Clear();

        var services = new ServiceCollection();

        services.AddLogging();

        services.AddSingleton<IProbeConfigurationService, ProbeConfigurationService>();
        services.AddTransient<IConfigurationProbePlan, ConfigurationProbePlan>();
        services.AddTransient<IInterfaceLogger, NoopInterfaceLogger>();

        var storeMock = new Mock<IProbingOperationStore>();

        storeMock.Setup(s => s.AddAsync(It.IsAny<ProbingOperation>())).ReturnsAsync((ProbingOperation op) =>
        {
            _Operations.Add(op);

            return op;
        });

        storeMock.Setup(s => s.UpdateAsync(It.IsAny<ProbingOperation>())).ReturnsAsync((ProbingOperation op) =>
        {
            _Operations[_Operations.FindIndex(o => o.Id == op.Id)] = op;

            return op;
        });

        storeMock.Setup(s => s.Query()).Returns(() =>
        {
            return _Operations.AsQueryable();
        });

        services.AddSingleton(storeMock.Object);

        services.AddSingleton(new Mock<IServerLifetime>().Object);
        services.AddSingleton(new Mock<IActiveOperations>().Object);
        services.AddSingleton(new Mock<IMeterTestSystemConfigurationStore>().Object);

        Services = services.BuildServiceProvider();

        Prober = Services.GetRequiredService<IProbeConfigurationService>();
    }

    private ProbingOperation LatestOperation => _Operations[^1];

    private ProbeConfigurationResult LatestResult => LatestOperation.Result;

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    private Task StartProbeAsync(MeterTestSystemComponentsConfiguration config) => StartProbeAsync(new ProbeConfigurationRequest { Configuration = config });

    private async Task StartProbeAsync(ProbeConfigurationRequest config)
    {
        ((ProbeConfigurationService)Prober).ResetActive();

        await Prober.ConfigureProbingAsync(config, Services);

        var plan = Services.GetRequiredService<IConfigurationProbePlan>();

        await plan.ConfigureProbeAsync();
        await plan.FinishProbeAsync(CancellationToken.None);
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

    private static readonly List<DCComponents> AllDcComponents = Enum.GetValues<DCComponents>().ToList();

    private static readonly List<TransformerComponents> AllTransformerComponents = Enum.GetValues<TransformerComponents>().ToList();

    private static readonly List<MT310s2Functions> AllMT310s2Functions = Enum.GetValues<MT310s2Functions>().ToList();

    private static readonly List<NBoxRouterTypes> AllNBoxRouters = Enum.GetValues<NBoxRouterTypes>().ToList();

    [Test]
    public async Task Can_Create_Probing_Plan_Async()
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration()
        {
            TestPositions = MakeList(TestPositionConfiguration.MaxPosition),
            DCComponents = AllDcComponents,
            TransformerComponents = AllTransformerComponents,
            NBoxRouterTypes = AllNBoxRouters,
            MT310s2Functions = AllMT310s2Functions,
            EnableCOM5003 = true,
            EnableDTS100 = true,
            EnableIPWatchDog = true,
            EnableMP2020Control = true,
            EnableOmegaiBTHX = true,
        });

        Assert.That(Prober.IsActive, Is.True);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(1146));
    }

    [TestCase(0, 21)]
    [TestCase(1, 35)]
    [TestCase(4, 77)]
    [TestCase(80, 1141)]
    public async Task Can_Restrict_Probing_Plan_By_Position_Count_Async(int count, int expected)
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration()
        {
            TestPositions = MakeList(count),
            DCComponents = AllDcComponents,
            MT310s2Functions = AllMT310s2Functions,
            NBoxRouterTypes = AllNBoxRouters,
            TransformerComponents = AllTransformerComponents,
        });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(expected));
    }

    [TestCase(81)]
    public void Can_Detect_Bad_Position_Count(int count)
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => StartProbeAsync(new MeterTestSystemComponentsConfiguration() { TestPositions = MakeList(count) }));
    }

    [TestCase(DCComponents.CurrentSCG06, 1)]
    [TestCase(DCComponents.CurrentSCG1000, 1)]
    [TestCase(DCComponents.CurrentSCG8, 1)]
    [TestCase(DCComponents.CurrentSCG80, 1)]
    [TestCase(DCComponents.FGControl, 1)]
    [TestCase(DCComponents.SPS, 1)]
    [TestCase(DCComponents.VoltageSVG1200, 1)]
    [TestCase(DCComponents.VoltageSVG150, 1)]
    public async Task Can_Restrict_Probing_Plan_By_DC_Component_Async(DCComponents component, int expected)
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { DCComponents = { component } });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(expected));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_DC_Components_Async()
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { DCComponents = { DCComponents.CurrentSCG06, DCComponents.VoltageSVG1200, DCComponents.SPS } });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task Can_Restrict_By_STM_Settings_Async()
    {
        var config = new MeterTestSystemComponentsConfiguration { TestPositions = { new() } };
        var pos = config.TestPositions.Single();

        /* Not enabled. */
        Assert.That(pos.Enabled, Is.False);

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(0));

        /* Enable configuration. */
        pos.Enabled = true;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(0));

        /* Enable direct DUT connection. */
        Assert.That(pos.EnableDirectDutConnection, Is.False);

        pos.EnableDirectDutConnection = true;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(2));

        /* Enable UART interface. */
        Assert.That(pos.EnableUART, Is.False);

        pos.EnableUART = true;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(4));

        /* Enable update server. */
        Assert.That(pos.EnableUpdateServer, Is.False);

        pos.EnableUpdateServer = true;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(6));

        /* Enable COM server. */
        Assert.That(pos.EnableCOMServer, Is.False);

        pos.EnableCOMServer = true;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(7));

        /* Enable backend gateway. */
        Assert.That(pos.EnableBackendGateway, Is.False);

        pos.EnableBackendGateway = true;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(8));

        /* Enable object access. */
        Assert.That(pos.EnableObjectAccess, Is.False);

        pos.EnableObjectAccess = true;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(9));

        /* Enable SIM server 1. */
        Assert.That(pos.EnableSIMServer1, Is.False);

        pos.EnableSIMServer1 = true;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(10));

        /* Enable MAD server. */
        Assert.That(pos.EnableMAD, Is.False);

        pos.EnableMAD = true;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(14));

        /* Forbidden protocol. */
        Assert.That(pos.MadProtocol, Is.Null);

        pos.MadProtocol = (ErrorCalculatorProtocols)33;

        Assert.ThrowsAsync<NotImplementedException>(() => StartProbeAsync(config));

        /* Only old MAD protocol. */
        pos.MadProtocol = ErrorCalculatorProtocols.MAD_1;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(12));

        /* Only STM6000. */
        Assert.That(pos.STMServer, Is.Null);

        pos.STMServer = ServerTypes.STM6000;

        await StartProbeAsync(config);

        Assert.That(LatestResult.Log, Has.Count.EqualTo(8));
    }

    [TestCase(TransformerComponents.STR260Phase1, 1)]
    [TestCase(TransformerComponents.STR260Phase2, 1)]
    [TestCase(TransformerComponents.STR260Phase3, 1)]
    [TestCase(TransformerComponents.CurrentWM3000or1000, 1)]
    [TestCase(TransformerComponents.VoltageWM3000or1000, 1)]
    [TestCase(TransformerComponents.SPS, 1)]
    public async Task Can_Restrict_Probing_Plan_By_Transformer_Components_Async(TransformerComponents components, int expected)
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { TransformerComponents = { components } });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(expected));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_Transformer_Component_Async()
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { TransformerComponents = { TransformerComponents.CurrentWM3000or1000, TransformerComponents.VoltageWM3000or1000 } });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(2));
    }

    [TestCase(NBoxRouterTypes.Prime, 1)]
    [TestCase(NBoxRouterTypes.G3, 1)]
    public async Task Can_Restrict_Probing_Plan_By_NBox_Router_Async(NBoxRouterTypes types, int expected)
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { NBoxRouterTypes = { types } });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(expected));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_NBox_Routers_Async()
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { NBoxRouterTypes = { NBoxRouterTypes.G3, NBoxRouterTypes.Prime } });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_COM5003_Async()
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { EnableCOM5003 = true });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_DTS100_Async()
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { EnableDTS100 = true });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_IP_Watchdog_Async()
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { EnableIPWatchDog = true });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_Omega_iBTHX_Async()
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { EnableOmegaiBTHX = true });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Can_Restrict_Probing_Plan_By_MP2020_Async()
    {
        await StartProbeAsync(new MeterTestSystemComponentsConfiguration() { EnableMP2020Control = true });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Can_Probe_Serial_Ports_Async()
    {
        await StartProbeAsync(new ProbeConfigurationRequest()
        {
            Configuration = { FrequencyGenerator = new(), MT768 = new() },
            SerialPorts = {
                new(){},
                new(){SerialPortTypes.RS232},
                new(){SerialPortTypes.USB},
                new(){SerialPortTypes.RS232,SerialPortTypes.USB},
        }
        });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(8));
    }

    [Test]
    public async Task Can_Probe_HID_Events_Async()
    {
        await StartProbeAsync(new ProbeConfigurationRequest()
        {
            Configuration = { BarcodeReader = 0 },
            HIDEvents = {
                false,
                true,
                false,
                true
            }
        });

        Assert.That(LatestResult.Log, Has.Count.EqualTo(2));
    }
}