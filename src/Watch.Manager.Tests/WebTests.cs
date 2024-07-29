using System.Net;

namespace Watch.Manager.Tests;

public class WebTests
{
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Arrange
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Watch_Manager_AppHost>().ConfigureAwait(false);
        await using var app = await appHost.BuildAsync();
        await app.StartAsync().ConfigureAwait(false);

        // Act
        var httpClient = app.CreateHttpClient("webfrontend");
        var response = await httpClient.GetAsync("/").ConfigureAwait(false);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
