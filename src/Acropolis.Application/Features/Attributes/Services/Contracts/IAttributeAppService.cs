using Acropolis.Application.Base.Models;
using Acropolis.Application.Features.Attributes.Requests;

namespace Acropolis.Application.Features.Attributes.Services.Contracts;

public interface IAttributeAppService
{
    Task<(Response, Attribute?)> CreateAttributeConsumer(CreateAttributeRequest request, CancellationToken cancellationToken = default);

    Task<Response> RemoveAttributeByProduct(RemoveAttributeRequest request, CancellationToken cancellationToken = default);
}