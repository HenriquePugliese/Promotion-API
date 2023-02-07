using Acropolis.Application.Features.Products.Requests;

namespace Acropolis.Application.Features.Products;

public class Product
{
    public Product(CreateProductRequest productRequest)
    {
        Id = productRequest.Id;
        SellerId = productRequest.SellerId;
        Name = productRequest.Name;
        MaterialCode = productRequest.MaterialCode;
        UnitMeasure = productRequest.UnitMeasure;
        Weight = productRequest.Weight;
        Status = productRequest.Status;
        UnitWeight = productRequest.UnitWeight;
    }

    private Product()
    {
    }

    public Guid Id { get; private set; }
    public Guid SellerId { get; private set; }
    public string? Name { get; private set; } = null!;
    public string? MaterialCode { get; private set; } = null!;
    public string? UnitMeasure { get; private set; } = null!;
    public string? UnitWeight { get; private set; } = null!;
    public decimal Weight { get; private set; }
    public short Status { get; private set; }

    public void Update(UpdateProductRequest productRequest)
    {
        SellerId = productRequest.SellerId;
        Name = productRequest.Name;
        MaterialCode = productRequest.MaterialCode;
        UnitMeasure = productRequest.UnitMeasure;
        Weight = productRequest.Weight;
        Status = productRequest.Status;
        UnitWeight = productRequest.UnitWeight;
    }
}
