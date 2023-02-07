using Acropolis.Application.Features.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(500);
        builder.Property(p => p.MaterialCode).IsRequired().HasMaxLength(50);
        builder.Property(p => p.UnitMeasure).IsRequired().HasMaxLength(50);
        builder.Property(p => p.UnitWeight).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Weight).IsRequired().HasColumnType("decimal(18,4)");
        builder.Property(p => p.Status).IsRequired();        
    }
}
