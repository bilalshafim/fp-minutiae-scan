using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FP_Engine.ControllerModels
{
    /// <summary>
    /// Object to pass in fp image (serialized byte array) and other related data.
    /// Optionally pass candidate fp image if gotten from OCR reading EID
    /// </summary>
    public class ProbeData
    {
        [JsonPropertyName("probe")]
        [JsonProperty(Required = Required.Always)]
        public string Probe {  get; set; }
        
        [JsonPropertyName("candidate")]
        [JsonProperty(Required = Required.AllowNull)]
        public string Candidate { get; set; }

        [JsonPropertyName("dpi")]
        [JsonProperty(Required = Required.Always)]
        public double Dpi { get; set; }

        [JsonPropertyName("threshold")]
        [JsonProperty(Required = Required.AllowNull)]
        public double Threshold { get; set; }
    }
}
