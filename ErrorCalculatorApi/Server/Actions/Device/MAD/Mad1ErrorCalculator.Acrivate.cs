namespace ErrorCalculatorApi.Actions.Device.MAD;

partial class Mad1ErrorCalculator
{
  private static readonly string ActivateXml =
  @"<?xml version=""1.0"" encoding=""UTF-8""?>
    <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKMA.xsd"">
      <!-- MAD_MPActivate.xml -->
      <kmaContainer>
        <kmaVersion>01</kmaVersion>
        <runKodisAccessReq>
          <envSettings>
            <devId></devId>
            <placeId>##MeterPlace##</placeId>
            <tOutUntilStateHasChanged>500</tOutUntilStateHasChanged>
            <logPrefix></logPrefix>
            <logPath></logPath>
            <logDebugMask>0x0001</logDebugMask>
          </envSettings>
          <fctSetToState>
            <amvRelais>##mpactive##</amvRelais>
            <amvMask>
              <line1>##mpactive_L1##</line1>
              <line2>##mpactive_L2##</line2>
              <line3>##mpactive_L3##</line3>
              <lineN>##mpactive_N##</lineN>
            </amvMask>
          </fctSetToState>
        </runKodisAccessReq>
      </kmaContainer>
    </KMA_XML_0_01>
  ";

  /// <inheritdoc />
  public Task ActivateSource(bool on)
  {
    /* Create and configure request. */
    var req = LoadXmlFromString(ActivateXml);

    var place = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runKodisAccessReq/envSettings/placeId")!;

    place.InnerText = $"{_position:00}";

    var relais = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runKodisAccessReq/fctSetToState/amvRelais")!;
    var line1 = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runKodisAccessReq/fctSetToState/amvMask/line1")!;
    var line2 = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runKodisAccessReq/fctSetToState/amvMask/line2")!;
    var line3 = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runKodisAccessReq/fctSetToState/amvMask/line3")!;
    var lineN = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/runKodisAccessReq/fctSetToState/amvMask/lineN")!;

    var data = on ? "true" : "false";

    relais.InnerText = data;
    line1.InnerText = data;
    line2.InnerText = data;
    line3.InnerText = data;
    lineN.InnerText = data;

    /* Execute the request. */
    return _connection.Execute(req, "runKodisAccessRes");
  }
}