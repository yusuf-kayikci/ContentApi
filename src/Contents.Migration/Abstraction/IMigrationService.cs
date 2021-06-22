using System.Threading.Tasks;

namespace Contents.Migration.Abstraction
{
    public interface IMigrationService
    {
        Task MigrateAsync();
    }
}
