using Contents.Data.Attributes;

namespace Contents.Data.Entity
{
    [BsonCollection("sequence")]
    public class Sequence : Document
    {
        public string Name { get; set; }
        public long Value { get; set; }
    }
}
