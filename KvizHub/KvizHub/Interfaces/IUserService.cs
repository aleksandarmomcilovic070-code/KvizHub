using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KvizHub.Dto;

namespace KvizHub.Interfaces
{
    public interface IUserService
    {
        string Login(string email, string password);

        string Register(UserDto userDto);
    }
}
