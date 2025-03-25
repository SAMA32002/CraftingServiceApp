using AutoMapper;
using CraftingServiceApp.AdminAPI.Dtos;
using CraftingServiceApp.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CraftingServiceApp.AdminAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())).ReverseMap();

           
        }
    }
}
