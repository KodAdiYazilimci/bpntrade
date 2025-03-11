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
    public class PreOrderAdapter : IPreOrderAdapter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PreOrderAdapter(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ResultDto<PreOrderResponseDto>> PreOrderAsync(PreOrderRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            var providerEndpoint = _configuration.GetSection("Providers:Bpn")["PreOrderEndpointUri"];

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

                    var deserializedPreOrder = JsonConvert.DeserializeObject<PreOrderResponseDto>(content);

                    return
                        deserializedPreOrder.Success
                        ?
                        ResultRoot.Success<PreOrderResponseDto>(deserializedPreOrder)
                        :
                        ResultRoot.Failure<PreOrderResponseDto>(new ErrorDto("PRE001", deserializedPreOrder.Message));
                }

                return ResultRoot.Failure<PreOrderResponseDto>(new ErrorDto("PRE001", "Preorder couldnt started"));
            }
        }
    }
}
