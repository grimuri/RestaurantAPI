using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;

namespace RestaurantAPI.IntegrationTests;

public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AccountControllerTests(WebApplicationFactory<Program> factory)
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
                   services.AddDbContext<RestaurantDbContext>(options =>
                       options.UseInMemoryDatabase("RestaurantDb"));
                });
            })
            .CreateClient();
    }

    [Fact]
    public async Task RegisterUser_WithValidModel_ReturnsOk()
    {
        // Arrange
        
        var registerUserDto = new RegisterUserDto()
        {
            Email = "test@test.com",
            Password = "password",
            ConfirmPassword = "password"
        };

        var httpContent = registerUserDto.ToJsonHttpContent();
        
        // Act

        var response = await _client.PostAsync("/api/account/register", httpContent);
        
        // Assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterUser_WithInvalidModel_ReturnsBadRequest()
    {
        // Arrange
        
        var registerUserDto = new RegisterUserDto()
        {
            Password = "123",
            ConfirmPassword = "321"
        };

        var httpContent = registerUserDto.ToJsonHttpContent();

        // Act

        var response = await _client.PostAsync("/api/account/register", httpContent);
        
        // Assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
}