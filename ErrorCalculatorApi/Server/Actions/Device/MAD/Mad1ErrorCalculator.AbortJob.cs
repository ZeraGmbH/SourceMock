namespace ErrorCalculatorApi.Actions.Device.MAD;

partial class Mad1ErrorCalculator
{
  /// <summary>
  /// 
  /// </summary>
  public static readonly string AbortJobXml =
  @"<?xml version=""1.0"" encoding=""UTF-8""?>
    <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKNS.xsd"">
    <!-- MAD_JobAbort.xml -->
    <kmaContainer>
      <jobDetails>
        <jobId>##JobId##</jobId>
        <jobAbort>true</jobAbort>
      </jobDetails>
    </kmaContainer>
    </KMA_XML_0_01>
  ";
}