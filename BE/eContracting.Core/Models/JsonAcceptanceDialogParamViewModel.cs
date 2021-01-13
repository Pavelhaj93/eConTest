using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Models
{
    public class JsonAcceptanceDialogParamViewModel
    {
        [JsonProperty("title")]
        public readonly string Title;

        [JsonProperty("group")]
        public readonly string Group;

        public JsonAcceptanceDialogParamViewModel(string group, string title)
        {
            this.Group = group;
            this.Title = title;
        }
    }
}
