namespace Acropolis.Application.Features.Promotions.Responses;
public class PromotionParameterResponse
{
    public PromotionParameterResponse(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; private set; }
    public string Value { get; private set; }
}

