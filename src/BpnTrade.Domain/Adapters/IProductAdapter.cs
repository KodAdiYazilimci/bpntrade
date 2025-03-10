using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Product;

namespace BpnTrade.Domain.Adapters
{
    public interface IProductAdapter
    {
        Task<ResultDto<List<ProductDto>>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}
