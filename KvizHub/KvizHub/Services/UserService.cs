﻿using KvizHub.Interfaces;
using KvizHub.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using KvizHub.Dto;
using KvizHub.Infrastructure;


namespace KvizHub.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly DbContextt _dbContext;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secretKey;

        public UserService(IConfiguration configuration, DbContextt dbContextt)
        {
            _configuration = configuration;
            _dbContext = dbContextt;

            var jwtSection = _configuration.GetSection("Jwt");
            _issuer = jwtSection["Issuer"];
            _audience = jwtSection["Audience"];
            _secretKey = jwtSection["SecretKey"];

            if (string.IsNullOrWhiteSpace(_issuer))
                throw new InvalidOperationException("JWT configuration error: 'Jwt:Issuer' is missing or empty.");
            if (string.IsNullOrWhiteSpace(_audience))
                throw new InvalidOperationException("JWT configuration error: 'Jwt:Audience' is missing or empty.");
            if (string.IsNullOrWhiteSpace(_secretKey))
                throw new InvalidOperationException("JWT configuration error: 'Jwt:SecretKey' is missing or empty.");
        }


        public string Login(string identifier, string password)
        {
            // Try to match either Email OR Username
            var user = _dbContext.Users
                .FirstOrDefault(u => u.Email == identifier || u.Name == identifier);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name),   // always include Name in token
        new Claim(ClaimTypes.Role, user.Role)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public string Register(UserDto userDto)
        {
            // Check if user already exists
            var existingUser = _dbContext.GetUserById(userDto.Name);
            if (existingUser != null)
            {
                throw new Exception("User already exists");
            }

            // Create new user
            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Image = userDto.Image,
                Role = "korisnik" // default role
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // JWT claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(20),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
