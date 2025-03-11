using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Roots;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

                    var deserializedCompletion = JsonConvert.DeserializeObject<CompleteResponseDto>(content);

                    return
                        deserializedCompletion.Success
                        ?
                        ResultRoot.Success<CompleteResponseDto>(deserializedCompletion)
                        :
                        ResultRoot.Failure<CompleteResponseDto>(new ErrorDto("CMP001", deserializedCompletion.Message));

                }

                return ResultRoot.Failure<CompleteResponseDto>(new ErrorDto("PRD001", "Products couldnt fetch"));
            }
        }
    }
}
