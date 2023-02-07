using Acropolis.Application.Base.Models;
using Acropolis.Application.Features.Promotions.Enums;
using Acropolis.Application.Features.Promotions.Requests;

namespace Acropolis.Application.Features.Promotions;

public class Promotion : Entity
{
    public Promotion(CreatePromotionRequest promotionRequest, Guid sellerId, Guid? id = null) : base(id)
    {
        SellerId = sellerId;
        Name = promotionRequest.Name;
        DtStart = promotionRequest.DtStart;
        DtEnd = promotionRequest.DtEnd;
        ExternalId = promotionRequest.ExternalId;
        UnitMeasurement = promotionRequest.UnitMeasurement;
        Parameters = (List<PromotionParameter>)promotionRequest;
        Rules = promotionRequest.Rules.Select(ruleRequest => (PromotionRule)ruleRequest).ToList();
        Attributes = promotionRequest.Attributes.Select(attributeRequest => (PromotionAttribute)attributeRequest).ToList();
        Status = PromotionStatus.Active;
    }

    public Promotion()
    {
        Parameters = new List<PromotionParameter>();
        Attributes = new List<PromotionAttribute>();
        Rules = new List<PromotionRule>();
        Cnpjs = new List<PromotionCnpj>();
    }

    public Guid SellerId { get; set; }
    public string Name { get; private set; } = null!;
    public string ExternalId { get; private set; } = null!;
    public string UnitMeasurement { get; private set; } = null!;
    public DateTime DtStart { get; private set; }
    public DateTime DtEnd { get; private set; }
    public PromotionStatus Status { get; private set; }
    public List<PromotionParameter> Parameters { get; private set; }
    public List<PromotionAttribute> Attributes { get; private set; }
    public List<PromotionRule> Rules { get; private set; }
    public List<PromotionCnpj>? Cnpjs { get; set; }
}