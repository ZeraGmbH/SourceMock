using System.Globalization;
using System.Xml;
using ErrorCalculatorApi.Actions.Device.MAD;
using ErrorCalculatorApi.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace ErrorCalculatorApiTests;

[TestFixture]
public class MadConnectionTests
{
    private string MADServer = null!;

    [SetUp]
    public void Setup()
    {
        MADServer = Environment.GetEnvironmentVariable("EXECUTE_KOALA_MAD_TESTS")!;

        if (string.IsNullOrEmpty(MADServer))
            Assert.Ignore("not running MAD tests");
    }

    private static XmlDocument LoadXmlFromString(string xml)
    {
        var doc = new XmlDocument();

        doc.LoadXml(xml);

        return doc;
    }

    [Test]
    public async Task Get_Firmware_Version()
    {
        using var cut = new MadTcpConnection(new NullLogger<MadTcpConnection>());

        await cut.Initialize(new()
        {
            Connection = ErrorCalculatorConnectionTypes.TCP,
            Protocol = ErrorCalculatorProtocols.MAD_1,
            Endpoint = $"{MADServer}:14207"
        });

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
        ), "serverVerRes");

        var info = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/serverVerRes");
        var version = info?.SelectSingleNode("versionValue")?.InnerText.Trim();
        var model = info?.SelectSingleNode("hwZeraName")?.InnerText.Trim();

        Assert.Multiple(() =>
        {
            Assert.That(version, Is.EqualTo("1.04"));
            Assert.That(model, Is.EqualTo("SIMULATION"));
        });
    }

    [Test]
    public async Task Run_Error_Measurement()
    {
        await ConfigureErrorMeasurement();

        var jobId = await StartErrorMeasurement();

        for (var i = 10; i-- > 0;)
            TestContext.WriteLine(await ReadJobStatus(jobId, false));

        for (var i = 0; ; i = 1)
        {
            var status = await ReadJobStatus(jobId, i == 0);

            TestContext.WriteLine(status);

            if (status.Item1 != "running") break;
        }
    }

    private async Task ConfigureErrorMeasurement()
    {
        using var cut = new MadTcpConnection(new NullLogger<MadTcpConnection>());

        await cut.Initialize(new()
        {
            Connection = ErrorCalculatorConnectionTypes.TCP,
            Protocol = ErrorCalculatorProtocols.MAD_1,
            Endpoint = $"{MADServer}:14207"
        });

        var req = LoadXmlFromString(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
            <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKMA.xsd"">
                <!-- MAD_KonfigRichtigkeit.xml -->
                <kmaContainer>
                    <kmaVersion>01</kmaVersion>
                    <bindErrorCalculatorsReq>
                        <envSettings>
                            <devId/>
                            <placeId>00</placeId>
                            <tOutUntilStateHasChanged>500</tOutUntilStateHasChanged>
                            <logPrefix/>
                            <logPath/>
                            <logDebugMask>0x0000</logDebugMask>
                        </envSettings>
                        <fctErrorCalculators>
                            <errorCalculator>
                                <useSrcPrecedence>true</useSrcPrecedence>
                                <measuringModule>1</measuringModule>
                                <evaluationSrc>##Source##</evaluationSrc>
                                <referenceSrc>src-ref-1</referenceSrc>
                                <runMode>errorMeasure</runMode>
                                <highActive>true</highActive>
                                <minPulseDuration>5</minPulseDuration>
                                <useLocalDisplay>true</useLocalDisplay>
                                <showProgressBar>true</showProgressBar>
                                <useLocalResetKey>true</useLocalResetKey>
                                <useDisplayReverseSign>false</useDisplayReverseSign>
                                <setDisplayFractionalSize>2</setDisplayFractionalSize>
                                <setDisplayEquipmentAccuracy>200</setDisplayEquipmentAccuracy>
                                <setDisplayAddedText>SC</setDisplayAddedText>
                                <setDisplayDecPoint>,</setDisplayDecPoint>
                            </errorCalculator>
                        </fctErrorCalculators>
                    </bindErrorCalculatorsReq>
                </kmaContainer>
            </KMA_XML_0_01>
            ");

        var source = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/bindErrorCalculatorsReq/fctErrorCalculators/errorCalculator/evaluationSrc")!;

        source.InnerText = "src-intern-1";

        await cut.Execute(req, "bindErrorCalculatorsRes");
    }

    private async Task<string> StartErrorMeasurement()
    {
        using var cut = new MadTcpConnection(new NullLogger<MadTcpConnection>());

        await cut.Initialize(new()
        {
            Connection = ErrorCalculatorConnectionTypes.TCP,
            Protocol = ErrorCalculatorProtocols.MAD_1,
            Endpoint = $"{MADServer}:14207"
        });

        var req = LoadXmlFromString(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
              <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKMA.xsd"">
                <!-- MAD_StartRichtigkeit.xml -->
                <kmaContainer>
                    <kmaVersion>01</kmaVersion>
                    <runErrorMeasureReq>
                        <envSettings>
                            <devId></devId>
                            <tOutUntilStateHasChanged>500</tOutUntilStateHasChanged>
                            <logPrefix></logPrefix>
                            <logPath></logPath>
                            <logDebugMask>0x0001</logDebugMask>
                            <jobReplacesOther>false</jobReplacesOther>
                        </envSettings>
                        <fctErrorMeasurement>
                            <evaluationSrc>##Source##</evaluationSrc>
                            <runMode>##Continuous##</runMode>
                            <metrologicalCounts>##MeterCounts##</metrologicalCounts>
                            <referenceCounts>##ReferenceCounts##</referenceCounts>
                            <offsetPPM>0</offsetPPM>
                            <upperTolerancePPM>999900</upperTolerancePPM>
                            <lowerTolerancePPM>-999900</lowerTolerancePPM>
                        </fctErrorMeasurement>
                    </runErrorMeasureReq>
                </kmaContainer>
              </KMA_XML_0_01>
            ");

        var dutCounts = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureReq/fctErrorMeasurement/metrologicalCounts")!;
        var mode = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureReq/fctErrorMeasurement/runMode")!;
        var refCounts = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureReq/fctErrorMeasurement/referenceCounts")!;
        var source = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureReq/fctErrorMeasurement/evaluationSrc")!;

        dutCounts.InnerText = "3";
        mode.InnerText = "repetition";
        refCounts.InnerText = "180000";
        source.InnerText = "src-intern-1";

        var res = await cut.Execute(req, "runErrorMeasureRes");

        var jobInfo = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/jobDetails");
        var jobId = jobInfo?.SelectSingleNode("jobId")?.InnerText?.Trim();
        var status = jobInfo?.SelectSingleNode("jobStatusRes")?.InnerText?.Trim();

        Assert.Multiple(() =>
        {
            Assert.That(jobId, Is.Not.Empty);
            Assert.That(status, Is.EqualTo("running"));
        });

        return jobId;
    }

    private async Task<Tuple<string, int?, int?, double?>> ReadJobStatus(string jobId, bool abort)
    {
        using var cut = new MadTcpConnection(new NullLogger<MadTcpConnection>());

        await cut.Initialize(new()
        {
            Connection = ErrorCalculatorConnectionTypes.TCP,
            Protocol = ErrorCalculatorProtocols.MAD_1,
            Endpoint = $"{MADServer}:14207"
        });

        var req = LoadXmlFromString(
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
              <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKNS.xsd"">
                <!-- MAD_JobStatus.xml -->
                <kmaContainer>
                    <jobDetails>
                        <jobId>##JobId##</jobId>
                        <jobAbort>##Abort##</jobAbort>
                    </jobDetails>
                </kmaContainer>
              </KMA_XML_0_01>
            ");

        var id = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/jobDetails/jobId")!;
        var op = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/jobDetails/jobAbort")!;

        id.InnerText = jobId;
        op.InnerText = abort ? "true" : "false";

        var res = await cut.Execute(req, "runErrorMeasureRes");

        var info = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureRes");

        var jobInfo = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/jobDetails");
        var status = jobInfo?.SelectSingleNode("jobStatusRes")?.InnerText?.Trim();

        Assert.That(status, Is.EqualTo("closed").Or.EqualTo("running"));

        var errorValues = info?.SelectSingleNode("errorValues");
        var seen = errorValues?.SelectSingleNode("actSeenCounts");
        var done = errorValues?.SelectSingleNode("doneMeasures");

        var error = errorValues?.SelectSingleNode("formattedErrorValue");

        return Tuple.Create(
            status!,
            string.IsNullOrEmpty(done?.InnerText) ? null : (int?)int.Parse(done.InnerText),
            string.IsNullOrEmpty(seen?.InnerText) ? null : (int?)int.Parse(seen.InnerText),
            string.IsNullOrEmpty(error?.InnerText) ? null : (double?)double.Parse(error.InnerText, CultureInfo.GetCultureInfo("de"))
        );
    }
}