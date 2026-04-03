using AlertHub.Api.Models;
using AlertHub.Api.Services;

namespace AlertHub.Tests.Services;

public class AlertServiceTests
{
    private readonly AlertService _sut;

    public AlertServiceTests()
    {
        _sut = new AlertService();
    }

    [Fact]
    public async Task ProcessAlertAsync_ShouldReturnTrue()
    {
        // Arrange
        var alert = new AlertMessage();

        // Act
        var result = await _sut.ProcessAlertAsync(alert);

        // Assert
        Assert.True(result);
    }
}
