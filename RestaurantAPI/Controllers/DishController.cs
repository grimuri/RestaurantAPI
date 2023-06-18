using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        public DishController(IDishService dishService)
        {
            _dishService = dishService;
        }

        [HttpDelete]
        public ActionResult DeleteAllDishes([FromRoute] int restaurantId)
        {
            _dishService.RemoveAll(restaurantId);
            return NoContent();
        }

        [HttpDelete("{dishId}")]
        public ActionResult DeleteDish([FromRoute] int restaurantId,
            [FromRoute] int dishId)
        {
            _dishService.Remove(restaurantId, dishId);
            return NoContent();
        }
        
        [HttpPost]
        public ActionResult CreateDish([FromRoute] int restaurantId,
            [FromBody] CreateDishDto dto)
        {
            var dishId = _dishService.Create(restaurantId, dto);
            return Created($"api/restaurant/{restaurantId}/dish/{dishId}", null);
        }

        [HttpGet("{dishId}")]
        public ActionResult<DishDto> GetDishById([FromRoute] int restaurantId, 
            [FromRoute] int dishId)
        {
            var result = _dishService.GetById(restaurantId, dishId);
            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<DishDto>> GetAllDish([FromRoute] int restaurantId)
        {
            var result = _dishService.GetAll(restaurantId);
            return Ok(result);
        }
    }
}
