using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace RestaurantAPI.IntegrationTests;

public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RestaurantControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Theory]
    [InlineData("pageSize=5&pageNumber=1")]
    [InlineData("pageSize=10&pageNumber=2")]
    [InlineData("pageSize=15&pageNumber=3")]
    public async Task GetAll_WithQueryParams_ReturnsOkResult(string queryParams)
    {
        // Arrange
        
        // Act

        var response = await _client.GetAsync($"api/restaurant?{queryParams}");
        
        // Assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
    
    [Theory]
    [InlineData("pageSize=1&pageNumber=1")]
    [InlineData("pageSize=10&pageNumber=0")]
    [InlineData("pageSize=15&pageNumber=-3")]
    [InlineData("pageSize=20&pageNumber=3")]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetAll_WithInvalidQueryParams_ReturnsBadRequest(string queryParams)
    {
        // Arrange
        
        // Act

        var response = await _client.GetAsync($"api/restaurant?{queryParams}");
        
        // Assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}