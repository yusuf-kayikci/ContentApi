using System.Collections.Generic;

namespace Contents.Data.Entity
{
    public class LookedUpOrderedContent : OrderedContent
    {
        public List<Content> ContentDocs { get; set; }
    }
}
