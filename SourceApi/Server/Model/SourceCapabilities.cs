using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebSamDeviceApis.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class SourceCapabilities
    {
        /// <summary>
        /// 
        /// </summary>
        [NotNull, Required]
        public List<PhaseCapability> Phases { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        [NotNull, Required]
        public List<FrequencyRange> FrequencyRanges { get; set; } = new();
    }
}