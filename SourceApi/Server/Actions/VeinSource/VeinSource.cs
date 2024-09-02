using Microsoft.Extensions.Logging;
using ZERA.WebSam.Shared.DomainSpecific;
using ZERA.WebSam.Shared.Models.Logging;
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

        public Task<bool> GetAvailableAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(true);

        public Task CancelDosageAsync(IInterfaceLogger logger)
        {
            throw new NotImplementedException();
        }

        public Task<LoadpointInfo> GetActiveLoadpointInfoAsync(IInterfaceLogger interfaceLogger) => Task.FromResult(_info);

        public Task<SourceCapabilities> GetCapabilitiesAsync(IInterfaceLogger interfaceLogger) => Task.FromException<SourceCapabilities>(new NotImplementedException());

        public Task<DosageProgress> GetDosageProgressAsync(IInterfaceLogger logger, MeterConstant meterConstant) => throw new NotImplementedException();

        public Task SetDosageEnergyAsync(IInterfaceLogger logger, ActiveEnergy value, MeterConstant meterConstant) => throw new NotImplementedException();

        public Task SetDosageModeAsync(IInterfaceLogger logger, bool on) => throw new NotImplementedException();

        /// <inheritdoc/>
        public Task<SourceApiErrorCodes> SetLoadpointAsync(IInterfaceLogger logger, TargetLoadpoint loadpoint)
        {
            var veinRequest = VeinLoadpointMapper.ConvertToZeraJson(loadpoint);

            _logger.LogInformation(veinRequest.ToString());

            return Task.FromResult(SourceApiErrorCodes.SUCCESS);
        }

        public Task StartDosageAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task<bool> CurrentSwitchedOffForDosageAsync(IInterfaceLogger logger) => throw new NotImplementedException();

        public Task<SourceApiErrorCodes> TurnOffAsync(IInterfaceLogger logger) => Task.FromException<SourceApiErrorCodes>(new NotImplementedException());

        public async Task<TargetLoadpoint?> GetCurrentLoadpointAsync(IInterfaceLogger logger)
            => VeinLoadpointMapper.ConvertToLoadpoint((await _veinClient.GetLoadpointAsync()).Value);
    }
}