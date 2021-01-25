using AutoMapper;
using JogoApi.Model.Users;
using JogoApi.Models;

namespace JogoApi.Configs
{

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
        }
    }

}



