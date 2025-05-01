using CodeCart.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeCart.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");

        builder.HasIndex(p => p.Brand);

        builder.HasIndex(p => p.Type);

        builder.HasIndex(p => new { p.Brand, p.Type });
    }
}