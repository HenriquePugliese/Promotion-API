using Acropolis.Application.Features.Customers;

namespace Acropolis.Application.Features.Parameters;

public class Parameter
{
    public Parameter(
        string? description,
        string? code,
        string? value,
        bool status,
        Guid customerId,
        Guid? id = null)
    {
        Description = description;
        Code = code;
        Value = value;
        Status = status;
        CustomerId = customerId;
        Id = id ?? Guid.NewGuid();
    }

    public Parameter()
    {

    }

    public Guid Id { get; private set; }
    public string? Description { get; private set; }
    public string? Code { get; private set; }
    public string? Value { get; private set; }
    public bool Status { get; private set; }
    public Guid CustomerId { get; private set; }

    public Customer? Customer { get; set; }
}
