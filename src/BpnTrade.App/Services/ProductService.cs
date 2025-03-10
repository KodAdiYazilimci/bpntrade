using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Roots;
using BpnTrade.Domain.Services;

using Microsoft.Extensions.Caching.Memory;

namespace BpnTrade.App.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductAdapter _productAdapter;
        private readonly IMemoryCache _memoryCache;

        public ProductService(
            IProductAdapter productAdapter,
            IMemoryCache memoryCache)
        {
            _productAdapter = productAdapter;
            _memoryCache = memoryCache;
        }

        public async Task<ResultDto<List<ProductResponseDto>>> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            if (_memoryCache.TryGetValue("Products", out List<ProductResponseDto> _products) && _products.Any())
            {
                return ResultRoot.Success(_products);
            }

            var products = await _productAdapter.GetProductsAsync(cancellationToken);

            return
                products != null && products.IsSuccess
                ?
                ResultRoot.Success<List<ProductResponseDto>>(products.Data)
                : 
                ResultRoot.Failure<List<ProductResponseDto>>(products.Error);
        }
    }
}
