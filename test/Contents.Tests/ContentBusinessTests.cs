using System.Linq;
using Contents.Business.Abstraction;
using Contents.Business.Model;
using Contents.Business.Service;
using Contents.Data.Abstraction;
using Contents.Data.Entity;
using MongoDB.Bson;
using Moq;
using Xunit;

namespace Contents.Tests
{
    public class ContentBusinessTests
    {

        private readonly Mock<IMongoRepository<Content>> _mockContentRepository;
        private readonly Mock<IMongoRepository<OrderedContent>> _mockOrderedContentRepository;
        private readonly IContentService _contentService;

        private const string item1 = "60d11c42295c0e8694e44d50";
        private const string item2 = "60d118706d4b05bc60e44e7d";
        private const string item3 = "60d118706d4b05bc60e44e84";
        private const string item4 = "60d118706d4b05bc60e44e87";
        private const string item5 = "60d118706d4b05bc60e44e88";

        public ContentBusinessTests()
        {
            _mockContentRepository = new Mock<IMongoRepository<Content>>();
            _mockOrderedContentRepository = new Mock<IMongoRepository<OrderedContent>>();
            _contentService = new ContentService(_mockContentRepository.Object, _mockOrderedContentRepository.Object);
        }

        /// <summary>
        /// Update current order with expected order.
        /// </summary>
        /// <param name="beforeOrder"> represent already saved ordered contents -- currentorder </param>
        /// <param name="orderUpdateModel">represent after update order of contents -- expectedorder</param>
        [InlineData(new string[] { item3, item2, item1 }, new string[] { item2, item1, item3 })]
        [InlineData(new string[] { item2, item3, item1 }, new string[] { item2, item1, item3 })]
        [InlineData(new string[] { item1, item2, item3, item5 }, new string[] { item5, item1, item3, item1 })]
        [InlineData(new string[] { item1, item3, item2, item5 }, new string[] { item2, item1, item3, item5 })]
        [InlineData(new string[] { item1, item3, item2, item4, item5 }, new string[] { item2, item1, item3, item5, item4 })]
        [Theory]
        public void Update_OrderedContent_Should_Order_Contents_Correctly(string[] beforeOrder, string[] orderUpdateModel)
        {
            //arrange
            var i = 0;
            var orderedContents = new OrderedContent
            {
                Id = new ObjectId(),
                Name = "test_ordered_list",
                Contents = beforeOrder.Select(x => new ContentItem
                {
                    Id = new ObjectId(x),
                    OrderValue = i++
                }).ToList()
            };

            var editorChanges = orderUpdateModel
                .Select(x => new ContentSaveModel
                {
                    ContentId = x
                })
                .ToList();

            //act
            var afterOrder = _contentService.ReorderContents(orderedContents, editorChanges);

            //assert
            foreach (var content in afterOrder.Contents)
            {
                var orderValue = editorChanges.FindIndex(x => x.ContentId == content.Id.ToString());
                //afterorder must equal with editor change
                Assert.Equal(content.OrderValue, orderValue);
            }

            Assert.Equal(beforeOrder.Count(), orderedContents.Contents.Count);
        }
    }
}
