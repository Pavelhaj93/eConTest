using eContracting.Kernel.GlassItems.Content;

namespace eContracting.Kernel.Models
{
    /// <summary>
    /// RichText additiona model.
    /// </summary>
    public class WelcomeRichTextModel
    {
        /// <summary>
        /// Gets or sets replaced text.
        /// </summary>
        public string ReplacedText { get; set; }

        /// <summary>
        /// Gets or sets a value of the associcated datasource.
        /// </summary>
        public EContractingWelcomeRichTextDatasource Datasource { get; set; }
    }
}
