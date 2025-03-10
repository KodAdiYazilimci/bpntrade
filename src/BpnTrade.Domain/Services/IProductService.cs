using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;

namespace BpnTrade.Domain.Services
{
    public interface IProductService
    {
        Task<ResultDto<List<ProductResponseDto>>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}
