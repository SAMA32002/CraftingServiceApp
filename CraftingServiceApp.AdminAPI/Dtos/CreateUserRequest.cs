﻿namespace CraftingServiceApp.AdminAPI.Dtos
{
    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
    }
}