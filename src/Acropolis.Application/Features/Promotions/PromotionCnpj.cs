using Acropolis.Application.Base.Extensions;
using Acropolis.Application.Base.Models;

namespace Acropolis.Application.Features.Promotions;

public class PromotionCnpj : Entity
{
    public PromotionCnpj(string externalId, string cnpj, Guid sellerId, Promotion? promotion = null) : base(Guid.Empty)
    {
        SellerId = sellerId;
        ExternalId = externalId;
        Cnpj = cnpj.OnlyNumbers();
        Promotion = promotion;
        PromotionId = promotion?.Id ?? Guid.Empty;
    }

    private PromotionCnpj()
    {
    }

    public Guid SellerId { get; set; }
    public string ExternalId { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;
    public Guid PromotionId { get; set; }
    public virtual Promotion? Promotion { get; set; }
}