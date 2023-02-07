using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Attribute = Acropolis.Application.Features.Attributes.Attribute;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class AttributeConfiguration : IEntityTypeConfiguration<Attribute>
{
    public void Configure(EntityTypeBuilder<Attribute> builder)
    {
        builder.ToTable("Attributes");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.ProductId).IsRequired();

        builder.Property(p => p.AttributeKeyId).IsRequired();
        builder.HasIndex(p => p.AttributeKey).IsUnique(false);
        builder.Property(p => p.AttributeKey).IsRequired();
        builder.Property(p => p.AttributeKeyLabel).IsRequired();
        builder.Property(p => p.AttributeKeyIsBeginOpen).IsRequired();
        builder.Property(p => p.AttributeKeyDescription).IsRequired(false);

        builder.Property(p => p.AttributeValueId).IsRequired();
        builder.HasIndex(p => p.AttributeValue).IsUnique(false);
        builder.Property(p => p.AttributeValue).IsRequired();
        builder.Property(p => p.AttributeValueLabel).IsRequired();        
    }
}
