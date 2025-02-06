using ZERA.WebSam.Shared.Models.Logging;

namespace ZeraDevices.ErrorCalculator.STM;

partial class Mad1ErrorCalculator
{
  private static readonly string AbortAllJobsXml =
  @"<?xml version=""1.0"" encoding=""UTF-8""?>
      <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKMA.xsd"">
        <!-- MAD_AllJobsAbort.xml -->
        <kmaContainer>
            <kmaVersion>01</kmaVersion>
            <resetReq>
            </resetReq>
        </kmaContainer>
      </KMA_XML_0_01>
    ";

  /// <inheritdoc/>
  public Task AbortAllJobsAsync(IInterfaceLogger logger)
  {
    return _connection.ExecuteAsync(logger, LoadXmlFromString(AbortAllJobsXml), "resetRes");
  }
}