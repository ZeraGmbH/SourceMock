using SourceApi.Model;

namespace SourceApiTests.Actions.DomainSpecificQuantitites;

[TestFixture]
public class LoadpointTests
{
    [Test]
    public void Create_A_Loadpoint()
    {
        var lp = new TargetLoadpointNGX
        {
            Phases = [
                new(){
                    Current=new(){
                        DcComponent = new(12.4),
                        AcComponent=new(){ Angle=113.4, Rms=new(12.1)},
                    }
                }
            ]
        };

        lp.Phases[0].Current.DcComponent *= 3;

        Assert.That((double)lp.Phases[0].Current.DcComponent, Is.EqualTo(37.2));
    }
}