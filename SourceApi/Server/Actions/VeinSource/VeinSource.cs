using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

using SourceApi.Actions.Source;
using SourceApi.Model;

namespace SourceApi.Actions.VeinSource
{
    /// <summary>
    /// Communicates with a ZENUX/Vein Source
    /// </summary>
    public class VeinSource : ISource
    {
        private readonly LoadpointInfo _info = new();
        private readonly ILogger<VeinSource> _logger;
        private readonly VeinClient _veinClient;

        public VeinSource(ILogger<VeinSource> logger, VeinClient veinClient)
        {
            _logger = logger;
            _veinClient = veinClient;
        }

        public bool Available => true;

        public Task CancelDosage()
        {
            throw new NotImplementedException();
        }

        public LoadpointInfo GetActiveLoadpointInfo() => _info;

        public Task<SourceCapabilities> GetCapabilities() => Task.FromException<SourceCapabilities>(new NotImplementedException());

        public Loadpoint? GetCurrentLoadpoint()
        {
            Loadpoint ret = new();

            var veinResponse = _veinClient.GetLoadpoint();
            // how to act on http statuscode and pass through to api endpoint?
            string zeraJson = veinResponse.Value;

            ret = VeinLoadpointMapper.ConvertToLoadpoint(zeraJson);

            return ret;
        }

        public Task<DosageProgress> GetDosageProgress()
        {
            throw new NotImplementedException();
        }

        public Task SetDosageEnergy(double value)
        {
            throw new NotImplementedException();
        }

        public Task SetDosageMode(bool on)
        {
            throw new NotImplementedException();
        }

        public Task<SourceApiErrorCodes> SetLoadpoint(Loadpoint loadpoint)
        {
            JObject veinRequest = VeinLoadpointMapper.ConvertToZeraJson(loadpoint);

            _logger.LogInformation(veinRequest.ToString());

            HttpStatusCode veinStatusCode = _veinClient.SetLoadpoint(veinRequest.ToString());


            return Task.FromResult(SourceApiErrorCodes.SUCCESS);
        }

        public Task StartDosage()
        {
            throw new NotImplementedException();
        }

        public Task<bool> CurrentSwitchedOffForDosage()
        {
            throw new NotImplementedException();
        }

        public Task<SourceApiErrorCodes> TurnOff() => Task.FromException<SourceApiErrorCodes>(new NotImplementedException());

        public Task<double[]> GetVoltageRanges()
        {
            throw new NotImplementedException();
        }

        public Task<double[]> GetCurrentRanges()
        {
            throw new NotImplementedException();
        }
    }
}