using RestaurantAPI;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;
using RestaurantAPI.Services.Interfaces;
using System.Reflection;
using NLog;
using NLog.Web;
using RestaurantAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Authorization;
using Microsoft.EntityFrameworkCore;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var authenticationSettings = new AuthenticationSettings();

builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);

builder.Services.AddSingleton(authenticationSettings);

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
    };
});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "Poland"));
    option.AddPolicy("AtLeast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
    option.AddPolicy("CreatedAtLeast2Restaurants", 
        builder => builder.AddRequirements(new CreatedMultipleRestaurantsRequirement(2)));
});

builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, CreatedMultipleRestaurantsRequirementHandler>();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddScoped<RestaurantSeeder>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendClient", policybuilder =>
        policybuilder.AllowAnyHeader()
               .AllowAnyMethod()
               .WithOrigins(builder.Configuration["AllowedOrigins"]));
});

builder.Services.AddDbContext<RestaurantDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantDbConnection")));

// NLog: Setup NLog for Dependency injection
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();
seeder.Seed();
app.UseResponseCaching();
app.UseStaticFiles();
app.UseCors("FrontendClient");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI(r =>
{
    r.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
});
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }