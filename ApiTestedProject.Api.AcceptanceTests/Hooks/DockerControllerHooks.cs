using ApiTestedProject.Api.AcceptanceTests.Models.Clients;
using ApiTestedProject.Api.AcceptanceTests.Models.MigrationSettings.Context;
using BoDi;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Builders;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Refit;

namespace ApiTestedProject.Api.AcceptanceTests.Hooks
{
    [Binding]
    public class DockerControllerHooks
    {
        private IObjectContainer _objectContainer;
        private static ICompositeService? _compositeService;

        public DockerControllerHooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [AfterTestRun]
        public static async Task DropSnapshotAndMigrationDownDatabaseAndDownContainers()
        {
            var masterConnectionString = "server=192.168.3.32,42069;password=password123!;Persist Security Info=True;User ID=sa;Initial Catalog=master;TrustServerCertificate=True;Command Timeout=10000";
            var onlineShopConnectionString = "server=192.168.3.32,42069;password=password123!;Persist Security Info=True;User ID=sa;Initial Catalog=OnlineShop;TrustServerCertificate=True;Command Timeout=10000";
            var snapshotName = "TestSnapshot";

            using (var connection = new SqlConnection(masterConnectionString))
            {
                await connection.OpenAsync();

                using (var checkSnapshotCommand = connection.CreateCommand())
                {
                    checkSnapshotCommand.CommandText = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{snapshotName}'";
                    var count = (int?)await checkSnapshotCommand.ExecuteScalarAsync();
                    if (count.HasValue && count.Value > 0)
                    {
                        using (var deleteSnapshotCommand = connection.CreateCommand())
                        {
                            deleteSnapshotCommand.CommandText = $"DROP DATABASE {snapshotName}";
                            await deleteSnapshotCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }

            var optionBuilder = new DbContextOptionsBuilder<OnlineShopDbContext>()
               .UseSqlServer(onlineShopConnectionString, options =>
               {
                   options.EnableRetryOnFailure();
               });
            using var db = new OnlineShopDbContext(optionBuilder.Options);
            await db.Database.EnsureDeletedAsync();

            _compositeService!.Stop();
            _compositeService.Dispose();
        }

        [BeforeTestRun]
        public static async Task UpContainersAndMigrationUpDatabaseAndCreateSnapshot()
        {
            var config = LoadConfiguration();
            var dockerComposeFileName = config["DockerComposeFileName"];
            var dockerComposePath = GetDockerComposeLocation(dockerComposeFileName!);
            _compositeService = new Builder()
                .UseContainer()
                .UseCompose()
                .FromFile(dockerComposePath)
                .RemoveOrphans()
                .Build();
            _compositeService!.Start();

            var masterConnectionString = "server=192.168.3.32,42069;password=password123!;Persist Security Info=True;User ID=sa;Initial Catalog=master;TrustServerCertificate=True;Command Timeout=10000";
            var onlineShopConnectionString = "server=192.168.3.32,42069;password=password123!;Persist Security Info=True;User ID=sa;Initial Catalog=OnlineShop;TrustServerCertificate=True;Command Timeout=10000";
            var snapshotName = "TestSnapshot";

            var optionBuilder = new DbContextOptionsBuilder<OnlineShopDbContext>()
                .UseSqlServer(onlineShopConnectionString, options =>
                {
                    options.EnableRetryOnFailure();
                });
            using var db = new OnlineShopDbContext(optionBuilder.Options);
            await db.Database.MigrateAsync();

            using (var connection = new SqlConnection(masterConnectionString))
            {
                await connection.OpenAsync();

                using (var createSnapshotCommand = connection.CreateCommand())
                {
                    createSnapshotCommand.CommandText = $"CREATE DATABASE {snapshotName} ON (NAME = OnlineShop, FILENAME = '/var/opt/sqlserver/data/{snapshotName}') AS SNAPSHOT OF OnlineShop";
                    await createSnapshotCommand.ExecuteNonQueryAsync();
                }
            }

        }

        [AfterScenario]
        public static async Task RestoreDatabase()
        {
            var masterConnectionString = "server=192.168.3.32,42069;password=password123!;Persist Security Info=True;User ID=sa;Initial Catalog=master;TrustServerCertificate=True;Command Timeout=10000";
            using (var connection = new SqlConnection(masterConnectionString))
            {
                await connection.OpenAsync();

                var currentProcessIdCommand = connection.CreateCommand();
                currentProcessIdCommand.CommandText = "SELECT @@SPID";
                short currentProcessIdShort = (short)currentProcessIdCommand.ExecuteScalar();
                int currentProcessId = Convert.ToInt32(currentProcessIdShort);

                var killConnectionsCommand = connection.CreateCommand();
                killConnectionsCommand.CommandText = $"DECLARE @kill varchar(8000) = ''; SELECT @kill = @kill + 'KILL ' + CONVERT(varchar(5), spid) + ';' FROM master..sysprocesses WHERE dbid = DB_ID('OnlineShop') AND spid <> {currentProcessId}; EXEC(@kill);";
                await killConnectionsCommand.ExecuteNonQueryAsync();

                var restoreCommand = connection.CreateCommand();
                restoreCommand.CommandText = "RESTORE DATABASE OnlineShop FROM DATABASE_SNAPSHOT = 'TestSnapshot'";
                await restoreCommand.ExecuteNonQueryAsync();
            }
        }

        [BeforeScenario]
        public void AddRefitClient()
        {
            var config = LoadConfiguration();

            var productClient = RestService.For<IProductClient>
                (config["ApiTestedProject:BaseAddress"]!);

            _objectContainer.RegisterInstanceAs<IProductClient>(productClient);
        }

        private static string GetDockerComposeLocation(string dockerComposeFileName)
        {
            var directory = Directory.GetCurrentDirectory();
            while (!Directory.EnumerateFiles(directory, "*.yml")
                .Any(_ => _.EndsWith(dockerComposeFileName)))
            {
                directory = directory.Substring(0,
                    directory.LastIndexOf(Path.DirectorySeparatorChar));
            }
            return Path.Combine(directory, dockerComposeFileName);
        }

        private static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        }
    }
}
