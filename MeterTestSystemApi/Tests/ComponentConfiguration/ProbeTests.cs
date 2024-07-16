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
        await Prober.StartProbe(new(), true);

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
        await Prober.StartProbe(new() { NumberOfPositions = count }, true);

        Assert.That(Prober.IsActive, Is.False);
        Assert.That(Prober.Result, Is.Not.Null);

        Assert.That(Prober.Result.Log, Has.Count.EqualTo(expected));
    }

    [TestCase(-1)]
    [TestCase(81)]
    public void Can_Detect_Bad_Position_Count(int count)
    {
        Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => Prober.StartProbe(new() { NumberOfPositions = count }, true));
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
        await Prober.StartProbe(new() { NumberOfPositions = 0, DCComponents = components }, true);

        Assert.That(Prober.IsActive, Is.False);
        Assert.That(Prober.Result, Is.Not.Null);

        Assert.That(Prober.Result.Log, Has.Count.EqualTo(expected));
    }

}