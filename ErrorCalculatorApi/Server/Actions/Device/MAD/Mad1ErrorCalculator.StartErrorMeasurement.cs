using ErrorCalculatorApi.Models;

namespace ErrorCalculatorApi.Actions.Device.MAD;

partial class Mad1ErrorCalculator
{
    private static readonly string ErrorMeasurementStartXml =
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
    ";

    private async Task<string> StartErrorMeasurement(bool continuous, ErrorCalculatorMeterConnections? connection, long dutImpulses, long refMeterImpulses)
    {
        /* Create and configure request. */
        var req = LoadXmlFromString(ErrorMeasurementStartXml);

        var dutCounts = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureReq/fctErrorMeasurement/metrologicalCounts")!;
        var mode = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureReq/fctErrorMeasurement/runMode")!;
        var refCounts = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureReq/fctErrorMeasurement/referenceCounts")!;
        var source = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureReq/fctErrorMeasurement/evaluationSrc")!;

        dutCounts.InnerText = $"{dutImpulses}";
        mode.InnerText = continuous ? "repetition" : "oneShot";
        refCounts.InnerText = $"{refMeterImpulses}";
        source.InnerText = _supportedMeterConnections[connection ?? ErrorCalculatorMeterConnections.Intern1];

        /* Execute the request. */
        var res = await _connection.Execute(req, "runErrorMeasureRes");
        var jobId = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/jobDetails/jobId")?.InnerText;

        if (string.IsNullOrEmpty(jobId)) throw new InvalidOperationException("got no job identifier");

        return jobId;
    }
}