using AcountService.dto.request.accountservice;
using AcountService.dto.response.account;
using AcountService.entity;
using AutoMapper;

namespace AcountService.mapper
{
    public class UserMapper: Profile
    {
         public UserMapper() {

            CreateMap<UserCreateRequest, User>();
            CreateMap<UserUpdateRequest, User>();
            CreateMap<User, UserResponse>();

        }
    }
}
