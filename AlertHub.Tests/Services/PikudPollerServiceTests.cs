using AlertHub.Api.Options;
using AlertHub.Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;

namespace AlertHub.Tests.Services;

public class PikudPollerServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly IOptions<PikudPollerOptions> _options;
    private readonly Mock<ILogger<PikudPollerService>> _loggerMock = new Mock<ILogger<PikudPollerService>>();
    public PikudPollerServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        var client = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        _options = Options.Create(new PikudPollerOptions { PikudApiUrl = "https://api.test/alerts" });
    }

    [Fact]
    public async Task GetAlertsAsJson_ShouldReturnAlerts_WhenResponseIsValid()
    {
        // Arrange
        var jsonResponse = """{"data":["Alert 1","Alert 2"]}""";
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        var service = new PikudPollerService(_httpClientFactoryMock.Object, _options, _loggerMock.Object);

        // Act
        var result = await service.GetAlertsAsJson(CancellationToken.None);

        // Assert
        Assert.Equal("""{"data":["Alert 1","Alert 2"]}""", result);
    }

    [Fact]
    public async Task GetAlertsAsJson_ShouldReturnEmpty_WhenResponseIsEmpty()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("") // Empty content
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(responseMessage);

        var service = new PikudPollerService(_httpClientFactoryMock.Object, _options, _loggerMock.Object);

        // Act
        var result = await service.GetAlertsAsJson(CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}
