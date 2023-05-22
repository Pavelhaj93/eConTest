using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class GiftDataParamsModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }
    }
}
