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
        });

    }

    [Test]
    public async Task Can_Create_Probing_Plan()
    {
        await Prober.StartProbe(new()
        {
            TestPositions = MakeList(TestPositionConfiguration.MaxPosition),
            DCComponents = DCComponents.All
        }, true);

        Assert.That(Prober.IsActive, Is.False);
        Assert.That(Prober.Result, Is.Not.Null);

        Assert.That(Prober.Result.Log, Has.Count.EqualTo(1146));
    }

    [TestCase(0, 26)]
    [TestCase(1, 40)]
    [TestCase(4, 82)]
    [TestCase(80, 1146)]
    public async Task Can_Restrict_Probing_Plan_By_Position_Count(int count, int expected)
    {
        await Prober.StartProbe(new()
        {
            TestPositions = MakeList(count),
            DCComponents = DCComponents.All
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

    [TestCase(DCComponents.None, 17)]
    [TestCase(DCComponents.CurrentSCG06, 18)]
    [TestCase(DCComponents.CurrentSCG1000, 18)]
    [TestCase(DCComponents.CurrentSCG8, 18)]
    [TestCase(DCComponents.CurrentSCG80, 18)]
    [TestCase(DCComponents.FGControl, 18)]
    [TestCase(DCComponents.SPS, 18)]
    [TestCase(DCComponents.VoltageSVG1200, 18)]
    [TestCase(DCComponents.VoltageSVG150, 18)]
    [TestCase(DCComponents.CurrentSCG06 | DCComponents.VoltageSVG1200 | DCComponents.SPS, 20)]
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

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(17));

        /* Enable configuration. */
        pos.Enabled = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(17));

        /* Enable direct DUT connection. */
        Assert.That(pos.EnableDirectDutConnection, Is.False);

        pos.EnableDirectDutConnection = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(19));

        /* Enable UART interface. */
        Assert.That(pos.EnableUART, Is.False);

        pos.EnableUART = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(21));

        /* Enable update server. */
        Assert.That(pos.EnableUpdateServer, Is.False);

        pos.EnableUpdateServer = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(23));

        /* Enable COM server. */
        Assert.That(pos.EnableCOMServer, Is.False);

        pos.EnableCOMServer = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(24));

        /* Enable backend gateway. */
        Assert.That(pos.EnableBackendGateway, Is.False);

        pos.EnableBackendGateway = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(25));

        /* Enable object access. */
        Assert.That(pos.EnableObjectAccess, Is.False);

        pos.EnableObjectAccess = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(26));

        /* Enable SIM server 1. */
        Assert.That(pos.EnableSIMServer1, Is.False);

        pos.EnableSIMServer1 = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(27));

        /* Enable MAD server. */
        Assert.That(pos.EnableMAD, Is.False);

        pos.EnableMAD = true;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(31));

        /* Forbidden protocol. */
        Assert.That(pos.MadProtocol, Is.Null);

        pos.MadProtocol = (ErrorCalculatorProtocols)33;

        Assert.ThrowsAsync<NotImplementedException>(() => Prober.StartProbe(config, true));

        /* Only old MAD protocol. */
        pos.MadProtocol = ErrorCalculatorProtocols.MAD_1;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(29));

        /* Only STM6000. */
        Assert.That(pos.STMServer, Is.Null);

        pos.STMServer = ServerTypes.STM6000;

        await Prober.StartProbe(config, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(25));
    }

    [TestCase(TransformerComponents.None, 11)]
    [TestCase(TransformerComponents.STR260Phase1, 12)]
    [TestCase(TransformerComponents.STR260Phase2, 12)]
    [TestCase(TransformerComponents.STR260Phase3, 12)]
    [TestCase(TransformerComponents.CurrentWM3000or1000, 12)]
    [TestCase(TransformerComponents.VoltageWM3000or1000, 12)]
    [TestCase(TransformerComponents.SPS, 12)]
    [TestCase(TransformerComponents.CurrentWM3000or1000 | TransformerComponents.VoltageWM3000or1000, 13)]
    public async Task Can_Restrict_Probing_Plan_By_Transformer_Components(TransformerComponents components, int expected)
    {
        await Prober.StartProbe(new() { TransformerComponents = components }, true);

        Assert.That(Prober.Result!.Log, Has.Count.EqualTo(expected));
    }
}