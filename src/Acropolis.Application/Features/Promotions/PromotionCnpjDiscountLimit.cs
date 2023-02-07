using Acropolis.Application.Base.Extensions;
using Acropolis.Application.Base.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acropolis.Application.Features.Promotions;

public class PromotionCnpjDiscountLimit : Entity
{
    private readonly string _rawCnpj = null!;

    public PromotionCnpjDiscountLimit(string cnpj, decimal percent, Guid sellerId)
    {
        SellerId = sellerId;
        _rawCnpj = cnpj;
        Cnpj = cnpj.OnlyNumbers();
        Percent = percent;
    }

    private PromotionCnpjDiscountLimit()
    {
    }

    public Guid SellerId { get; set; }
    public string Cnpj { get; private set; } = null!;
    public decimal Percent { get; private set; }    
    [NotMapped]
    public string RawCnpj => _rawCnpj;

    public void UpdatePercent(decimal percent)
    {
        Percent = percent;
    }
}