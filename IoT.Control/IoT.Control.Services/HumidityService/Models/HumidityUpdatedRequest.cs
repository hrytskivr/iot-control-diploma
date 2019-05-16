using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IoT.Control.Services.HumidityService.Models
{
    public class HumidityUpdatedRequest
    {
        [JsonProperty("current_humidity_level")]
        public double CurrentHumidityLevel { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("current_humidifier_state")]
        public HumidifierStates CurrentHumidifierState { get; set; }
    }

    public enum HumidifierStates
    {
        Enabled,
        Disabled
    }
}
