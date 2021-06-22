using System.Threading.Tasks;
using Contents.Migration.Model;

namespace Contents.Migration.Abstraction
{
    public interface IContentApiClient
    {
        Task<ContentIntegrationData> GetNewsAsync();
    }
}
