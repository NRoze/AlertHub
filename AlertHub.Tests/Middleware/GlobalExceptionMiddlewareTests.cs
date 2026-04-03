using AlertHub.Api.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Moq;

namespace AlertHub.Tests.Middleware;

public class GlobalExceptionMiddlewareTests
{
    private readonly GlobalExceptionMiddleware _sut;
    private readonly Mock<FunctionContext> _contextMock;

    public GlobalExceptionMiddlewareTests()
    {
        _sut = new GlobalExceptionMiddleware();
        _contextMock = new Mock<FunctionContext>();
    }

    [Fact]
    public async Task Invoke_ShouldCallNext_WhenNoException()
    {
        // Arrange
        bool nextCalled = false;
        FunctionExecutionDelegate next = (context) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        // Act
        await _sut.Invoke(_contextMock.Object, next);

        // Assert
        Assert.True(nextCalled);
    }
}
