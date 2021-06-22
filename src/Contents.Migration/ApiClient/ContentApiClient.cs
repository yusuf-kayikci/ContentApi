using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Contents.Common.Config;
using Contents.Common.Exceptions;
using Contents.Migration.Abstraction;
using Contents.Migration.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Contents.Migration.ApiClient
{
    public class ContentApiClient : IContentApiClient
    {
        private readonly HttpClient _client;
        private readonly MigrationHttpDataSourceConfig _config;
        public ContentApiClient(HttpClient client, IOptions<MigrationHttpDataSourceConfig> config)
        {
            _config = config.Value;
            _client = client;
        }

        public async Task<ContentIntegrationData> GetNewsAsync()
        {
            var response = await _client.GetAsync(_config.Url);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ContentMigrationException($"Invalid api client request with status code {response.StatusCode}");
            }

            var responseContent = await response?.Content?.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<ContentIntegrationData>(responseContent);

            return responseModel;
        }
    }
}
