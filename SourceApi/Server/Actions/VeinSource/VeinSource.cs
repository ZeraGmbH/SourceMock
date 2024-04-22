using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SharedLibrary.Models.Logging;
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

        public Task CancelDosage(IInterfaceLogger logger)
        {
            throw new NotImplementedException();
        }

        public LoadpointInfo GetActiveLoadpointInfo() => _info;

        public Task<SourceCapabilities> GetCapabilities() => Task.FromException<SourceCapabilities>(new NotImplementedException());

        public TargetLoadpoint? GetCurrentLoadpoint()
        {
            TargetLoadpoint ret = new();

            var veinResponse = _veinClient.GetLoadpoint();
            // how to act on http statuscode and pass through to api endpoint?
            string zeraJson = veinResponse.Value;

            ret = VeinLoadpointMapper.ConvertToLoadpoint(zeraJson);

            return ret;
        }

        public Task<DosageProgress> GetDosageProgress(IInterfaceLogger logger, double meterConstant)
        {
            throw new NotImplementedException();
        }

        public Task SetDosageEnergy(IInterfaceLogger logger, double value, double meterConstant)
        {
            throw new NotImplementedException();
        }

        public Task SetDosageMode(IInterfaceLogger logger, bool on)
        {
            throw new NotImplementedException();
        }

        public Task<SourceApiErrorCodes> SetLoadpoint(IInterfaceLogger logger, TargetLoadpoint loadpoint)
        {
            JObject veinRequest = VeinLoadpointMapper.ConvertToZeraJson(loadpoint);

            _logger.LogInformation(veinRequest.ToString());

            HttpStatusCode veinStatusCode = _veinClient.SetLoadpoint(veinRequest.ToString());


            return Task.FromResult(SourceApiErrorCodes.SUCCESS);
        }

        public Task StartDosage(IInterfaceLogger logger)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CurrentSwitchedOffForDosage(IInterfaceLogger logger)
        {
            throw new NotImplementedException();
        }

        public Task<SourceApiErrorCodes> TurnOff(IInterfaceLogger logger) => Task.FromException<SourceApiErrorCodes>(new NotImplementedException());
    }
}