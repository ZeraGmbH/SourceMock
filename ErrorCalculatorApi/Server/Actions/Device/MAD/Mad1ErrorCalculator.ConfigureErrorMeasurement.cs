using ErrorCalculatorApi.Models;
using SharedLibrary.Models.Logging;

namespace ErrorCalculatorApi.Actions.Device.MAD;

partial class Mad1ErrorCalculator
{
    private static readonly string ErrorMeasurementConfigurationXml =
    @"<?xml version=""1.0"" encoding=""UTF-8""?>
      <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKMA.xsd"">
        <!-- MAD_KonfigRichtigkeit.xml -->
        <kmaContainer>
            <kmaVersion>01</kmaVersion>
            <bindErrorCalculatorsReq>
                <envSettings>
                    <devId/>
                    <placeId>##Position##</placeId>
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
    ";

    private static readonly Dictionary<ErrorCalculatorMeterConnections, string> _supportedMeterConnections = new() {
        {ErrorCalculatorMeterConnections.COM1InUART1, "src-com-server-1-in"},
        {ErrorCalculatorMeterConnections.COM1OutUART2, "src-com-server-1-out"},
        {ErrorCalculatorMeterConnections.COM2InUART3, "src-com-server-2-in"},
        {ErrorCalculatorMeterConnections.COM2OutUART4, "src-com-server-2-out"},
        {ErrorCalculatorMeterConnections.Extern1, "src-extern-1"},
        {ErrorCalculatorMeterConnections.Extern2, "src-extern-2"},
        {ErrorCalculatorMeterConnections.FrontBNC,"src-extern-bnc"},
        {ErrorCalculatorMeterConnections.FrontBNC10,"src-extern-bnc-10"},
        {ErrorCalculatorMeterConnections.FrontBNC100,"src-extern-bnc-100"},
        {ErrorCalculatorMeterConnections.FrontBNC1000,"src-extern-bnc-1000"},
        {ErrorCalculatorMeterConnections.Intern1, "src-intern-1"},
        {ErrorCalculatorMeterConnections.Intern2, "src-intern-2"},
        {ErrorCalculatorMeterConnections.NoInput, "src-nc"},
        {ErrorCalculatorMeterConnections.ResetKey, "src-reset-key"},
        {ErrorCalculatorMeterConnections.S0x1, "src-s0-1"},
        {ErrorCalculatorMeterConnections.S0x2, "src-s0-2"},
        {ErrorCalculatorMeterConnections.S0x3, "src-s0-3"},
        {ErrorCalculatorMeterConnections.S0x4, "src-s0-4"},
        {ErrorCalculatorMeterConnections.S0x5, "src-s0-5"},
        {ErrorCalculatorMeterConnections.S0x6, "src-s0-6"},
        {ErrorCalculatorMeterConnections.S0x7, "src-s0-7"},
        {ErrorCalculatorMeterConnections.S0x8, "src-s0-8"},
        {ErrorCalculatorMeterConnections.S0x9, "src-s0-9"},
        {ErrorCalculatorMeterConnections.S0x10, "src-s0-10"},
        {ErrorCalculatorMeterConnections.S0x11, "src-s0-11"},
        {ErrorCalculatorMeterConnections.S0x12, "src-s0-12"},
        {ErrorCalculatorMeterConnections.Software, "src-software"},
    };

    private Task ConfigureErrorMeasurement(IInterfaceLogger logger, ErrorCalculatorMeterConnections? connection)
    {
        /* Create and configure request. */
        var req = LoadXmlFromString(ErrorMeasurementConfigurationXml);

        var source = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/bindErrorCalculatorsReq/fctErrorCalculators/errorCalculator/evaluationSrc")!;
        var place = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/bindErrorCalculatorsReq/envSettings/placeId")!;

        source.InnerText = _supportedMeterConnections[connection ?? ErrorCalculatorMeterConnections.Intern1];
        place.InnerText = $"{_position:00}";

        /* Execute the request. */
        return _connection.Execute(logger, req, "bindErrorCalculatorsRes");
    }
}