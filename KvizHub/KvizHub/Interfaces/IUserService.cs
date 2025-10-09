using KvizHub.Dto;
using KvizHub.Models;
using System.Collections.Generic;

namespace KvizHub.Interfaces
{
    public interface IUserService
    {
        public string Login(LoginDto loginDto);
        public User Register(RegisterDto registerDto);
        public List<User> GetAllUsers();
    }
}
