using AutoMapper;
using KvizHub.Dto;
using KvizHub.Infrastructure;
using KvizHub.Interfaces;
using KvizHub.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace KvizHub.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly KvizDbContext _dbContex;
        private readonly IConfigurationSection _secretKey;
        public UserService(IConfiguration config, IMapper mapper, KvizDbContext dbContex)
        {
            _mapper = mapper;
            _dbContex = dbContex;
            _secretKey = config.GetSection("SecretKey");
        }

        public List<User> GetAllUsers()
        {
            return _dbContex.Users.ToList();
        }

        public string Login(LoginDto loginDto)
        {

            User user = _dbContex.Users.FirstOrDefault(u => u.Username == loginDto.UsernameOrEmail || u.Email == loginDto.UsernameOrEmail);
            if (user == null) {
                return null;
            }
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash)) 
            {
                return null;        
            }
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Id", user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Role,user.Role.ToString()));

            SymmetricSecurityKey secretKey =new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey.Value));
            SigningCredentials signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: signingCredentials

                );
            string tokenString= new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            Console.WriteLine(tokenString);
            return tokenString; 
           


        }

        public User Register(RegisterDto registerDto)
        {
            if (_dbContex.Users.Any(u => u.Username == registerDto.Username || u.Email==registerDto.Email))
            {
                throw new System.Exception("Korisnicko ime ili mejl vec postoje");
            } 
            string hashedPassword=BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var user=_mapper.Map<User>(registerDto);
            user.PasswordHash = hashedPassword;

            _dbContex.Users.Add(user);
            _dbContex.SaveChanges();

            return user;

        }
    }
}
