using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RestAPI.Services;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]

    /*
    ControllerBase class from namespace Microsoft.AspNetCore
    - ASP.NET core treats 'AuthController' as an API Controller instead of a normal c# class
    - Provides Helper methods - Ok() , BadRequest() , Unauthorized() 
    - Attribute based routing - ([HttpGet], [HttpPost], [HttpPut], [HttpDelete]).
    */
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly JwtService _jwtService;
        public AuthController(IUserRepository repo, JwtService jwtService)
        {
            _repo = repo;
            _jwtService = jwtService;
        }

        /*
        IActionResult is also from namespace Microsoft.AspNetCore
        - is a standard return type for API actions
        - allows different methods (login/signup) to return different HTTP responses whilst being under same 
        return type . for eg. Ok() , BadRequest() have different HTTP codes but same return type.
        */
        // Anyone can signup (no token needed)
        [HttpPost("signup")]
        public IActionResult Signup(string username, string password)
        {
            var existing = _repo.GetByUsername(username);
            if (existing != null)
            {
                return BadRequest("User already exists!");
            }

            var newUser = new User { Username = username, Password = password };
            _repo.Add(newUser);

            // Give them a token immediately
            var token = _jwtService.CreateToken(newUser);

            return Ok(new
            {
                message = "Signed up!",
                token = token
            });
        }
        // no token needed
        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var user = _repo.GetByUsername(username);
            if (user == null || user.Password != password)
            {
                return Unauthorized("Invalid Credentials!");
            }
            // Give them a token
            var token = _jwtService.CreateToken(user);

            return Ok(new
            {
                message = "Logged in!",
                token = token
            });
        }
        
        //token needed
        [HttpGet]
        [Authorize] //this makes it require a token
        public IActionResult GetAllUsers()
        {
            var users = _repo.GetAll();

            var safeUsers = new List<object>(); //create safe user object (without password)
            foreach (var user in users)
            {
                var safeUser = new
                {
                    Id = user.Id,
                    Username = user.Username,
                    CreatedAt = user.CreatedAt,
                };
                safeUsers.Add(safeUser);
            }
            return Ok(safeUsers);
        }
        //token needed
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUserById(int id)
        {
            var user = _repo.GetById(id);

            if (user == null)
            {
                return NotFound("User not found!");
            }


            var safeUser = new
            {
                Id = user.Id,
                Username = user.Username,
                CreatedAt = user.CreatedAt
            };

            return Ok(safeUser);
        }

        //Delete by ID , token needed
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            var user = _repo.GetById(id);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            _repo.Delete(id);
            return Ok("User deleted successfully!");
        }

        // UPDATE - Update user information , token needed
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, string newUsername, string newPassword)
        {
            var user = _repo.GetById(id);

            if (user == null)
            {
                return NotFound("User not found!");
            }

            // Check if new username is already taken (if username is being changed)
            if (!string.IsNullOrEmpty(newUsername) && newUsername != user.Username)
            {
                var existingUser = _repo.GetByUsername(newUsername);
                if (existingUser != null)
                {
                    return BadRequest("Username already taken!");
                }
                user.Username = newUsername;
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(newPassword))
            {
                user.Password = newPassword;
            }

            _repo.Update(user);
            return Ok("User updated successfully!");
        }

    }
}
