using System.Linq;
using System.Threading.Tasks;
using Contents.Data.Abstraction;
using Contents.Data.Entity;
using Contents.Migration.Abstraction;

namespace Contents.Migration.Service
{
    public class MigrationService : IMigrationService
    {

        private readonly IMongoRepository<Content> _newsRepository;
        private readonly IContentApiClient _newsIntegration;
        public MigrationService(
            IContentApiClient newsIntegration,
            IMongoRepository<Content> newsRepository)
        {
            _newsIntegration = newsIntegration;
            _newsRepository = newsRepository;

        }

        /// <summary>
        /// Migrate news
        /// </summary>
        /// <returns></returns>
        public async Task MigrateAsync()
        {
            var contentIntegrationData = await _newsIntegration.GetNewsAsync();
            var contents = contentIntegrationData.Articles
                .Select(x => new Content
                {
                    Title = x.Title,
                    Description = x.Description,
                    Url = x.Url
                })
                .ToList();

            await _newsRepository.InsertManyAsync(contents);
        }
    }
}
