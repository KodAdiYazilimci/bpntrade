using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Product;
using BpnTrade.Domain.Roots;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace BpnTrade.App.Adapters
{
    public class BpnProductAdapter : IProductAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public BpnProductAdapter(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ResultDto<List<ProductDto>>> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            var providerEndpoint = _configuration.GetSection("Providers:Bpn")["ProductsEndpointUri"];

            var getResult = await _httpClient.GetAsync(providerEndpoint, cancellationToken);

            if (getResult.IsSuccessStatusCode)
            {
                var content = await getResult.Content.ReadAsStringAsync(cancellationToken);

                var deserializedProducts = JsonConvert.DeserializeObject<List<ProductDto>>(content);

                return ResultRoot.Success<List<ProductDto>>(deserializedProducts);

            }

            return ResultRoot.Failure<List<ProductDto>>(new ErrorDto("PRD001", "Products couldnt fetch"));
        }
    }
}
