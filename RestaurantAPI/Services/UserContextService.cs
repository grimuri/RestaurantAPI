using RestaurantAPI.Services.Interfaces;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => _contextAccessor.HttpContext.User;
        public int? GetUserId => 
            (int?)int.Parse(User is null ? null : User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }
}
