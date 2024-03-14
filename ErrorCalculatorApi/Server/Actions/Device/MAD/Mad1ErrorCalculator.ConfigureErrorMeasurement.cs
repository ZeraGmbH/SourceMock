using ErrorCalculatorApi.Models;

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
    ";

    private static readonly Dictionary<ErrorCalculatorConnections, string> _supportedConnections = new() {
        {ErrorCalculatorConnections.COM1InUART1, "src-com-server-1-in"},
        {ErrorCalculatorConnections.COM1OutUART2, "src-com-server-1-out"},
        {ErrorCalculatorConnections.COM2InUART3, "src-com-server-2-in"},
        {ErrorCalculatorConnections.COM2OutUART4, "src-com-server-2-out"},
        {ErrorCalculatorConnections.Extern1, "src-extern-1"},
        {ErrorCalculatorConnections.Extern2, "src-extern-2"},
        {ErrorCalculatorConnections.FrontBNC,"src-extern-bnc"},
        {ErrorCalculatorConnections.Intern1, "src-intern-1"},
        {ErrorCalculatorConnections.Intern2, "src-intern-2"},
        {ErrorCalculatorConnections.NoInput, "src-nc"},
        {ErrorCalculatorConnections.RefMeter1, "src-ref-1"},
        {ErrorCalculatorConnections.RefMeter2, "src-ref-2"},
        {ErrorCalculatorConnections.RefMeter3, "src-ref-3"},
        {ErrorCalculatorConnections.ResetKey, "src-reset-key"},
        {ErrorCalculatorConnections.S0x1, "src-s0-1"},
        {ErrorCalculatorConnections.S0x2, "src-s0-2"},
        {ErrorCalculatorConnections.S0x3, "src-s0-3"},
        {ErrorCalculatorConnections.S0x4, "src-s0-4"},
        {ErrorCalculatorConnections.S0x5, "src-s0-5"},
        {ErrorCalculatorConnections.S0x6, "src-s0-6"},
        {ErrorCalculatorConnections.S0x7, "src-s0-7"},
        {ErrorCalculatorConnections.S0x8, "src-s0-8"},
        {ErrorCalculatorConnections.S0x9, "src-s0-9"},
        {ErrorCalculatorConnections.S0x10, "src-s0-10"},
        {ErrorCalculatorConnections.S0x11, "src-s0-11"},
        {ErrorCalculatorConnections.S0x12, "src-s0-12"},
        {ErrorCalculatorConnections.Software, "src-software"},
    };

    private Task ConfigureErrorMeasurement(ErrorCalculatorConnections? connection)
    {
        /* Create and configure request. */
        var req = LoadXmlFromString(ErrorMeasurementConfigurationXml);

        var source = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/bindErrorCalculatorsReq/fctErrorCalculators/errorCalculator/evaluationSrc")!;

        source.InnerText = _supportedConnections[connection ?? ErrorCalculatorConnections.Intern1];

        /* Execute the request. */
        return _connection.Execute(req, "bindErrorCalculatorsRes");
    }
}