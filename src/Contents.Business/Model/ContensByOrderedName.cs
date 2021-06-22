using System.Collections.Generic;

namespace Contents.Business.Model
{
    public class ContensByOrderedName
    {
        public string OrderedName { get; set; }
        public List<ContentModel> Contents { get; set; }
    }
}
