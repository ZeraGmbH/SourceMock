using System.Globalization;
using ErrorCalculatorApi.Models;

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
  public async Task<ErrorMeasurementStatus> GetErrorStatus()
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
    var res = await _connection.Execute(req, "runErrorMeasureRes");
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

    /* Set the error value - we use , as a decimal separator, see start command. */
    var error = errorValues?.SelectSingleNode("formattedErrorValue");

    reply.ErrorValue =
      string.IsNullOrEmpty(error?.InnerText)
      ? null
      : double.Parse(error.InnerText, CultureInfo.GetCultureInfo("de"));

    /* Set the progress. */
    if (status != "running")
    {
      _jobId = null;

      reply.Progress = 100d;
    }
    else
    {
      var seen = errorValues?.SelectSingleNode("actSeenCounts");

      var impulses =
        string.IsNullOrEmpty(seen?.InnerText)
        ? (long?)null
        : long.Parse(seen.InnerText);

      if (impulses != null && _dutImpules != null)
        reply.Progress = impulses * 100.0d / _dutImpules;
    }

    /* Report summary. */
    return reply;
  }
}