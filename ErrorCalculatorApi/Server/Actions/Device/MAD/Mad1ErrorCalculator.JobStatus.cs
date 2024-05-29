using ErrorCalculatorApi.Models;
using SharedLibrary.Models.Logging;

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
  public async Task<ErrorMeasurementStatus> GetErrorStatus(IInterfaceLogger logger)
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
    var res = await _connection.Execute(logger, req, "runErrorMeasureRes");
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
    {
      /* [TODO] add support for 0x HEX response. */
      var rawValue = long.Parse(error.InnerText);

      reply.ErrorValue = rawValue / 10000d;
    }

    /* Set the progress. */
    if (status != "running")
    {
      _jobId = null;

      reply.Progress = 100d;
      reply.CountsAreEnergy = false;

      var refCounts = errorValues?.SelectSingleNode("lastSeenRefCounts")?.InnerText;
      if (!string.IsNullOrEmpty(refCounts)) reply.ReferenceCountsOrEnergy = long.Parse(refCounts);

      var meterCounts = errorValues?.SelectSingleNode("lastSeenMeterCounts")?.InnerText;
      if (!string.IsNullOrEmpty(meterCounts)) reply.MeterCountsOrEnergy = long.Parse(meterCounts);
    }
    else
    {
      var seen = errorValues?.SelectSingleNode("actSeenCounts")?.InnerText;

      /* [TODO] add support for 0x HEX response. */
      var impulses = string.IsNullOrEmpty(seen) ? (long?)null : long.Parse(seen);

      if (impulses != null && _dutImpules != null)
        reply.Progress = impulses * 100.0d / _dutImpules;
    }

    /* Report summary. */
    return reply;
  }
}