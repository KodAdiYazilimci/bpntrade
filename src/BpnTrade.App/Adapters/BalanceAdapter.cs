using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Roots;

using Microsoft.AspNetCore.Http.Extensions;
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

        public async Task<ResultDto<BalanceResponseDto>> GetUserBalanceAsync(BalanceRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var providerEndpoint = _configuration.GetSection("Providers:Bpn")["UserBalanceEndpointUri"];

            using (var client = _httpClientFactory.CreateClient())
            {
                QueryBuilder query = new QueryBuilder();
                query.Append(new KeyValuePair<string, string>("userId", requestDto.UserId));

                Uri uri = new Uri(providerEndpoint + query.ToString());

                var getResult = await client.GetAsync(uri, cancellationToken);

                if (getResult.IsSuccessStatusCode)
                {
                    var content = await getResult.Content.ReadAsStringAsync(cancellationToken);

                    var deserializedProducts = JsonConvert.DeserializeObject<BalanceResponseDto>(content);

                    return ResultRoot.Success<BalanceResponseDto>(deserializedProducts);

                }

                return ResultRoot.Failure<BalanceResponseDto>(new ErrorDto("BLC001", "Balance info couldnt fetch"));
            }
        }
    }
}
