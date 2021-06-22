using System.Collections.Generic;
using System.Threading.Tasks;
using Contents.Business.Model;
using Contents.Data.Entity;

namespace Contents.Business.Abstraction
{
    public interface IContentService
    {
        Task<ContentSelectResult> GetContentsAsync();
        OrderedContentSelectResult GetContentsByOrderedName(string orderedName);
        Task<ContentUpdateResult> UpdateOrderedContentAsync(string orderedName, List<ContentSaveModel> contentUpdateModel);
        Task<ContentInsertResult> InsertOrderedContentAsync(string orderedName, List<ContentSaveModel> contentInsertModel);
        OrderedContent ReorderContents(OrderedContent orderedContent, List<ContentSaveModel> contentUpdateModel);
    }
}
