using Acropolis.Application.Features.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class PromotionRuleConfiguration : IEntityTypeConfiguration<PromotionRule>
{
    public void Configure(EntityTypeBuilder<PromotionRule> builder)
    {
        builder.ToTable("PromotionsRules");

        builder.HasKey(promotionRule => promotionRule.Id);

        builder.Property(promotionRule => promotionRule.TotalAttributes);
        builder.Property(promotionRule => promotionRule.Percentage).HasColumnType("decimal(24,2)");
        builder.Property(promotionRule => promotionRule.GreaterEqualValue).HasColumnType("decimal(24,2)");

        builder.HasOne(promotionRule => promotionRule.Promotion)
            .WithMany(promotion => promotion.Rules)
            .HasForeignKey(promotionRule => promotionRule.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(promotionRule => promotionRule.PromotionId);
    }
}
