using Microsoft.EntityFrameworkCore;

namespace ApiTestedProject.Api.AcceptanceTests.Models.MigrationSettings.Context
{
    public class OnlineShopDbContext : DbContext
    {
        public OnlineShopDbContext(DbContextOptions<OnlineShopDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(OnlineShopDbContext).Assembly);
        }
    }
}
