using Acropolis.Application.Base.Extensions;

namespace Acropolis.Application.Features.Promotions.Requests;

public class RemovePromotionCnpjRequest
{
    public RemovePromotionCnpjRequest(string externalId, string cnpj)
    {
        ExternalId = externalId;
        Cnpj = cnpj;
    }

    public string ExternalId { get; private set; }

    public string Cnpj { get; private set; }
}
