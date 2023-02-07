namespace Acropolis.Application.Features.Promotions.Requests;

public class CreatePromotionParameterRequest
{
    public CreatePromotionParameterRequest()
    {
        UF = Array.Empty<string>();
        MesoRegiao = Array.Empty<string>();
        MicroRegiao = Array.Empty<string>();
        GrupoSegmento = Array.Empty<string>();
        Segmento = Array.Empty<string>();
    }

    public IEnumerable<string> UF { get; set; }
    public IEnumerable<string> MesoRegiao { get; set; }
    public IEnumerable<string> MicroRegiao { get; set; }
    public IEnumerable<string> GrupoSegmento { get; set; }
    public IEnumerable<string> Segmento { get; set; }
}