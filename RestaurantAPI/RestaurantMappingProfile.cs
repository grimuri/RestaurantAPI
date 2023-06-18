using AutoMapper;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(r => r.Street, c => c.MapFrom(s => s.Address.Street))
                .ForMember(r => r.City, c => c.MapFrom(s => s.Address.City))
                .ForMember(r => r.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));

            CreateMap<Dish, DishDto>();

            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(r => r.Address, c => c.MapFrom(dto => new Address()
                {
                    Street = dto.Street,
                    City = dto.City,
                    PostalCode = dto.PostalCode,
                }));

            CreateMap<CreateDishDto, Dish>();
        }
    }
}
