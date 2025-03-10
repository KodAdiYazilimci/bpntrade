using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Product;
using BpnTrade.Domain.Services;

namespace BpnTrade.App.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductAdapter _productAdapter;

        public ProductService(IProductAdapter productAdapter)
        {
            _productAdapter = productAdapter;
        }

        public Task<ResultDto<List<ProductDto>>> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            return _productAdapter.GetProductsAsync(cancellationToken);
        }
    }
}
