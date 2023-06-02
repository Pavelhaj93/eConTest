using Newtonsoft.Json;

namespace eContracting.Models.JsonDescriptor
{
    public class DocumentDataBodyHeadModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
