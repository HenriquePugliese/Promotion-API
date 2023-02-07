using System.ComponentModel.DataAnnotations;

namespace Acropolis.Application.Features.Promotions.Enums;

public enum PromotionStatus : short
{
    [Display(Name = "Active")]
    Active = 1,
    [Display(Name = "Disabled")]
    Disabled = 2
}
