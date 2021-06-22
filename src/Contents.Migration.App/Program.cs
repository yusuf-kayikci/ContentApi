using System;
using System.IO;
using System.Threading.Tasks;
using Contents.Common.Config;
using Contents.Data.Abstraction;
using Contents.Data.Repository;
using Contents.Migration.Abstraction;
using Contents.Migration.ApiClient;
using Contents.Migration.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Contents.Migration.App
{
    class Program
    {
        public static IConfigurationRoot Configuration;

        static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

            Configuration = builder.Build();

            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));
            services.Configure<MigrationHttpDataSourceConfig>(Configuration.GetSection("MigrationHttpDataSourceConfig"));

            services.AddSingleton<IMongoDbSettings>(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
            services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
            services.AddSingleton<IMigrationService, MigrationService>();

            services.AddHttpClient<IContentApiClient, ContentApiClient>();

            var serviceProvider = services.BuildServiceProvider();

            RunMigration(serviceProvider).Wait();
        }

        public async static Task RunMigration(ServiceProvider serviceProvider)
        {
            try
            {
                var migrationService = serviceProvider.GetService<IMigrationService>();

                Console.WriteLine("Migration start");
                await migrationService.MigrateAsync();
                Console.WriteLine("Migration end");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
