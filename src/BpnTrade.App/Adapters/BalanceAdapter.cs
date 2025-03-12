using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Roots;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using Polly;

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

            var retry =
                Policy
                .HandleResult<ResultDto<BalanceResponseDto>>(x => !x.IsSuccess || !x.Data.Success)
                .RetryAsync(3, (result, retryCount, context) =>
                {

                });

            var result = await retry.ExecuteAsync(async () =>
            {
                using (var client = _httpClientFactory.CreateClient())
                {
                    QueryBuilder query = new QueryBuilder();
                    query.Append(new KeyValuePair<string, string>("userId", requestDto.UserId));

                    Uri uri = new Uri(providerEndpoint + query.ToString());

                    var getResult = await client.GetAsync(uri, cancellationToken);

                    if (getResult.IsSuccessStatusCode || getResult.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var content = await getResult.Content.ReadAsStringAsync(cancellationToken);

                        var deserializedBalance = JsonConvert.DeserializeObject<BalanceResponseDto>(content);

                        return deserializedBalance.Success
                        ?
                        ResultRoot.Success<BalanceResponseDto>(deserializedBalance)
                        :
                        ResultRoot.Failure<BalanceResponseDto>(new ErrorDto("BLC001", deserializedBalance.Message));
                    }

                    return ResultRoot.Failure<BalanceResponseDto>(new ErrorDto("BLC002", "Balance info couldnt fetch"));
                }
            });

            return result;
        }
    }
}
