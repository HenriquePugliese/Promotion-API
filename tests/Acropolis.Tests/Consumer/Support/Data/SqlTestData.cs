using Acropolis.Application.Features.Customers;
using Acropolis.Application.Features.Parameters;
using Acropolis.Application.Features.Products;
using Acropolis.Infrastructure.Contexts;
using Bogus;
using Bogus.Extensions.Brazil;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Acropolis.Tests.Consumer.Support.Data;

public static class SqlTestData
{
    public static async Task<Customer> CustomerSeeder(IServiceScope serviceScope)
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<AcropolisContext>();

        var cnpj = new Faker("pt_BR").Company.Cnpj();

        var sellerId = new Guid().ToString();

        var customer = new Customer(Regex.Replace(cnpj, "[^0-9]", ""), sellerId, "987654");

        customer.Parameters = new List<Parameter>() {
                new(description: "VC-VAREJO_PP", code: "Mesoregiao" , value: "35B" , status: true, customerId: customer.Id, null),
                new(description: "VC-TATUI", code: "Microregiao" , value: "35B" , status: true, customerId: customer.Id, null),
        };

        await context.Customers.AddAsync(customer);

        await context.SaveChangesAsync();

        return customer;
    }

    public static async Task<Product> ProductSeeder(IServiceScope serviceScope)
    {
        var context = serviceScope.ServiceProvider.GetRequiredService<AcropolisContext>();

        var product = new Product(new()
        {
            SellerId = Guid.NewGuid(),
            Name = "Produto Teste Name",
            MaterialCode = "123456",
            UnitMeasure = "Kg",
            UnitWeight = "1",
            Weight = 10,
            Status = 1,
            Id = Guid.NewGuid()
        });

        await context.Products.AddAsync(product);

        await context.SaveChangesAsync();

        return product;
    }
}