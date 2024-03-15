namespace ErrorCalculatorApi.Actions.Device.MAD;

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
    public Task AbortAllJobs()
    {
        return _connection.Execute(LoadXmlFromString(AbortAllJobsXml), "resetRes");
    }
}