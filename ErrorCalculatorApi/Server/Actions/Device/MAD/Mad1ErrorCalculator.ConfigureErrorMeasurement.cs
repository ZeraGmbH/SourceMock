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
}