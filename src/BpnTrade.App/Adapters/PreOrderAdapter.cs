using BpnTrade.Domain.Adapters;
using BpnTrade.Domain.Dto;
using BpnTrade.Domain.Dto.Integration;
using BpnTrade.Domain.Roots;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var providerEndpoint = _configuration.GetSection("Providers:Bpn")["ProductsEndpointUri"];

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

                    var deserializedProducts = JsonConvert.DeserializeObject<PreOrderResponseDto>(content);

                    return ResultRoot.Success<PreOrderResponseDto>(deserializedProducts);

                }

                return ResultRoot.Failure<PreOrderResponseDto>(new ErrorDto("PRD001", "Products couldnt fetch"));
            }
        }
    }
}
