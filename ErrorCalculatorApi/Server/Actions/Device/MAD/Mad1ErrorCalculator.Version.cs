using ErrorCalculatorApi.Models;
using SharedLibrary.Models.Logging;

namespace ErrorCalculatorApi.Actions.Device.MAD;

partial class Mad1ErrorCalculator
{
  private static readonly string VersionRequestXml =
  @"<?xml version=""1.0"" encoding=""UTF-8""?>
    <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKMA.xsd"">
    <!--   MAD_Version.xml -->
    <kmaContainer>
      <kmaVersion>01</kmaVersion>
      <serverVerReq>
      </serverVerReq>
    </kmaContainer>
    </KMA_XML_0_01>
  ";

  /// <inheritdoc/>
  public async Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersion(IInterfaceLogger logger)
  {
    /* Execute the request. */
    var res = await _connection.Execute(logger, LoadXmlFromString(VersionRequestXml), "serverVerRes");

    /* Analyse overall result. */
    var info = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/serverVerRes");

    return new()
    {
      ModelName = info?.SelectSingleNode("hwZeraName")?.InnerText.Trim() ?? string.Empty,
      Version = info?.SelectSingleNode("versionValue")?.InnerText.Trim() ?? string.Empty
    };
  }
}