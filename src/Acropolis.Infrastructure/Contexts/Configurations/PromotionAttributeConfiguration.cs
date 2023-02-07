using Acropolis.Application.Features.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class PromotionAttributeConfiguration : IEntityTypeConfiguration<PromotionAttribute>
{
    public void Configure(EntityTypeBuilder<PromotionAttribute> builder)
    {
        builder.ToTable("PromotionsAttributes");

        builder.HasKey(promotionAttribute => promotionAttribute.Id);

        builder.Property(promotionAttribute => promotionAttribute.AmountWeight).HasColumnType("decimal(18,4)");
        builder.Property(promotionAttribute => promotionAttribute.AttributesId);

        builder.HasMany(promotionAttribute => promotionAttribute.SKUs)
                    .WithOne(sku => sku.PromotionAttribute)
                    .HasForeignKey(sku => sku.PromotionAttributeId);

        builder.HasOne(promotionAttribute => promotionAttribute.Promotion)
            .WithMany(promotion => promotion.Attributes)
            .HasForeignKey(promotionAttribute => promotionAttribute.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(promotionAttribute => promotionAttribute.PromotionId);
    }
}
