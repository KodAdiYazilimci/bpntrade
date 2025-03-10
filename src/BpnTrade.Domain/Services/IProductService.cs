using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Product;

namespace BpnTrade.Domain.Services
{
    public interface IProductService
    {
        Task<ResultDto<List<ProductDto>>> GetProductsAsync(CancellationToken cancellationToken = default);
    }
}
