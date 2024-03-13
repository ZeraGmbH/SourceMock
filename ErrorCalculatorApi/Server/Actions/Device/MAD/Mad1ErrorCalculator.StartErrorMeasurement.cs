namespace ErrorCalculatorApi.Actions.Device.MAD;

partial class Mad1ErrorCalculator
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly string ErrorMeasurementStartXml =
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
}