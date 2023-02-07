using Acropolis.Application.Features.Attributes.Requests;
using Acropolis.Application.Features.Customers;
using Acropolis.Application.Features.Products;
using Acropolis.Application.Features.Products.Requests;
using Acropolis.Application.Features.Promotions;
using Acropolis.Application.Features.Promotions.Requests;
using Attribute = Acropolis.Application.Features.Attributes.Attribute;
using Parameter = Acropolis.Application.Features.Parameters.Parameter;

namespace Acropolis.Infrastructure.Contexts;

public class AcropolisDatabaseSeed
{
    private readonly AcropolisContext _context;

    public static readonly Guid SellerId_Votorantim = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d");

    public AcropolisDatabaseSeed(AcropolisContext context)
    {
        _context = context;
    }

    public void SeedData()
    {
        SeedPromotions();
        SeedPromotionsCnpjs();
        SeedPromotionsCnpjsDiscountLimits();
        SeedProductsWithAttributes();
        SeedCustomersWithParameters();
    }

    #region [ Promotions ]

    private void SeedPromotions()
    {
        if (!_context.Promotions.Any())
        {
            var promotion1 = new Promotion(new CreatePromotionRequest()
            {
                Name = "Promotion Test 1",
                ExternalId = "ext-ok-1",
                DtStart = DateTime.Today,
                DtEnd = DateTime.Today.AddDays(10),
                UnitMeasurement = "kg",
                Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = "1", Qtd = 1 } },
                Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 1, TotalAttributes = 1, GreaterEqualValue = 1 } }
            }, SellerId_Votorantim);

            var promotion2 = new Promotion(new CreatePromotionRequest()
            {
                Name = "Promotion Test 2",
                ExternalId = "ext-ok-2",
                DtStart = DateTime.Today,
                DtEnd = DateTime.Today.AddDays(20),
                UnitMeasurement = "kg",
                Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = "2", Qtd = 2 } },
                Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 2, TotalAttributes = 2, GreaterEqualValue = 2 } }
            }, SellerId_Votorantim);

            var promotion3 = new Promotion(new CreatePromotionRequest()
            {
                Name = "Promotion Test 3",
                ExternalId = "ext-ok-3",
                DtStart = DateTime.Today,
                DtEnd = DateTime.Today.AddDays(30),
                UnitMeasurement = "kg",
                Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = "3", Qtd = 3 } },
                Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 3, TotalAttributes = 3, GreaterEqualValue = 3 } }
            }, SellerId_Votorantim);

            var promotion4 = new Promotion(new CreatePromotionRequest()
            {
                Name = "Promotion Test 4",
                ExternalId = "ext-ok-4",
                DtStart = DateTime.Today,
                DtEnd = DateTime.Today.AddDays(30),
                UnitMeasurement = "kg",
                Attributes = new List<CreatePromotionAttributeRequest>() { new CreatePromotionAttributeRequest() { AttributesId = "2C7A038F-19FC-4B99-94BB-E79DCDCE1F11", Qtd = 1 } },
                Rules = new List<CreatePromotionRuleRequest>() { new CreatePromotionRuleRequest() { Percentage = 3, TotalAttributes = 3, GreaterEqualValue = 3 } }
            }, SellerId_Votorantim);

            _context.Promotions.AddRange(promotion1, promotion2, promotion3, promotion4);
            _context.SaveChanges();
        }
    }

    #endregion

    #region [ PromotionsCnpjs ]

    private void SeedPromotionsCnpjs()
    {
        if (!_context.PromotionsCnpjs.Any())
        {
            var promotion = _context.Promotions.FirstOrDefault();

            var promotionCnpj1 = new PromotionCnpj("ext-ok-1", "72527073000147", SellerId_Votorantim, promotion);
            var promotionCnpj2 = new PromotionCnpj("ext-ok-1", "92723795000184", SellerId_Votorantim, promotion);
            var promotionCnpj3 = new PromotionCnpj("ext-ok-1", "95251634000123", SellerId_Votorantim, promotion);

            _context.PromotionsCnpjs.AddRange(promotionCnpj1, promotionCnpj2, promotionCnpj3);
            _context.SaveChanges();
        }
    }

    #endregion

    #region [ PromotionsCnpjsDiscountLimits ]

    private void SeedPromotionsCnpjsDiscountLimits()
    {
        if (_context.PromotionsCnpjsDiscountLimits.Any())
            return;
        
        var promotionCnpjDiscountLimit1 = new PromotionCnpjDiscountLimit("72527073000147", 5, SellerId_Votorantim);
        var promotionCnpjDiscountLimit2 = new PromotionCnpjDiscountLimit("92723795000184", 10, SellerId_Votorantim);
        var promotionCnpjDiscountLimit3 = new PromotionCnpjDiscountLimit("95251634000123", 15, SellerId_Votorantim);

        _context.PromotionsCnpjsDiscountLimits.AddRange(promotionCnpjDiscountLimit1, promotionCnpjDiscountLimit2, promotionCnpjDiscountLimit3);
        _context.SaveChanges();        
    }

    #endregion

    #region [ Products And Attributes]

    private void SeedProductsWithAttributes()
    {
        if (_context.Products.Any() || _context.Attributes.Any())
            return;

        var product1 = new Product(new CreateProductRequest(){
            Id = Guid.NewGuid(),
            SellerId = SellerId_Votorantim,
            MaterialCode = "mtc-1",
            Name = "Produto 1",            
            Status = 1,
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10
        });

        var product2 = new Product(new CreateProductRequest()
        {
            Id = Guid.NewGuid(),
            SellerId = SellerId_Votorantim,
            MaterialCode = "mtc-2",
            Name = "Produto 2",            
            Status = 1,
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10
        });

        var product3 = new Product(new CreateProductRequest()
        {
            Id = Guid.NewGuid(),
            SellerId = Guid.Parse("d0164e85-8bfc-4138-37e3-08d68bda781d"),
            MaterialCode = "sku1",
            Name = "Produto 3",
            Status = 1,
            UnitMeasure = "kg",
            UnitWeight = "10",
            Weight = 10
        });

        var attribute1 = new Attribute(new CreateAttributeRequest()
        {
            ProductId = product1.Id,
            AttributeKey = "attr-1",
            AttributeKeyId = Guid.NewGuid(),
            AttributeKeyDescription = "attr-1",
            AttributeKeyLabel = "attr-1",
            AttributeKeyIsBeginOpen = true,
            AttributeKeyStatus = 1,
            AttributeKeyType = Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = "attr-1",
            AttributeValueId = new Guid("2C7A038F-19FC-4B99-94BB-E79DCDCE1F11"),
            AttributeValueLabel = "attr-1",
            AttributeValueStatus = 1
        });

        var attribute2 = new Attribute(new CreateAttributeRequest()
        {
            ProductId = product2.Id,
            AttributeKey = "attr-2",
            AttributeKeyId = Guid.NewGuid(),
            AttributeKeyDescription = "attr-2",
            AttributeKeyLabel = "attr-2",
            AttributeKeyIsBeginOpen = true,
            AttributeKeyStatus = 1,
            AttributeKeyType = Application.Features.Attributes.Enums.FilterType.Single,
            AttributeValue = "attr-2",
            AttributeValueId = new Guid("766679ea-8b45-41fa-a2a5-294aad64b2dd"),
            AttributeValueLabel = "attr-2",
            AttributeValueStatus = 1
        });

        _context.Products.AddRange(product1, product2, product3);
        _context.Attributes.AddRange(attribute1, attribute2);
        _context.SaveChanges();
    }

    #endregion

    #region [ Customers with parameters ]

    private void SeedCustomersWithParameters()
    {
        if (_context.Customers.Any())
            return;

        var customer1 = new Customer("14461346000100", SellerId_Votorantim.ToString(), "acb-cc-1")
        {
            Parameters = new List<Acropolis.Application.Features.Parameters.Parameter>() {
                new Acropolis.Application.Features.Parameters.Parameter("São Paulo", "UF", "SP", true, new Guid("7E389E1D-0D3B-45F9-A7D7-066A9D35164E"), null)
            }
        };
        var customer2 = new Customer("73074254000128", SellerId_Votorantim.ToString(), "acb-cc-2");
        var customer3 = new Customer("72527073000147", SellerId_Votorantim.ToString(), "acb-cc-3");
        var customer4 = new Customer("97194593000106", SellerId_Votorantim.ToString(), "acb-cc-4");

        var customer1Parameter = new Parameter("a", "1", "1", true, customer1.Id);
        var customer2Parameter = new Parameter("b", "2", "2", true, customer2.Id);
        var customer3Parameter = new Parameter("Santa Catarina", "UF", "SC", true, customer3.Id);
        
        _context.Customers.AddRange(customer1, customer2, customer3, customer4);
        _context.Parameters.AddRange(customer1Parameter, customer2Parameter, customer3Parameter);
        _context.SaveChanges();
    }

    #endregion
}
