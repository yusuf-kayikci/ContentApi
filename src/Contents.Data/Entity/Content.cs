using Contents.Data.Attributes;

namespace Contents.Data.Entity
{
    [BsonCollection("contents")]
    public class Content : Document
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
    }
}
