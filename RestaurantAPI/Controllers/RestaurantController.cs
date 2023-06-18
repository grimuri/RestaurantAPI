using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult UpdateRestaurant([FromRoute] int id,
            [FromBody] UpdateRestaurantDto dto)
        {
            var isUpdated = _restaurantService.Update(id, dto);

            return Ok("Successfully updated");
        }
        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult DeleteRestaurant([FromRoute] int id)
        {
            var isDeleted = _restaurantService.Delete(id);

            return Ok("Successfully deleted");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<PagedResult<RestaurantDto>> GetAll([FromQuery] RestaurantQuery query)
        {
            var restaurants = _restaurantService.GetAll(query);
            return Ok(restaurants);
        }

        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var id = _restaurantService.Create(dto);

            return Created($"api/restaurant/{id}", null);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaurantService.GetById(id);

            return StatusCode(200, restaurant);
        }
    }
}
