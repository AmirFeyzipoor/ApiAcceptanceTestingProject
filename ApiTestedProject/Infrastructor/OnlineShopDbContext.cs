using Microsoft.EntityFrameworkCore;

namespace WebApiTestedProject.Infrastructor
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
