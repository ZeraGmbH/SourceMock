using Newtonsoft.Json.Linq;
using SharedLibrary.DomainSpecific;
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
            var ret = new JObject();

            var frequency = new JObject
            {
                ["type"] = MapFrequencyModeToType(inLoadpoint.Frequency.Mode),
                ["val"] = (double)inLoadpoint.Frequency.Value
            };

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
            ret.Frequency.Value = new(json["Frequency"]?["val"]?.ToObject<double>() ?? 0);

            int counter = 0;
            while (json.ContainsKey($"I{counter + 1}"))
            {
                if (ret.Phases.Count <= counter) ret.Phases.Add(new TargetLoadpointPhase());
                ret.Phases[counter].Current.AcComponent!.Rms = new(json[$"I{counter + 1}"]?["rms"]?.ToObject<double>() ?? 0);
                ret.Phases[counter].Current.AcComponent!.Angle = new(json[$"I{counter + 1}"]?["angle"]?.ToObject<double>() ?? 0);
                ret.Phases[counter].Current.On = json[$"I{counter + 1}"]?["on"]?.ToObject<bool>() ?? false;
                counter += 1;
            }

            counter = 0;
            while (json.ContainsKey($"U{counter + 1}"))
            {
                if (ret.Phases.Count <= counter) ret.Phases.Add(new TargetLoadpointPhase());
                ret.Phases[counter].Voltage.AcComponent!.Rms = new(json[$"U{counter + 1}"]?["rms"]?.ToObject<double>() ?? 0);
                ret.Phases[counter].Voltage.AcComponent!.Angle = new(json[$"U{counter + 1}"]?["angle"]?.ToObject<double>() ?? 0);
                ret.Phases[counter].Voltage.On = json[$"U{counter + 1}"]?["on"]?.ToObject<bool>() ?? false;
                counter += 1;
            }
            return ret;
        }

        private static JObject MapElectricalVectorQuantityToJObject<T>(ElectricalVectorQuantity<T> evq) where T : struct, IDomainSpecificNumber<T>
            => new() { ["rms"] = (double)evq.Rms, ["angle"] = (double)evq.Angle };

        private static string MapFrequencyModeToType(FrequencyMode mode)
            => mode switch
            {
                FrequencyMode.SYNTHETIC => "var",
                FrequencyMode.GRID_SYNCRONOUS => "sync",
                _ => throw new NotImplementedException($"Unrecognized frequency mode: {mode}"),
            };

        private static FrequencyMode MapFrequencyTypeToMode(string type)
            => type switch
            {
                "var" => FrequencyMode.SYNTHETIC,
                "sync" => FrequencyMode.GRID_SYNCRONOUS,
                _ => throw new NotImplementedException($"Unrecognized type: {type}"),
            };
    }
}
