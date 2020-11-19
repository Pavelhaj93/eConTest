using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace eContracting.Website.Areas.eContracting2.Models
{
    public class FilesSectionViewModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("files")]
        public IEnumerable<FileViewModel> Files { get; set; }

        public FilesSectionViewModel()
        {
        }

        public FilesSectionViewModel(IEnumerable<FileViewModel> files, string title)
        {
            this.Files = files;
            this.Title = title;
        }
    }
}
