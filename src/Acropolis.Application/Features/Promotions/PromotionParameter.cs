using Acropolis.Application.Base.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Acropolis.Application.Features.Promotions;

public class PromotionParameter : Entity
{
    public PromotionParameter(Enums.PromotionParameter kind, string value, Promotion? promotion = null)
    {
        Name = kind.ToString();
        Value = value;
        Promotion = promotion;
        PromotionId = promotion?.Id ?? Guid.Empty;
    }    

    private PromotionParameter()
    {
    }

    public string? Name { get; private set; }
    public string? Value { get; private set; }
    [NotMapped]
    public Enums.PromotionParameter? Kind { get { return string.IsNullOrWhiteSpace(Name) ? null : Enum.Parse<Enums.PromotionParameter>(Name); } }
    public Guid PromotionId { get; set; }
    public virtual Promotion? Promotion { get; set; }
}