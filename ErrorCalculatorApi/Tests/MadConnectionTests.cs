using System.Xml;
using ErrorCalculatorApi.Actions.Device;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace ErrorCalculatorApiTests;

[TestFixture]
public class MadConnectionTests
{

    private static XmlDocument LoadXmlFromString(string xml)
    {
        var doc = new XmlDocument();

        doc.LoadXml(xml);

        return doc;
    }

    [Test, Ignore("requires local MAD server running")]
    public async Task Get_Firmware_Version()
    {
        using var cut = new MadTcpConnection(new NullLogger<MadTcpConnection>());

        await cut.Initialize(new()
        {
            Connection = ErrorCalculatorConnectionTypes.TCP,
            Protocol = ErrorCalculatorProtocols.MAD_1,
            Endpoint = "manns1:14207"
        });

        await Task.Delay(4000);

        var res = await cut.Execute(LoadXmlFromString(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKMA.xsd"">
            <!--   MAD_Version.xml -->
            <kmaContainer>
                <kmaVersion>01</kmaVersion>
                <serverVerReq>
                </serverVerReq>
            </kmaContainer>
            </KMA_XML_0_01>  
            "
        ));

        var info = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/serverVerRes");
        var cmd = info?.SelectSingleNode("cmdResCode")?.InnerText?.Trim();
        var version = info?.SelectSingleNode("versionValue")?.InnerText.Trim();
        var model = info?.SelectSingleNode("hwZeraName")?.InnerText.Trim();

        Assert.Multiple(() =>
        {
            Assert.That(cmd, Is.EqualTo("OK"));
            Assert.That(version, Is.EqualTo("1.04"));
            Assert.That(model, Is.EqualTo("SIMULATION"));
        });

    }
}