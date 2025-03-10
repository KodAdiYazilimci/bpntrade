using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Roots;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

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
            var providerEndpoint = _configuration.GetSection("Providers:Bpn")["CancelEndpointUri"];

            using (var client = _httpClientFactory.CreateClient())
            {
                var getResult =
                    await client.PostAsync(
                        providerEndpoint,
                        new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json"),
                        cancellationToken);

                if (getResult.IsSuccessStatusCode)
                {
                    var content = await getResult.Content.ReadAsStringAsync(cancellationToken);

                    var deserializedProducts = JsonConvert.DeserializeObject<CancelResponseDto>(content);

                    return ResultRoot.Success<CancelResponseDto>(deserializedProducts);

                }

                return ResultRoot.Failure<CancelResponseDto>(new ErrorDto("PRD001", "Products couldnt fetch"));
            }
        }
    }
}
