using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Roots;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace BpnTrade.App.Adapters
{
    public class BpnProductAdapter : IProductAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public BpnProductAdapter(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ResultDto<ProductResponseDto>> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            var providerEndpoint = _configuration.GetSection("Providers:Bpn")["ProductsEndpointUri"];

            using (var client = _httpClientFactory.CreateClient())
            {
                var getResult = await client.GetAsync(providerEndpoint, cancellationToken);

                if (getResult.IsSuccessStatusCode)
                {
                    var content = await getResult.Content.ReadAsStringAsync(cancellationToken);

                    var deserializedProducts = JsonConvert.DeserializeObject<ProductResponseDto>(content);

                    return ResultRoot.Success<ProductResponseDto>(deserializedProducts);

                }

                return ResultRoot.Failure<ProductResponseDto>(new ErrorDto("PRD001", "Products couldnt fetch"));
            }
        }
    }
}
