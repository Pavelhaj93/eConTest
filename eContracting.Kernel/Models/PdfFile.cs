namespace eContracting.Kernel.Models
{
    /// <summary>
    /// Represents a PDF file used for signing.
    /// </summary>
    public class PdfFile
    {
        /// <summary>
        /// Gets or sets a binary data of the file.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Gets or sets a type of the file.
        /// </summary>
        public string FileType { get; set; }
    }
}
