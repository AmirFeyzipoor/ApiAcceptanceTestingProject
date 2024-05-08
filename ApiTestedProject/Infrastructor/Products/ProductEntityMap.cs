using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiTestedProject.Models;

namespace WebApiTestedProject.Infrastructor.Products
{
    public class ProductEntityMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.Property(_ => _.Id).ValueGeneratedOnAdd();
            
            builder.Property(_ => _.Name).IsRequired();
            builder.Property(_ => _.Price).IsRequired();
        }
    }
}
