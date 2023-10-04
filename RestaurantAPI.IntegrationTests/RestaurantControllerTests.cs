using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;

namespace RestaurantAPI.IntegrationTests;

public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public RestaurantControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services
                        .SingleOrDefault(service =>
                            service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                    services.Remove(dbContextOptions);
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));
                    services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                });
            })
            .CreateClient();
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

    [Fact]
    public async Task CreateRestaurant_WithValidModel_ReturnsCreatedStatus()
    {
        // Arrange

        var createRestaurantDto = new CreateRestaurantDto()
        {
            Name = "TestRestaurant",
            Street = "Miła 5",
            Description = "Dobra restauracja",
            City = "Rzeszów"
        };

        var httpContent = createRestaurantDto.ToJsonHttpContent();
        
        // Act

        var response = await _client.PostAsync("api/restaurant", httpContent);
        
        // Assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        response.Headers.Should().NotBeNull();

    }

    [Fact]
    public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
    {
        // Arrange

        var createRestaurantDto = new CreateRestaurantDto()
        {
            ContactEmail = "email@email.com",
            HasDelivery = true
        };

        var httpContent = createRestaurantDto.ToJsonHttpContent();
        
        // Act
        
        var response = await _client.PostAsync("api/restaurant", httpContent);
        
        // Assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
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