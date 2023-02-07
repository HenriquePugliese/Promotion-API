using Acropolis.Application.Base.Models;
using Acropolis.Application.Features.Products.Requests;

namespace Acropolis.Application.Features.Products.Services.Contracts
{
    public interface IProductAppService
    {
        Task<(Response, Product?)> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken = default);
        Task<(Response, Product?)> UpdateProduct(UpdateProductRequest request, CancellationToken cancellationToken = default);
        Task<Response> RemoveProduct(RemoveProductRequest request, CancellationToken cancellationToken = default);
    }
}
