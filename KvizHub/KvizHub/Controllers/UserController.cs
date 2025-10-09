using KvizHub.Dto;
using KvizHub.Interfaces;
using KvizHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace KvizHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        [Authorize (Roles ="Admin")]
        public IActionResult GetAllUsers() 
        {
            return Ok(_userService.GetAllUsers());
        } 
        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody]LoginDto loginDto) 
        {
            return Ok(_userService.Login(loginDto));
        }

        [HttpPost("Register")]
        public IActionResult RegisterUser([FromBody] RegisterDto registerDto) 
        {
            User user;
            try
            {
                user = _userService.Register(registerDto);
            }
            catch (InvalidCredentialException e)
            {
                return Conflict(e.Message);
            }
            return Ok(user);
        }
    }
}
