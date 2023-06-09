﻿using RestaurantAPI.Models;

namespace RestaurantAPI.Services.Interfaces
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        string GenerateJWT(LoginUserDto dto);
    }
}
