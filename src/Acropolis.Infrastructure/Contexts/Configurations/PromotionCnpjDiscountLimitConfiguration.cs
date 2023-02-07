using Acropolis.Application.Features.Promotions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class PromotionCnpjDiscountLimitConfiguration : IEntityTypeConfiguration<PromotionCnpjDiscountLimit>
{
    public void Configure(EntityTypeBuilder<PromotionCnpjDiscountLimit> builder)
    {
        builder.ToTable("PromotionsCnpjsDiscountLimits");

        builder.HasKey(discountLimit => discountLimit.Id);

        builder.Property(discountLimit => discountLimit.Cnpj).IsRequired().HasMaxLength(14);
        builder.Property(discountLimit => discountLimit.Percent).IsRequired();

        builder.HasIndex(discountLimit => discountLimit.Cnpj).IsUnique();
    }
}
