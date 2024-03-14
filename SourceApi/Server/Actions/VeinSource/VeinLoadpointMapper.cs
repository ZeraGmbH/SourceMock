using Newtonsoft.Json.Linq;

using SourceApi.Model;

namespace SourceApi.Actions.VeinSource
{
    /// <summary>
    /// Maps VEIN and SourceAPIs LoadPoint values
    /// </summary>
    public static class VeinLoadpointMapper
    {
        public static JObject ConvertToZeraJson(TargetLoadpoint inLoadpoint)
        {
            JObject ret = new();
            JObject frequency = new();
            frequency["type"] = MapFrequencyModeToType(inLoadpoint.Frequency.Mode);
            frequency["val"] = inLoadpoint.Frequency.Value;

            ret["Frequency"] = frequency;

            bool globalOn = false;
            var i = 0;
            foreach (var phase in inLoadpoint.Phases)
            {
                ret[$"I{i + 1}"] = MapElectricalVectorQuantityToJObject(phase.Current.AcComponent!);
                ret[$"U{i + 1}"] = MapElectricalVectorQuantityToJObject(phase.Voltage.AcComponent!);

                globalOn |= inLoadpoint.Phases[i].Current.On || inLoadpoint.Phases[i].Voltage.On;
                ++i;
            }

            ret["on"] = globalOn;
            return ret;
        }

        public static TargetLoadpoint ConvertToLoadpoint(string zeraJson)
        {
            TargetLoadpoint ret = new();
            JObject json = JObject.Parse(zeraJson);

            ret.Frequency.Mode = MapFrequencyTypeToMode(json["Frequency"]?["type"]?.ToString() ?? "");
            ret.Frequency.Value = json["Frequency"]?["val"]?.ToObject<double>() ?? 0;

            int counter = 0;
            while (json.ContainsKey($"I{counter + 1}"))
            {
                if (ret.Phases.Count <= counter) ret.Phases.Add(new TargetLoadpointPhase());
                ret.Phases[counter].Current.AcComponent!.Rms = json[$"I{counter + 1}"]?["rms"]?.ToObject<double>() ?? 0;
                ret.Phases[counter].Current.AcComponent!.Angle = json[$"I{counter + 1}"]?["angle"]?.ToObject<double>() ?? 0;
                ret.Phases[counter].Current.On = json[$"I{counter + 1}"]?["on"]?.ToObject<bool>() ?? false;
                counter += 1;
            }

            counter = 0;
            while (json.ContainsKey($"U{counter + 1}"))
            {
                if (ret.Phases.Count <= counter) ret.Phases.Add(new TargetLoadpointPhase());
                ret.Phases[counter].Voltage.AcComponent!.Rms = json[$"U{counter + 1}"]?["rms"]?.ToObject<double>() ?? 0;
                ret.Phases[counter].Voltage.AcComponent!.Angle = json[$"U{counter + 1}"]?["angle"]?.ToObject<double>() ?? 0;
                ret.Phases[counter].Voltage.On = json[$"U{counter + 1}"]?["on"]?.ToObject<bool>() ?? false;
                counter += 1;
            }
            return ret;
        }

        private static JObject MapElectricalVectorQuantityToJObject(ElectricalVectorQuantity evq)
        {
            JObject ret = new();
            ret["rms"] = evq.Rms;
            ret["angle"] = evq.Angle;
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

        private static FrequencyMode MapFrequencyTypeToMode(string type)
        {
            switch (type)
            {
                case "var":
                    return FrequencyMode.SYNTHETIC;
                case "sync":
                    return FrequencyMode.GRID_SYNCRONOUS;
                default:
                    throw new NotImplementedException($"Unrecognized type: {type}");
            }
        }
    }
}
