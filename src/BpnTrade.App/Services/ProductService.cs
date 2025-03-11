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

        public async Task<ResultDto<ProductResponseDto>> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            if (_memoryCache.TryGetValue("Products", out ProductResponseDto _productResponse) && _productResponse != null)
            {
                return ResultRoot.Success(_productResponse);
            }

            var products = await _productAdapter.GetProductsAsync(cancellationToken);

            _memoryCache.Set<ProductResponseDto>("Products", products.Data, TimeSpan.FromSeconds(30));

            return
                products != null && products.IsSuccess
                ?
                ResultRoot.Success<ProductResponseDto>(products.Data)
                :
                ResultRoot.Failure<ProductResponseDto>(products.Error);
        }
    }
}
