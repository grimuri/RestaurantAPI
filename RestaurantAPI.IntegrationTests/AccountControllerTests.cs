using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Moq;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.IntegrationTests;

public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private Mock<IAccountService> _mockAcountService;
    public AccountControllerTests(WebApplicationFactory<Program> factory)
    {
        _mockAcountService = new Mock<IAccountService>();
        _client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                   var dbContextOptions = services
                       .SingleOrDefault(service => 
                           service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                   services.Remove(dbContextOptions);
                   services.AddSingleton<IAccountService>(_mockAcountService.Object);
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

    [Fact]
    public async Task Login_ForRegisteredUser_ReturnsOk()
    {
        // Arrange
        _mockAcountService
            .Setup(x => x.GenerateJWT(It.IsAny<LoginUserDto>()))
            .Returns("jwt");
        
        var loginDto = new LoginUserDto()
        {
            Email = "email@email.com",
            Password = "password123"
        };

        var httpContent = loginDto.ToJsonHttpContent();
        
        // Act
        var response = await _client.PostAsync("/api/account/login", httpContent);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}