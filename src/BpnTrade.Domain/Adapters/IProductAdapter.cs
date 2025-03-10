using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;

namespace BpnTrade.Domain.Adapters
{
    public interface IProductAdapter
    {
        Task<ResultDto<List<ProductResponseDto>>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}
