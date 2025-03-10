using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Entities.Integration;
using BpnTrade.Domain.Roots;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace BpnTrade.App.Adapters
{
    public class BalanceAdapter : IBalanceAdapter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public BalanceAdapter(
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ResultDto<BalanceEntity>> GetUserBalanceAsync(string userId, CancellationToken cancellationToken = default)
        {
            var providerEndpoint = _configuration.GetSection("Providers:Bpn")["UserBalanceEndpointUri"];

            using (var client = _httpClientFactory.CreateClient())
            {
                var getResult = await client.GetAsync(providerEndpoint, cancellationToken);

                if (getResult.IsSuccessStatusCode)
                {
                    var content = await getResult.Content.ReadAsStringAsync(cancellationToken);

                    var deserializedProducts = JsonConvert.DeserializeObject<BalanceEntity>(content);

                    return ResultRoot.Success<BalanceEntity>(deserializedProducts);

                }

                return ResultRoot.Failure<BalanceEntity>(new ErrorDto("BLC001", "Balance info couldnt fetch"));
            }
        }
    }
}
