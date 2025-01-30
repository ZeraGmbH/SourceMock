using ErrorCalculatorApi.Models;
using ZERA.WebSam.Shared.Models.Logging;

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

  /// <summary>
  /// Try to retrive the current firmware version.
  /// </summary>
  /// <param name="connection">Connection to use.</param>
  /// <param name="logger">Protocol logging sink.</param>
  /// <returns>Version information if available.</returns>
  public static async Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersionAsync(IMadConnection connection, IInterfaceLogger logger)
  {
    /* Execute the request. */
    var res = await connection.ExecuteAsync(logger, LoadXmlFromString(VersionRequestXml), "serverVerRes");

    /* Analyse overall result. */
    var info = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/serverVerRes");

    return new()
    {
      ModelName = info?.SelectSingleNode("hwZeraName")?.InnerText.Trim() ?? string.Empty,
      Version = info?.SelectSingleNode("versionValue")?.InnerText.Trim() ?? string.Empty
    };
  }

  /// <inheritdoc/>
  public Task<ErrorCalculatorFirmwareVersion> GetFirmwareVersionAsync(IInterfaceLogger logger) => GetFirmwareVersionAsync(_connection, logger);
}