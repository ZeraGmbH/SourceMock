using System.Net;

using Newtonsoft.Json.Linq;

using SourceMock.Actions.Source;
using SourceMock.Model;

namespace SourceMock.Actions.VeinSource
{
    /// <summary>
    /// Communicates with a ZENUX/Vein Source
    /// </summary>
    public class VeinSource : ISource
    {
        private readonly ILogger<VeinSource> _logger;
        private readonly VeinClient _veinClient;

        public VeinSource(ILogger<VeinSource> logger, VeinClient veinClient)
        {
            _logger = logger;
            _veinClient = veinClient;
        }

        public SourceCapabilities GetCapabilities()
        {
            throw new NotImplementedException();
        }

        public Loadpoint? GetCurrentLoadpoint()
        {
            Loadpoint ret = new();

            var veinResponse = _veinClient.GetLoadpoint();
            // how to act on http statuscode and pass through to api endpoint?
            string zeraJson = veinResponse.Value;

            ret = VeinLoadpointMapper.ConvertToLoadpoint(zeraJson);

            return ret;
        }

        public SourceResult SetLoadpoint(Loadpoint loadpoint)
        {
            JObject veinRequest = VeinLoadpointMapper.ConvertToZeraJson(loadpoint);

            _logger.LogInformation(veinRequest.ToString());

            HttpStatusCode veinStatusCode = _veinClient.SetLoadpoint(veinRequest.ToString());


            return SourceResult.SUCCESS;
        }

        public SourceResult TurnOff()
        {
            throw new NotImplementedException();
        }
    }
}