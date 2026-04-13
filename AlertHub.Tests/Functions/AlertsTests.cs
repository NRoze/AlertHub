using AlertHub.Api.Functions;
using AlertHub.Api.Models;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Immutable;

namespace AlertHub.Tests.Functions;

public class AlertsTests
{
    [Fact]
    public async Task Alerts_ReturnsCacheContents()
    {
        // Arrange
        var cacheMock = new Mock<IAlertCache>();
        var sampleDto = new AlertMessageDto { Id = "test" };
        var mockData = ImmutableArray.Create(sampleDto);
        
        cacheMock.Setup(c => c.GetAll()).Returns(mockData);
        var func = new AlertsFunction(cacheMock.Object);
        
        var contextMock = new Mock<FunctionContext>();
        
        var requestMock = new Mock<HttpRequestData>(contextMock.Object);

        // Act
        var result = await func.Alerts(requestMock.Object);

        // Assert
        Assert.Single(result);
        Assert.Equal("test", result[0].Id);
    }
}
