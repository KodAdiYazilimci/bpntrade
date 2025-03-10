using AutoMapper;

using BpnTrade.Domain.Dto.Order;
using BpnTrade.Domain.Entities;
using BpnTrade.Domain.Entities.Persistence;

namespace BpnTrade.Api.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOrderRequestDto, OrderEntity>()
                .ForMember(x => x.CustomerId, y => y.MapFrom(z => z.CustomerId))
                .ForMember(
                    x => x.OrderItems, 
                    y => y.MapFrom(z => 
                    z.OrderItems != null 
                        ? 
                        z.OrderItems.Select(x => new OrderItemEntity()
                        {
                            ProductId = x.ProductId,
                            Quantity = x.Quantity
                        }).ToList() 
                        : default));
        }
    }
}
