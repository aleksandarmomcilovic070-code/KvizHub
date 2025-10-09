using AutoMapper;
using KvizHub.Dto;
using KvizHub.Models;
namespace KvizHub.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User,RegisterDto>().ReverseMap();
            CreateMap<User, LoginDto>().ReverseMap();
        } 
    }
}
