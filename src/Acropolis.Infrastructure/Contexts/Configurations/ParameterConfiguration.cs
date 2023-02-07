using Acropolis.Application.Features.Parameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class ParameterConfiguration : IEntityTypeConfiguration<Parameter>
{
    public void Configure(EntityTypeBuilder<Parameter> builder)
    {
        builder.ToTable("Parameters");

        builder.HasKey(p => p.Id);

        builder.HasIndex(t => new
        {
            t.CustomerId,
            t.Code,
            t.Value
        })
        .IsUnique(false);

        builder.Property(t => t.Code).HasMaxLength(100).IsRequired();

        builder.Property(t => t.Value).HasMaxLength(100).IsRequired();

        builder.Property(t => t.Description).HasMaxLength(100);

        builder.HasOne(x => x.Customer)
                   .WithMany(x => x.Parameters)
                   .HasForeignKey(x => x.CustomerId)
                   .OnDelete(DeleteBehavior.ClientCascade);
    }
}