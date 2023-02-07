using Acropolis.Application.Features.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class PromotionAttributeSkuConfiguration : IEntityTypeConfiguration<PromotionAttributeSku>
{
    public void Configure(EntityTypeBuilder<PromotionAttributeSku> builder)
    {
        builder.ToTable("PromotionsAttributesSKUs");

        builder.HasKey(promotionAttributeSKU => promotionAttributeSKU.Id);

        builder.Property(promotionAttributeSKU => promotionAttributeSKU.Value);

        builder.HasOne(promotionAttributeSKU => promotionAttributeSKU.PromotionAttribute)
            .WithMany(promotionAttribute => promotionAttribute.SKUs)
            .HasForeignKey(promotionAttributeSKU => promotionAttributeSKU.PromotionAttributeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(promotionAttributeSKU => promotionAttributeSKU.PromotionAttributeId);
    }
}
