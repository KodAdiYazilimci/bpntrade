using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Roots;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using System.Text;

namespace BpnTrade.App.Adapters
{
    public class CompleteAdapter : ICompleteAdapter
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public CompleteAdapter(
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResultDto<CompleteResponseDto>> CompleteAsync(CompleteRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var providerEndpoint = _configuration.GetSection("Providers:Bpn")["CompleteEndpointUri"];

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

                    var deserializedProducts = JsonConvert.DeserializeObject<CompleteResponseDto>(content);

                    return ResultRoot.Success<CompleteResponseDto>(deserializedProducts);

                }

                return ResultRoot.Failure<CompleteResponseDto>(new ErrorDto("PRD001", "Products couldnt fetch"));
            }
        }
    }
}
