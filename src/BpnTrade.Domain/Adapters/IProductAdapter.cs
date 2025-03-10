using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Entities.Integration;

namespace BpnTrade.Domain.Adapters
{
    public interface IProductAdapter
    {
        Task<ResultDto<List<ProductEntity>>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}
