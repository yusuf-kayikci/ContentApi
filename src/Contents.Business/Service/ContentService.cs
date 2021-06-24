using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contents.Business.Abstraction;
using Contents.Business.Model;
using Contents.Common.Exceptions;
using Contents.Data.Abstraction;
using Contents.Data.Entity;
using MongoDB.Bson;

namespace Contents.Business.Service
{
    public class ContentService : IContentService
    {
        private readonly IMongoRepository<Content> _contentRepository;
        private readonly IMongoRepository<OrderedContent> _orderedContentRepository;
        public ContentService(
            IMongoRepository<Content> contentRepository,
            IMongoRepository<OrderedContent> orderedContentRepository)
        {
            _contentRepository = contentRepository;
            _orderedContentRepository = orderedContentRepository;
        }

        public async Task<ContentSelectResult> GetContentsAsync()
        {
            var selectResult = new ContentSelectResult();

            var contents = await _contentRepository.FilterByAsync(x => true);

            var contentModelItems = contents.Select(x => new ContentModel
            {
                Id = x.Id.ToString(),
                Description = x.Description,
                Title = x.Title,
                Url = x.Url
            });

            selectResult.Data = contentModelItems;

            return selectResult;
        }

        public OrderedContentSelectResult GetContentsByOrderedName(string orderedName)
        {
            var selectResult = new OrderedContentSelectResult();

            var orderedContents = _orderedContentRepository.Aggregate<LookedUpOrderedContent>(
                x => x.Name == orderedName,
                "contents",
                "Contents.Id",
                "_id",
                "ContentDocs");

            if (orderedContents == null)
            {
                selectResult.Data = default(ContensByOrderedName);
                return selectResult;
            }

            var contentsByOrderedName = new ContensByOrderedName();

            contentsByOrderedName.OrderedName = orderedContents.Name;
            contentsByOrderedName.Contents = orderedContents.ContentDocs
                .Select(conDoc => new ContentModel
                {
                    Id = conDoc.Id.ToString(),
                    Description = conDoc.Description,
                    OrderValue = orderedContents?.Contents?.SingleOrDefault(x => x.Id.Equals(conDoc.Id))?.OrderValue,
                    Title = conDoc.Title,
                    Url = conDoc.Url
                })
                .OrderBy(x => x.OrderValue).ToList();

            selectResult.Data = contentsByOrderedName;

            return selectResult;
        }

        public async Task<ContentInsertResult> InsertOrderedContentAsync(string orderedContentName, List<ContentSaveModel> contentUpdateModel)
        {
            var insertResult = new ContentInsertResult();

            var existingItem = _orderedContentRepository.FindOne(x => x.Name == orderedContentName);

            if (existingItem != null)
            {
                insertResult.IsSuccess = false;
                insertResult.Message = $"{orderedContentName} already exists.";
            }

            int orderValue = 0;

            var orderedContentEntity = new OrderedContent();
            orderedContentEntity.Name = orderedContentName;
            orderedContentEntity.Contents = contentUpdateModel.Select(x => new ContentItem
            {
                Id = new ObjectId(x.ContentId),
                OrderValue = orderValue++
            }).ToList();

            await _orderedContentRepository.InsertOneAsync(orderedContentEntity);
            insertResult.Data = true;

            return insertResult;
        }

        public async Task<ContentUpdateResult> UpdateOrderedContentAsync(string orderedContentName, List<ContentSaveModel> contentUpdateModel)
        {
            var result = new ContentUpdateResult();

            var orderedContent = _orderedContentRepository.FindOne(x => x.Name == orderedContentName);
            if (orderedContent == null)
            {
                result.IsSuccess = false;
                result.Message = $"Ordered content not found with name {orderedContentName}";

                return result;
            }

            var orderList = contentUpdateModel.Select(x => new ObjectId(x.ContentId)).ToList();
            foreach (var contentItem in orderedContent.Contents.ToList())
            {
                var orderIndex = orderList.FindIndex(x => x == contentItem.Id);
                if(orderIndex == -1)
                {
                    orderedContent.Contents.Remove(contentItem);
                }

                contentItem.OrderValue = orderIndex;
            }

            var replaceResult = await _orderedContentRepository.ReplaceOneAsync(x => x.Id == orderedContent.Id, orderedContent);
            if (replaceResult.MatchedCount < 1)
            {
                result.IsSuccess = false;
                result.Data = orderedContent.Id.ToString();
            }

            return result;
        }

        public OrderedContent ReorderContents(OrderedContent orderedContent, List<ContentSaveModel> contentUpdateModel)
        {


            return orderedContent;
        }

        public async Task<ContentUpdateResult> UpdateContent(string contentId, ContentModel content)
        {
            var contentUpdateResult = new ContentUpdateResult();
            var documentId = new ObjectId(contentId);

            var currentContent = _contentRepository.FindOne(x => x.Id == documentId);
            if (currentContent == null)
            {
                contentUpdateResult.IsSuccess = false;
                contentUpdateResult.Message = $"{contentId} content not found";

                return contentUpdateResult;
            }

            var contentEntity = new Content
            {
                Id = documentId,
                Description = content.Description,
                Title = content.Title,
                Url = content.Url
            };

            var replaceResult = await _contentRepository.ReplaceOneAsync(x => x.Id == documentId, contentEntity);

            if (replaceResult.ModifiedCount == 0)
            {
                contentUpdateResult.IsSuccess = false;
                contentUpdateResult.Message = $"{contentId} content cant updated";

                return contentUpdateResult;
            }

            contentUpdateResult.Data = contentId;

            return contentUpdateResult;
        }
    }
}
