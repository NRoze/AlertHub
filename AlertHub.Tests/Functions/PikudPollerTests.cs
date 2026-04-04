using AlertHub.Api.Functions;
using AlertHub.Api.Options;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;

namespace AlertHub.Tests.Functions;

public class PikudPollerTests
{
    private readonly Mock<IPikudPollerService> _serviceMock;
    private readonly Mock<IAlertCache> _cacheMock;
    private readonly Mock<IConnectionMultiplexer> _connectionMock;
    private readonly Mock<ISubscriber> _subscriberMock;
    private readonly Mock<FunctionContext> _contextMock;

    public PikudPollerTests()
    {
        _serviceMock = new Mock<IPikudPollerService>();
        _cacheMock = new Mock<IAlertCache>();
        _connectionMock = new Mock<IConnectionMultiplexer>();
        _subscriberMock = new Mock<ISubscriber>();

        _connectionMock.Setup(c => c.GetSubscriber(It.IsAny<object>())).Returns(_subscriberMock.Object);

        _contextMock = new Mock<FunctionContext>();
        
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        _contextMock.SetupProperty(c => c.InstanceServices, serviceProvider);
    }

    [Fact]
    public async Task Run_ShouldFetchAlertsAndCacheThem()
    {
        // Arrange
        var alerts = new List<string> { "{\"id\":\"1\"}" };
        _serviceMock.Setup(s => s.GetAlertsAsJson(It.IsAny<CancellationToken>()))
            .ReturnsAsync(alerts);
        
        _cacheMock.Setup(c => c.TryAddAsync("1", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var options = Options.Create(new RedisOptions());
        var poller = new PikudPoller(_serviceMock.Object, _cacheMock.Object, _connectionMock.Object, options);
        var timerInfo = new TimerInfo();

        // Act
        await poller.Run(timerInfo, _contextMock.Object, CancellationToken.None);
        
        // Assert
        _serviceMock.Verify(s => s.GetAlertsAsJson(It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(c => c.TryAddAsync("1",  It.IsAny<CancellationToken>()), Times.Once);
        _subscriberMock.Verify(s => s.PublishAsync(It.IsAny<RedisChannel>(), "{\"id\":\"1\"}", It.IsAny<CommandFlags>()), Times.Once);
    }
}
