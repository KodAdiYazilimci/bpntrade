using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Entities.Integration;

namespace BpnTrade.Domain.Services
{
    public interface IProductService
    {
        Task<ResultDto<List<ProductEntity>>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}
