using System;
using MongoDB.Bson;

namespace Contents.Data.Entity
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }
        public DateTime CreatedAt { get => Id.CreationTime; }
    }
}
