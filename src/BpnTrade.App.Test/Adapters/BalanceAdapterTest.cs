using BpnTrade.App.Adapters;
using BpnTrade.Domain.Dto.Integration;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using System.Text;
using BpnTrade.App.Test.Mocks;
using System.Net;

namespace BpnTrade.App.Test.Adapters
{
    [TestClass]
    public class BalanceAdapterTest
    {
        private IConfiguration configuration;

        [TestInitialize]
        public void Init()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(@"{
                ""Providers"": {
                    ""Bpn"": {
                      ""CancelEndpointUri"": ""https://mock/cancel"",
                      ""UserBalanceEndpointUri"": ""https://mock"",
                      ""ProductsEndpointUri"": ""https://balance-management-pi44.onrender.com/api/products"",
                      ""CompleteEndpointUri"": ""https://mock/complete"",
                      ""PreOrderEndpointUri"": ""https://mock/preorder""
                    }
                  }
                }")));

            configuration = configurationBuilder.Build();
        }

        [TestMethod]
        public async Task When_Get_Call_Failed()
        {
            var httpClientFactory =
                 HttpClientFactoryMock
                 .CreateHttpClientFactory(
                     httpMethod: HttpMethod.Get,
                     httpStatusCode: HttpStatusCode.InternalServerError,
                     responseJson: JsonConvert.SerializeObject(new { }));

            var balanceAdapter = new BalanceAdapter(
                httpClientFactory: httpClientFactory,
                configuration: configuration);

            var userBalanceResult = await balanceAdapter.GetUserBalanceAsync(new BalanceRequestDto()
            {
                UserId = "0"
            }, CancellationToken.None);

            Assert.IsTrue(!userBalanceResult.IsSuccess && userBalanceResult.Error?.Code == "BLC002");
        }

        [TestMethod]
        public async Task When_Get_Call_Succeed()
        {
            var httpClientFactory =
                 HttpClientFactoryMock
                 .CreateHttpClientFactory(
                     httpMethod: HttpMethod.Get,
                     httpStatusCode: HttpStatusCode.OK,
                     responseJson: @"{
                                      ""success"": true,
                                      ""data"": {
                                        ""userId"": ""550e8400-e29b-41d4-a716-446655440000"",
                                        ""totalBalance"": 10000000000,
                                        ""availableBalance"": 10000000000,
                                        ""blockedBalance"": 0,
                                        ""currency"": ""USD"",
                                        ""lastUpdated"": ""2023-06-15T10:30:00Z""
                                      }
                                    }");

            var balanceAdapter = new BalanceAdapter(
                httpClientFactory: httpClientFactory,
                configuration: configuration);

            var userBalanceResult = await balanceAdapter.GetUserBalanceAsync(new BalanceRequestDto()
            {
                UserId = "0"
            }, CancellationToken.None);

            Assert.IsTrue(userBalanceResult.IsSuccess && userBalanceResult.Data.Success);
        }
    }
}
