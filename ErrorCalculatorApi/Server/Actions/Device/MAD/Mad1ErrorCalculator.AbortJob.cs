using SharedLibrary.Models.Logging;

namespace ErrorCalculatorApi.Actions.Device.MAD;

partial class Mad1ErrorCalculator
{
  private static readonly string AbortJobXml =
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


  /// <inheritdoc/>
  public Task AbortErrorMeasurement(IInterfaceLogger logger)
  {
    /* Validate. */
    var jobId = _jobId;

    _jobId = null;

    if (string.IsNullOrEmpty(jobId)) return Task.CompletedTask;

    /* Create and configure request. */
    var req = LoadXmlFromString(AbortJobXml);

    var id = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/jobDetails/jobId")!;

    id.InnerText = jobId;

    /* Execute the request. */
    return _connection.Execute(logger, req, "runErrorMeasureRes");
  }
}