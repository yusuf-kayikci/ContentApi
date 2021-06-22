using System.Collections.Generic;
using Contents.Data.Attributes;
using MongoDB.Bson;

namespace Contents.Data.Entity
{
    [BsonCollection("orderedcontents")]
    public class OrderedContent : Document 
    {
        public string Name { get; set; }
        public List<ContentItem> Contents { get; set; }
    }

    public class ContentItem
    {
        public ObjectId Id { get; set; }
        public int OrderValue { get; set; }
    }

}
