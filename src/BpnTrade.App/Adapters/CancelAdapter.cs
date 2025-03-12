using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Roots;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Polly;

using System.Text;

namespace BpnTrade.App.Adapters
{
    public class CancelAdapter : ICancelAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public CancelAdapter(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResultDto<CancelResponseDto>> CancelAsync(CancelRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                var providerEndpoint = _configuration.GetSection("Providers:Bpn")["CancelEndpointUri"];

                var retry =
                   Policy
                   .HandleResult<ResultDto<CancelResponseDto>>(x => !x.IsSuccess || !x.Data.Success)
                   .RetryAsync(3, (result, retryCount, context) =>
                   {

                   });

                var result = await retry.ExecuteAsync(async () =>
                {
                    var postResult =
                    await client.PostAsync(
                        providerEndpoint,
                        new StringContent(JsonConvert.SerializeObject(requestDto, new JsonSerializerSettings()
                                {
                                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                                }), Encoding.UTF8, "application/json"),
                        cancellationToken);

                    if (postResult.IsSuccessStatusCode || postResult.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        var content = await postResult.Content.ReadAsStringAsync(cancellationToken);

                        var deserializedCancellation = JsonConvert.DeserializeObject<CancelResponseDto>(content);

                        return
                        deserializedCancellation.Success
                        ?
                        ResultRoot.Success<CancelResponseDto>(deserializedCancellation)
                        :
                        ResultRoot.Failure<CancelResponseDto>(new ErrorDto("CNC001", deserializedCancellation.Message));

                    }

                    return ResultRoot.Failure<CancelResponseDto>(new ErrorDto("CNC001", "Products couldnt fetch"));
                });

                return result;
            }
        }
    }
}
