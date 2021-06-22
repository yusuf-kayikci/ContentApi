using System.Collections.Generic;
using Contents.Business.Model;

namespace Contents.Api.Model.Request
{
    public class OrderedContentPost
    {
        public List<ContentSaveModel> Data { get; set; }
    }
}
