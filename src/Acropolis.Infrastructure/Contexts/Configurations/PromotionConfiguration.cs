using Acropolis.Application.Features.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.ToTable("Promotions");

        builder.HasKey(promotion => promotion.Id);
        builder.Property(promotion => promotion.Name).HasColumnType("varchar").IsRequired().HasMaxLength(300);
        builder.Property(promotion => promotion.DtStart).IsRequired();
        builder.Property(promotion => promotion.DtEnd).IsRequired();
        builder.Property(promotion => promotion.UnitMeasurement).IsRequired();
        
        builder.HasIndex(promotion => promotion.ExternalId).IsUnique();

        builder.HasMany(promotion => promotion.Rules)
                    .WithOne(rule => rule.Promotion)
                    .HasForeignKey(rule => rule.PromotionId);

        builder.HasMany(promotion => promotion.Attributes)
                    .WithOne(rule => rule.Promotion)
                    .HasForeignKey(rule => rule.PromotionId);

        builder.HasMany(promotion => promotion.Parameters)
                    .WithOne(rule => rule.Promotion)
                    .HasForeignKey(rule => rule.PromotionId);

        builder.HasMany(promotion => promotion.Cnpjs)
                    .WithOne(cnpj => cnpj.Promotion)
                    .HasForeignKey(cnpj => cnpj.PromotionId);
    }
}
