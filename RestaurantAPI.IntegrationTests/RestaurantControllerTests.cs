using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;

namespace RestaurantAPI.IntegrationTests;

public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public RestaurantControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services
                        .SingleOrDefault(service =>
                            service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                    services.Remove(dbContextOptions);
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.AddMvc(option => 
                        option.Filters.Add(new FakeUserFilter()));
                    services.AddDbContext<RestaurantDbContext>(options => 
                        options.UseInMemoryDatabase("RestaurantDb"));
                });
            });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Delete_ForNonExistingRestaurant_ReturnsNotFound()
    {
        // Arrange

        // Act

        var response = await _client.DeleteAsync($"api/restaurant/999");

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ForNonRestaurantOwner_ReturnsForbidden()
    {
        // Arrange

        var restaurant = new Restaurant()
        {
            CreatedById = 2,
            Name = "Test"
        };
        
        SeedRestaurant(restaurant);

        // Act

        var response = await _client.DeleteAsync($"api/restaurant/{restaurant.Id}");

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task Delete_ForRestaurantOwner_ReturnsOkStatus()
    {
        // Arrange

        var restaurant = new Restaurant()
        {
            CreatedById = 1,
            Name = "Test"
        };
        
        SeedRestaurant(restaurant);

        // Act

        var response = await _client.DeleteAsync($"api/restaurant/{restaurant.Id}");

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.OK);
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

        response.StatusCode.Should().Be(HttpStatusCode.OK);
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

        response.StatusCode.Should().Be(HttpStatusCode.Created);
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

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private void SeedRestaurant(Restaurant restaurant)
    {
        var serviceScopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

        dbContext.Restaurants.Add(restaurant);
        dbContext.SaveChanges();
    }
}