using Acropolis.Application.Features.Attributes.Requests;
using Acropolis.Application.Features.Customers.Requests;
using Acropolis.Consumer.Features.Attribute.Messages;
using Acropolis.Consumer.Features.Customer.Messages;
using AutoMapper;

namespace Acropolis.Consumer.Common.AutoMapper;

public class ConfiguringMapperProfile : Profile
{
    public ConfiguringMapperProfile()
    {
        CreateMap<AttributeCreatedMessage, CreateAttributeRequest>()
            .ReverseMap();

        CreateMap<CustomerCreatedMessage, CreateCustomerRequest>()
            .ReverseMap();

        CreateMap<CustomerChangedMessage, ChangeCustomerRequest>()
            .ReverseMap();

        CreateMap<CustomerRemovedMessage, RemoveCustomerRequest>()
            .ReverseMap();
    }
}