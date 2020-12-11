namespace eContracting.Storage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UploadGroupOriginalFile
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public int FileId { get; set; }
    }
}
