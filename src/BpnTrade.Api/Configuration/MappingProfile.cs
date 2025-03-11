using AutoMapper;

using BpnTrade.Domain.Dto.Order;
using BpnTrade.Domain.Entities;

namespace BpnTrade.Api.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateOrderRequestDto, OrderEntity>()
                .ForMember(x => x.UserId, y => y.MapFrom(z => z.UserId))
                .ForMember(
                    x => x.OrderItems, 
                    y => y.MapFrom(z => 
                    z.OrderItems != null 
                        ? 
                        z.OrderItems.Select(x => new OrderItemEntity()
                        {
                            ProductId = x.ProductId,
                            Quantity = x.Quantity,
                            UnitPrice = x.UnitPrice
                        }).ToList() 
                        : default));
        }
    }
}
