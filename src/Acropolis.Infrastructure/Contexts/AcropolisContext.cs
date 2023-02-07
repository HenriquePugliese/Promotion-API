using Acropolis.Application.Features.Customers;
using Acropolis.Application.Features.Parameters;
using Acropolis.Application.Features.Products;
using Acropolis.Application.Features.Promotions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Ziggurat.SqlServer;
using Attribute = Acropolis.Application.Features.Attributes.Attribute;

namespace Acropolis.Infrastructure.Contexts;

public class AcropolisContext : DbContext
{
    public AcropolisContext(DbContextOptions<AcropolisContext> options) : base(options)
    {
    }

    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<PromotionRule> PromotionsRules => Set<PromotionRule>();
    public DbSet<PromotionParameter> PromotionsParameters => Set<PromotionParameter>();
    public DbSet<PromotionAttribute> PromotionsAttributes => Set<PromotionAttribute>();
    public DbSet<PromotionAttributeSku> PromotionsAttributesSKUs => Set<PromotionAttributeSku>();
    public DbSet<PromotionCnpj> PromotionsCnpjs => Set<PromotionCnpj>();
    public DbSet<PromotionCnpjDiscountLimit> PromotionsCnpjsDiscountLimits => Set<PromotionCnpjDiscountLimit>();
    public DbSet<Attribute> Attributes => Set<Attribute>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Parameter> Parameters => Set<Parameter>();
    public DbSet<MessageTracking> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}