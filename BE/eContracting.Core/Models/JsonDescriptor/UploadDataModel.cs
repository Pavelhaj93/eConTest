namespace eContracting.Models.JsonDescriptor
{
    public class UploadDataModel : IDataModel
    {
        public string Type { get; set; }
        public int Position { get; set; }
        public IDataHeaderModel Header { get; set; }
        public IDataBodyModel Body { get; set; }
    }
}
