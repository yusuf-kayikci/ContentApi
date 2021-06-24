using System.Collections.Generic;
using System.Linq;
using Contents.Business.Abstraction;
using Contents.Business.Model;
using Contents.Business.Service;
using Contents.Data.Abstraction;
using Contents.Data.Entity;
using FluentAssertions;
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

        public static IEnumerable<object[]> MockData => new List<object[]> {
            new object[] { mockOrderedContents1_Before, mockOrderUpdateModel1, mockOrderedContents1_After},
            new object[] { mockOrderedContents2_Before, mockOrderUpdateModel2, mockOrderedContents2_After},
            new object[] { mockOrderedContents3_Before, mockOrderUpdateModel3, mockOrderedContents3_After},
            new object[] { mockOrderedContents4_Before, mockOrderUpdateModel4, mockOrderedContents4_After},
        };

        public ContentBusinessTests()
        {
            _mockContentRepository = new Mock<IMongoRepository<Content>>(MockBehavior.Strict);
            _mockOrderedContentRepository = new Mock<IMongoRepository<OrderedContent>>(MockBehavior.Strict);
            _contentService = new ContentService(_mockContentRepository.Object, _mockOrderedContentRepository.Object);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="beforeOrder"> current ordered contents in database </param>
        /// <param name="editorChanges"> editor changes about ordered contents </param>
        /// <param name="afterOrder">  </param>
        [MemberData(nameof(MockData))]
        [Theory]
        public void ReorderContents_Should_Update_Contents_Order_Correctly(OrderedContent beforeOrder, List<ContentSaveModel> editorChanges, LookedUpOrderedContent afterOrder)
        {
            string orderedName = beforeOrder.Name;
            _mockOrderedContentRepository.Setup(x => x.Aggregate<LookedUpOrderedContent>(
                x => x.Name == orderedName,
                "contents",
                "Contents.Id",
                "_id",
                "ContentDocs")).Returns(afterOrder);

            _contentService.UpdateOrderedContentAsync(beforeOrder.Name, editorChanges);

            string orderedContentName = afterOrder.Name;
            _mockOrderedContentRepository.Setup(x => x.FindOne(x => x.Name == orderedContentName))
                .Returns(beforeOrder);

            var actualResult = _contentService.GetContentsByOrderedName(beforeOrder.Name);

            actualResult.Data.OrderedName.Should().Be(afterOrder.Name);
            actualResult.Data.Contents.Count.Should().Be(afterOrder.Contents.Count);

            foreach (var content in actualResult.Data.Contents)
            {
                var itemAfterOrder = afterOrder.Contents.Single(x => x.Id.ToString() == content.Id);

                itemAfterOrder.Should().NotBeNull();
                itemAfterOrder.OrderValue.Should().Be(content.OrderValue);
            }
        }

        #region Mock Data

        private const string item1 = "60d11c42295c0e8694e44d50";
        private const string item2 = "60d118706d4b05bc60e44e7d";
        private const string item3 = "60d118706d4b05bc60e44e84";
        private const string item4 = "60d118706d4b05bc60e44e87";
        private const string item5 = "60d118706d4b05bc60e44e88";


        private static OrderedContent mockOrderedContents1_Before = new OrderedContent
        {
            Name = "orderedContent1",
            Contents = new List<ContentItem>
            {
                new ContentItem { Id = new ObjectId(item1) , OrderValue = 0  },
                new ContentItem { Id = new ObjectId(item2) , OrderValue = 1  },
                new ContentItem { Id = new ObjectId(item3) , OrderValue = 2  },
                new ContentItem { Id = new ObjectId(item4) , OrderValue = 3  },
                new ContentItem { Id = new ObjectId(item5) , OrderValue = 4  }
            }
        };

        private static List<ContentSaveModel> mockOrderUpdateModel1 = new List<ContentSaveModel>
        {
            new ContentSaveModel {ContentId = item5},
            new ContentSaveModel {ContentId = item4},
            new ContentSaveModel {ContentId = item3},
            new ContentSaveModel {ContentId = item2},
            new ContentSaveModel {ContentId = item1}
        };

        private static LookedUpOrderedContent mockOrderedContents1_After = new LookedUpOrderedContent
        {
            Name = "orderedContent1",
            Contents = new List<ContentItem>
            {
                new ContentItem { Id = new ObjectId(item1), OrderValue = 4 },
                new ContentItem { Id = new ObjectId(item2), OrderValue = 3 },
                new ContentItem { Id = new ObjectId(item3), OrderValue = 2 },
                new ContentItem { Id = new ObjectId(item4), OrderValue = 1 },
                new ContentItem { Id = new ObjectId(item5), OrderValue = 0 }
            },
            ContentDocs = new List<Content>
            {
                new Content{Id = new ObjectId(item1)},
                new Content{Id = new ObjectId(item2)},
                new Content{Id = new ObjectId(item3)},
                new Content{Id = new ObjectId(item4)},
                new Content{Id = new ObjectId(item5)},

            }
        };


        private static OrderedContent mockOrderedContents2_Before = new OrderedContent
        {
            Name = "orderedContent2",
            Contents = new List<ContentItem>
            {
                new ContentItem { Id = new ObjectId(item5) , OrderValue = 0  },
                new ContentItem { Id = new ObjectId(item4) , OrderValue = 1  },
                new ContentItem { Id = new ObjectId(item3) , OrderValue = 2  },
                new ContentItem { Id = new ObjectId(item2) , OrderValue = 3  },
                new ContentItem { Id = new ObjectId(item1) , OrderValue = 4  }
            }
        };

        private static List<ContentSaveModel> mockOrderUpdateModel2 = new List<ContentSaveModel>
        {
            new ContentSaveModel {ContentId = item2},
            new ContentSaveModel {ContentId = item1},
            new ContentSaveModel {ContentId = item4},
            new ContentSaveModel {ContentId = item5},
            new ContentSaveModel {ContentId = item3}
        };

        private static LookedUpOrderedContent mockOrderedContents2_After = new LookedUpOrderedContent
        {
            Name = "orderedContent2",
            Contents = new List<ContentItem>
            {
                new ContentItem { Id = new ObjectId(item5) , OrderValue = 3  },
                new ContentItem { Id = new ObjectId(item4) , OrderValue = 2  },
                new ContentItem { Id = new ObjectId(item3) , OrderValue = 4  },
                new ContentItem { Id = new ObjectId(item2) , OrderValue = 0  },
                new ContentItem { Id = new ObjectId(item1) , OrderValue = 1  }
            },
            ContentDocs = new List<Content>
            {
                new Content{Id = new ObjectId(item5)},
                new Content{Id = new ObjectId(item4)},
                new Content{Id = new ObjectId(item3)},
                new Content{Id = new ObjectId(item2)},
                new Content{Id = new ObjectId(item1)},
            }
        };


        private static OrderedContent mockOrderedContents3_Before = new OrderedContent
        {
            Name = "orderedContent2",
            Contents = new List<ContentItem>
            {
                new ContentItem { Id = new ObjectId(item5) , OrderValue = 0  },
                new ContentItem { Id = new ObjectId(item4) , OrderValue = 1  },
                new ContentItem { Id = new ObjectId(item3) , OrderValue = 2  },
                new ContentItem { Id = new ObjectId(item2) , OrderValue = 3  },
                new ContentItem { Id = new ObjectId(item1) , OrderValue = 4  }
            }
        };

        private static List<ContentSaveModel> mockOrderUpdateModel3 = new List<ContentSaveModel>
        {
            new ContentSaveModel {ContentId = item5},
            new ContentSaveModel {ContentId = item2},
            new ContentSaveModel {ContentId = item3},
            new ContentSaveModel {ContentId = item1},
            new ContentSaveModel {ContentId = item4}
        };

        private static LookedUpOrderedContent mockOrderedContents3_After = new LookedUpOrderedContent
        {
            Name = "orderedContent2",
            Contents = new List<ContentItem>
            {
                new ContentItem { Id = new ObjectId(item5) , OrderValue = 0  },
                new ContentItem { Id = new ObjectId(item4) , OrderValue = 4  },
                new ContentItem { Id = new ObjectId(item3) , OrderValue = 2  },
                new ContentItem { Id = new ObjectId(item2) , OrderValue = 1  },
                new ContentItem { Id = new ObjectId(item1) , OrderValue = 3  }
            },
            ContentDocs = new List<Content>
            {
                new Content{Id = new ObjectId(item5)},
                new Content{Id = new ObjectId(item4)},
                new Content{Id = new ObjectId(item3)},
                new Content{Id = new ObjectId(item2)},
                new Content{Id = new ObjectId(item1)},
            }
        };


        // when editor save again with same order
        private static OrderedContent mockOrderedContents4_Before = new OrderedContent
        {
            Name = "orderedContent2",
            Contents = new List<ContentItem>
            {
                new ContentItem { Id = new ObjectId(item1) , OrderValue = 0  },
                new ContentItem { Id = new ObjectId(item2) , OrderValue = 1  },
                new ContentItem { Id = new ObjectId(item3) , OrderValue = 2  },
                new ContentItem { Id = new ObjectId(item4) , OrderValue = 3  },
                new ContentItem { Id = new ObjectId(item5) , OrderValue = 4  }
            }
        };

        private static List<ContentSaveModel> mockOrderUpdateModel4 = new List<ContentSaveModel>
        {
            new ContentSaveModel {ContentId = item1},
            new ContentSaveModel {ContentId = item2},
            new ContentSaveModel {ContentId = item3},
            new ContentSaveModel {ContentId = item4},
            new ContentSaveModel {ContentId = item5}
        };

        private static LookedUpOrderedContent mockOrderedContents4_After = new LookedUpOrderedContent
        {
            Name = "orderedContent2",
            Contents = new List<ContentItem>
            {
                new ContentItem { Id = new ObjectId(item1) , OrderValue = 0  },
                new ContentItem { Id = new ObjectId(item2) , OrderValue = 1  },
                new ContentItem { Id = new ObjectId(item3) , OrderValue = 2  },
                new ContentItem { Id = new ObjectId(item4) , OrderValue = 3  },
                new ContentItem { Id = new ObjectId(item5) , OrderValue = 4  }
            },
            ContentDocs = new List<Content>
            {
                new Content{Id = new ObjectId(item1)},
                new Content{Id = new ObjectId(item2)},
                new Content{Id = new ObjectId(item3)},
                new Content{Id = new ObjectId(item4)},
                new Content{Id = new ObjectId(item5)},
            }
        };
        #endregion
    }
}
