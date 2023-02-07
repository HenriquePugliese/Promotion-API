using Acropolis.Application.Features.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class PromotionCnpjConfiguration : IEntityTypeConfiguration<PromotionCnpj>
{
    public void Configure(EntityTypeBuilder<PromotionCnpj> builder)
    {
        builder.ToTable("PromotionsCnpjs");

        builder.HasKey(promotionCnpj => promotionCnpj.Id);

        builder.Property(promotionCnpj => promotionCnpj.ExternalId).IsRequired();
        builder.Property(promotionCnpj => promotionCnpj.Cnpj).IsRequired().HasMaxLength(14);

        builder.HasOne(promotionCnpj => promotionCnpj.Promotion)
            .WithMany(promotion => promotion.Cnpjs)
            .HasForeignKey(promotionCnpj => promotionCnpj.PromotionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(promotionCnpj => promotionCnpj.PromotionId);
        builder.HasIndex(promotionCnpj => new { promotionCnpj.ExternalId, promotionCnpj.Cnpj }).IsUnique();
    }
}
