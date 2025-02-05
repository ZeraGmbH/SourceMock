using ZERA.WebSam.Shared.Models.Source;

namespace SourceApiTests.Actions.DomainSpecificQuantitites;

[TestFixture]
public class LoadpointTests
{
    [Test]
    public void Create_A_Loadpoint()
    {
        var lp = new TargetLoadpoint
        {
            Phases = [
                new(){
                    Current=new(){
                        DcComponent = new(12.4),
                        AcComponent=new(){ Angle=new(113.4), Rms=new(12.1)},
                    }
                }
            ]
        };


#pragma warning disable CS8620
        lp.Phases[0].Current.DcComponent *= 3;
#pragma warning restore CS8620

#pragma warning disable CS8629
        Assert.That((double)lp.Phases[0].Current.DcComponent, Is.EqualTo(37.2));
#pragma warning restore CS8629
    }
}