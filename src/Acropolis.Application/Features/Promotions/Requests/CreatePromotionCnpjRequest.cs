using Acropolis.Application.Base.Extensions;
using System.Text.Json.Serialization;

namespace Acropolis.Application.Features.Promotions.Requests;

public class CreatePromotionCnpjRequest
{
    private IEnumerable<string> _cnpjs;

    public CreatePromotionCnpjRequest()
    {
        _cnpjs = Array.Empty<string>();
    }

    public string ExternalId { get; set; } = null!;
    
    [JsonPropertyName("CNPJs")]
    public IEnumerable<string> Cnpjs { 
        get { return _cnpjs.Select(cnpj => cnpj.OnlyNumbers()).Distinct(); }
        set { _cnpjs = value; }
    }
}