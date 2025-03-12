using Moq;
using Moq.Protected;

using System.Net;

namespace BpnTrade.App.Test.Mocks
{
    public class HttpClientFactoryMock
    {
        public static IHttpClientFactory CreateHttpClientFactory(HttpMethod httpMethod, HttpStatusCode httpStatusCode, string responseJson)
        {
            var response = new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(responseJson)
            };

            var _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                  ItExpr.Is<HttpRequestMessage>(req => req.Method == httpMethod),
                  ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var httpClient = new HttpClient(_httpMessageHandler.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            return httpClientFactoryMock.Object;
        }
    }
}
