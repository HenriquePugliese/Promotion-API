using Acropolis.Application.Features.Parameters;

namespace Acropolis.Application.Features.Customers;

public class Customer
{
    public Customer(string? cnpj, string? sellerid, string? customerCode, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Cnpj = cnpj;
        SellerId = sellerid;
        CustomerCode = customerCode;
    }

    private Customer()
    {
    }

    public Guid Id { get; private set; }
    public string? Cnpj { get; private set; }
    public string? SellerId { get; set; }
    public string? CustomerCode { get; set; }

    public List<Parameter>? Parameters{ get; set; }
}