using AlertHub.Api.Functions;
using AlertHub.Api.Models;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;

namespace AlertHub.Tests.Functions;

public class PikudPollerTests
{
    private readonly Mock<IPikudPollerService> _serviceMock;
    private readonly Mock<IAlertCache> _cacheMock;
    private readonly Mock<TimeProvider> _timeProviderMock;
    private readonly Mock<FunctionContext> _contextMock;

    public PikudPollerTests()
    {
        _serviceMock = new Mock<IPikudPollerService>();
        _cacheMock = new Mock<IAlertCache>();
        _timeProviderMock = new Mock<TimeProvider>();
        _contextMock = new Mock<FunctionContext>();

        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        _contextMock.SetupProperty(c => c.InstanceServices, serviceProvider);
    }

    [Fact]
    public async Task Run_ShouldReturnPingAction_WhenPayloadEmpty()
    {
        _serviceMock.Setup(s => s.GetAlertsAsJson(It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        var poller = new PikudPoller(_serviceMock.Object, _cacheMock.Object, _timeProviderMock.Object);
        var timerInfo = new TimerInfo();

        var result = await poller.Run(timerInfo, _contextMock.Object, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("ping", result.Target);
        Assert.Contains("heartbeat", result.Arguments);
    }

    [Fact]
    public async Task Run_ShouldReturnNewAlertAction_WhenCacheAddsIt()
    {
        var alertDto = new AlertMessageDto { Id = "test-1" };
        var json = JsonSerializer.Serialize(alertDto);
        
        _serviceMock.Setup(s => s.GetAlertsAsJson(It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);
        
        _cacheMock.Setup(c => c.TryAdd(It.IsAny<AlertMessageDto>())).Returns(true);

        var poller = new PikudPoller(_serviceMock.Object, _cacheMock.Object, _timeProviderMock.Object);
        var timerInfo = new TimerInfo();

        var result = await poller.Run(timerInfo, _contextMock.Object, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("newAlert", result.Target);
        var arg = result.Arguments[0] as AlertMessageDto;
        Assert.NotNull(arg);
        Assert.Equal("test-1", arg.Id);
    }

    [Fact]
    public async Task Run_ShouldReturnPingAction_WhenCacheRejectsIt()
    {
        var alertDto = new AlertMessageDto { Id = "test-1" };
        var json = JsonSerializer.Serialize(alertDto);
        
        _serviceMock.Setup(s => s.GetAlertsAsJson(It.IsAny<CancellationToken>()))
            .ReturnsAsync(json);
        
        _cacheMock.Setup(c => c.TryAdd(It.IsAny<AlertMessageDto>())).Returns(false);

        var poller = new PikudPoller(_serviceMock.Object, _cacheMock.Object, _timeProviderMock.Object);
        var timerInfo = new TimerInfo();

        var result = await poller.Run(timerInfo, _contextMock.Object, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("ping", result.Target);
    }
}
