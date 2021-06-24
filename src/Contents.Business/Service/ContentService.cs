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

            var reorderedContents = ReorderContents(orderedContent, contentUpdateModel);

            var replaceResult = await _orderedContentRepository.ReplaceOneAsync(x => x.Id == reorderedContents.Id, reorderedContents);
            if (replaceResult.MatchedCount < 1)
            {
                result.IsSuccess = false;
                result.Data = reorderedContents.Id.ToString();
            }

            return result;
        }

        public OrderedContent ReorderContents(OrderedContent orderedContent, List<ContentSaveModel> contentUpdateModel)
        {
            var orderList = new List<ObjectId>();
            orderList = contentUpdateModel.Select(x => new ObjectId(x.ContentId)).ToList();
            foreach (var contentItem in orderedContent.Contents)
            {
                var orderIndex = orderList.FindIndex(x => x == contentItem.Id);
                contentItem.OrderValue = orderIndex;
            }

            return orderedContent;
        }
    }
}
