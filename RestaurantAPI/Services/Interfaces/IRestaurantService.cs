using RestaurantAPI.Models;
using System.Security.Claims;

namespace RestaurantAPI.Services.Interfaces
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto dto);
        PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
        RestaurantDto GetById(int id);
        bool Delete(int id);
        bool Update(int id, UpdateRestaurantDto dto);
    }
}