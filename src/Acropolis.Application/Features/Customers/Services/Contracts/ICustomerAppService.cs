using Acropolis.Application.Base.Models;
using Acropolis.Application.Features.Customers.Requests;

namespace Acropolis.Application.Features.Customers.Services.Contracts;

public interface ICustomerAppService
{
    Task<(Response, Customer?)> CreateCustomer(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<(Response, Customer?)> ChangeCustomerParameters(ChangeCustomerRequest request, CancellationToken cancellationToken = default);
    Task<Response> RemoveCustomer(RemoveCustomerRequest request, CancellationToken cancellationToken = default);
}
