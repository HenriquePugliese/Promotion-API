using Acropolis.Application.Features.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acropolis.Infrastructure.Contexts.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(p => p.Id);

        builder.Property(t => t.Cnpj).IsRequired().HasMaxLength(14);

        builder.HasIndex(t => new { t.Cnpj, t.SellerId } ).IsUnique(false);

        builder.Property(t => t.SellerId).IsRequired();

        builder.Property(t => t.CustomerCode).IsRequired();
    }
}
