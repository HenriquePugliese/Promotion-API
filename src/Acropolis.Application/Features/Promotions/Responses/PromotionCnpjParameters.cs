using Acropolis.Application.Base.Models;
using System.ComponentModel.DataAnnotations;

namespace Acropolis.Application.Features.Promotions.Responses;

public class PromotionCnpjParameters : Parameter
{
    [Required]
    public string ExternalId { get; set; } = null!;

    public IEnumerable<string>? Cnpjs { get; set; }
}