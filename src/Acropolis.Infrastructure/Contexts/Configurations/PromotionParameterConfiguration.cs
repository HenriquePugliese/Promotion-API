using Acropolis.Application.Features.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class PromotionParameterConfiguration : IEntityTypeConfiguration<PromotionParameter>
{
    public void Configure(EntityTypeBuilder<PromotionParameter> builder)
    {
        builder.ToTable("PromotionsParameters");

        builder.HasKey(promotionParameter => promotionParameter.Id);

        builder.Property(promotionParameter => promotionParameter.Name);
        builder.Property(promotionParameter => promotionParameter.Value);

        builder.HasOne(promotionParameter => promotionParameter.Promotion)
            .WithMany(promotion => promotion.Parameters)
            .HasForeignKey(promotionParameter => promotionParameter.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(promotionParameter => promotionParameter.PromotionId);
    }
}
