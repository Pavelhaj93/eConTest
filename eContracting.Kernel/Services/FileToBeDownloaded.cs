using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace eContracting.Kernel.Services
{
    [Serializable]
    public class FileToBeDownloaded
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Guid { get; set; }
        public string Index { get; set; }
        public string FileNumber { get; set; }
        public string FileName { get; set; }
        public List<Byte> FileContent { get; set; }
    }

}
