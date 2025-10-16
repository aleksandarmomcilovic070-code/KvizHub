using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KvizHub.Dto;
using KvizHub.Interfaces;

namespace KvizHub.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var token = _userService.Login(dto.Email, dto.Password);

            if (token == null)
            {
                return Unauthorized(new { error = "Invalid username/email or password" });
            }

            return Ok(new { token });
        }



        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDto dto)
        {
            try
            {
                // Register the user and return the JWT token in JSON
                var token = _userService.Register(dto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                // Return a proper JSON error response
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpGet("check")]
        [Authorize(Roles = "admin")]
        public IActionResult Check()
        {
            return Ok("Admin si"); 
        }
    }
}
