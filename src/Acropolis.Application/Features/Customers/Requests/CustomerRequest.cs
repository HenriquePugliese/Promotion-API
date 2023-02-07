using Acropolis.Application.Features.Parameters.Requests;

namespace Acropolis.Application.Features.Customers.Requests;

public class CustomerRequest
{
    public string? SellerId { get; set; }
    public string? Cnpj { get; set; }
    public string? CustomerCode { get; set; }
    public List<ParameterRequest>? Parameters { get; set; }
}
