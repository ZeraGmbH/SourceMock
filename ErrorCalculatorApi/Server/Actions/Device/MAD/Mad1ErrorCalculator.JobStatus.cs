using ErrorCalculatorApi.Models;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;

namespace ErrorCalculatorApi.Actions.Device.MAD;

partial class Mad1ErrorCalculator
{
  private static readonly string JobStatusXml =
  @"<?xml version=""1.0"" encoding=""UTF-8""?>
    <KMA_XML_0_01 xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""KoaLaKNS.xsd"">
      <!-- MAD_JobStatus.xml -->
      <kmaContainer>
          <jobDetails>
              <jobId>##JobId##</jobId>
          </jobDetails>
      </kmaContainer>
    </KMA_XML_0_01>
    ";

  /// <inheritdoc/>
  public async Task<ErrorMeasurementStatus> GetErrorStatusAsync(IInterfaceLogger logger)
  {
    var reply = new ErrorMeasurementStatus { State = ErrorMeasurementStates.NotActive };

    /* Validate. */
    var jobId = _jobId;

    if (string.IsNullOrEmpty(jobId)) return reply;

    /* Create and configure request. */
    var req = LoadXmlFromString(JobStatusXml);

    var id = req.SelectSingleNode("KMA_XML_0_01/kmaContainer/jobDetails/jobId")!;

    id.InnerText = jobId;

    /* Execute the request. */
    var res = await _connection.ExecuteAsync(logger, req, "runErrorMeasureRes");
    var jobInfo = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/jobDetails");

    var info = res.SelectSingleNode("KMA_XML_0_01/kmaContainer/runErrorMeasureRes");
    var errorValues = info?.SelectSingleNode("errorValues");

    /* Set the status. */
    var status = jobInfo?.SelectSingleNode("jobStatusRes")?.InnerText?.Trim();

    reply.State =
      status == "running"
      ? ErrorMeasurementStates.Running
      : status == "closed"
      ? ErrorMeasurementStates.Finished
      : ErrorMeasurementStates.Active;

    /* Set the error value. */
    var error = errorValues?.SelectSingleNode("lastDeviationRaw");

    if (!string.IsNullOrEmpty(error?.InnerText))
      reply.ErrorValue = ParseLong(error.InnerText) / 10000d;

    /* Report counters. */
    var refCounts = errorValues?.SelectSingleNode("lastSeenRefCounts")?.InnerText;
    if (!string.IsNullOrEmpty(refCounts)) reply.ReferenceCounts = new(ParseLong(refCounts));

    var meterCounts = errorValues?.SelectSingleNode("lastSeenMeterCounts")?.InnerText;
    if (!string.IsNullOrEmpty(meterCounts)) reply.MeterCounts = new(ParseLong(meterCounts));

    /* Set the progress. */
    if (status != "running")
    {
      _jobId = null;

      reply.Progress = 100d;
    }
    else if (_dutImpules != null)
    {
      var seen = errorValues?.SelectSingleNode("actSeenCounts")?.InnerText;
      var impulses = string.IsNullOrEmpty(seen) ? (Impulses?)null : new Impulses(ParseLong(seen));

      if (impulses != null) reply.Progress = impulses / _dutImpules * 100d;
    }

    /* Report summary. */
    return reply;
  }
}