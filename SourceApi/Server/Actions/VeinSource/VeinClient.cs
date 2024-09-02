using System.Net;
using System.Net.Http.Headers;

using Newtonsoft.Json.Linq;

namespace SourceApi.Actions.VeinSource
{
    /// <summary>
    /// Connects to VeinAPI to get and set components in Vein Entities
    /// </summary>
    public class VeinClient
    {
        /// <summary>
        /// Creates an instance of VeinClient
        /// </summary>
        /// <param name="client">Injected HttpClient to be used by this instance</param>
        /// <param name="hostIp">Ip which the Vein API listens to</param>
        /// <param name="hostPort">Port on which the Vein API listens</param>
        public VeinClient(HttpClient client, string hostIp, int hostPort)
        {
            _client = client;
            _hostIp = hostIp;
            _hostPort = hostPort;

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Query EntityName of Vein System Entity
        /// </summary>
        /// <returns>String of System Entity Name</returns>
        public async Task<VeinGetResult<string>> GetSystemEntityNameAsync()
        {
            JObject json = await GetFromVeinAsync(EntityIds.SYSTEM, "EntityName");
            VeinGetResult<string> result = new()
            {
                Status = (HttpStatusCode)Int32.Parse(json["status"]?.ToString() ?? ""),
                Value = json["ReturnInformation"]?.ToString() ?? ""
            };
            return result;
        }

        /// <summary>
        /// Set Customer City of Vein Customer Entity
        /// </summary>
        /// <param name="cityName">City Name of Customer</param>
        /// <returns>HttpStatusCode of Request</returns>
        public async Task<HttpStatusCode> SetCustomerCityAsync(string cityName)
        {
            JObject json = await SetToVeinAsync(EntityIds.CUSTOMER_DATA, "PAR_CustomerCity", cityName);

            return (HttpStatusCode)Int32.Parse(json["status"]?.ToString() ?? "");
        }

        /// <summary>
        /// Set Loadpoint of Source
        /// </summary>
        /// <param name="jsonLoadpoint">Loadpoint formated in zera compatible json</param>
        /// <returns>HttpStatusCode of Request</returns>
        public async Task<HttpStatusCode> SetLoadpointAsync(string jsonLoadpoint)
        {
            JObject json = await SetToVeinAsync(EntityIds.SOURCE, "PAR_SourceState0", jsonLoadpoint);

            return (HttpStatusCode)Int32.Parse(json["status"]?.ToString() ?? "");
        }

        /// <summary>
        /// Get current Loadpoint of Source
        /// </summary>
        /// <returns>String of Current Loadpoint in zera compatible json</returns>
        public async Task<VeinGetResult<string>> GetLoadpointAsync()
        {
            JObject json = await GetFromVeinAsync(EntityIds.SOURCE, "PAR_SourceState0");
            VeinGetResult<string> result = new()
            {
                Status = (HttpStatusCode)Int32.Parse(json["status"]?.ToString() ?? ""),
                Value = json["ReturnInformation"]?.ToString() ?? ""
            };
            return result;
        }

        private async Task<JObject> GetFromVeinAsync(EntityIds entityId, string componentName)
        {
            string payload = $"?entity_id={(int)entityId}&component_name={componentName}";

            HttpResponseMessage response = await _client.GetAsync($"http://{_hostIp}:{_hostPort}/api/v1/Vein/{payload}");

            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        private async Task<JObject> SetToVeinAsync(EntityIds entityId, string componentName, string value)
        {
            // Escape double quotes inside "newValue" json-object
            value = value.Replace("\"", "\\\"");

            string payload = $"{{\"EntityID\": {(int)entityId}, \"componentName\": \"{componentName}\", \"newValue\": \"{value}\"}}";
            StringContent stringContent = new(payload);

            HttpResponseMessage response = await _client.PutAsync($"http://{_hostIp}:{_hostPort}/api/v1/Vein/", stringContent);

            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        private readonly HttpClient _client;
        private readonly string _hostIp;
        private readonly int _hostPort;

        private enum EntityIds
        {
            SYSTEM = 0,
            CUSTOMER_DATA = 200,
            SOURCE = 1300
        }

        /// <summary>
        /// Result of GetInfo request to Vein API
        /// </summary>
        /// <typeparam name="T">Type of return value</typeparam>
        public struct VeinGetResult<T>
        {
            /// <summary>
            /// HttpStatusCode of GetInfo response
            /// </summary>
            public HttpStatusCode Status;
            /// <summary>
            /// Return value of GetInfo response
            /// </summary>
            public T Value;
        }

    }
}