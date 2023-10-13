using Newtonsoft.Json.Linq;

using SourceMock.Model;

namespace SourceMock.Actions.VeinSource
{
    /// <summary>
    /// Maps VEIN and SourceAPIs LoadPoint values
    /// </summary>
    public static class VeinLoadpointMapper
    {

        public static JObject ConvertToJson(Loadpoint inLoadpoint)
        {
            JObject ret = new();
            JObject frequency = new();
            frequency["type"] = MapFrequencyModeToType(inLoadpoint.Frequency.Mode);
            frequency["val"] = inLoadpoint.Frequency.Value;

            ret["Frequency"] = frequency;

            bool globalOn = false;
            for (int i = 0; i < inLoadpoint.Phases.Count; i++)
            {
                ret[$"I{i + 1}"] = MapElectricalVectorQuantityToJObject(inLoadpoint.Phases[i].Current);
                ret[$"U{i + 1}"] = MapElectricalVectorQuantityToJObject(inLoadpoint.Phases[i].Voltage);

                globalOn |= inLoadpoint.Phases[i].Current.On || inLoadpoint.Phases[i].Voltage.On;
            }

            ret["on"] = globalOn;
            return ret;
        }

        private static JObject MapElectricalVectorQuantityToJObject(ElectricalVectorQuantity evq)
        {
            JObject ret = new();
            ret["rms"] = evq.Rms;
            ret["angle"] = evq.Angle;
            ret["on"] = evq.On;
            return ret;
        }
        private static string MapFrequencyModeToType(FrequencyMode mode)
        {
            switch (mode)
            {
                case FrequencyMode.SYNTHETIC:
                    return "var";
                case FrequencyMode.GRID_SYNCRONOUS:
                    return "sync";
                default:
                    throw new NotImplementedException($"Unrecognized frequency mode: {mode}");
            }
        }
    }
}