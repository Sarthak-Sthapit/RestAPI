using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RestAPI.Services;
using RestAPI.DTOs;
using RestAPI.Application.Commands;
using RestAPI.Application.Queries;
using RestAPI.Application.Handlers;
using MediatR; // Added this

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase
    {
        // Replace all your handler dependencies with just IMediator
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Keep your exact endpoint logic, just change handler calls
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupDto dto)
        {
            var command = new CreateUserCommand
            {
                Username = dto.Username,
                Password = dto.Password
            };

            // Use mediator instead of direct handler call
            var result = _mediator.Send(command).Result; // Keep it sync like your original

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new
            {
                message = result.Message,
                token = result.Token,
                user = result.User
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var query = new AuthenticateUserQuery
            {
                Username = dto.Username,
                Password = dto.Password
            };

            var result = _mediator.Send(query).Result; // Keep sync
            if (!result.Success)
            {
                return Unauthorized(result.Message);
            }

            return Ok(new
            {
                message = result.Message,
                token = result.Token,
                user = result.User
            });
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult GetAllUsers()
        {
            var query = new GetAllUsersQuery();
            var result = _mediator.Send(query).Result; // Keep sync
            return Ok(result.Users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUserById(int id)
        {
            var query = new GetUserByIdQuery { UserId = id };
            var result = _mediator.Send(query).Result; // Keep sync

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.User);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            var command = new DeleteUserCommand { UserId = id };
            var result = _mediator.Send(command).Result; // Keep sync

            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(new { message = result.Message });
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var command = new UpdateUserCommand
            {
                UserId = id,
                NewUsername = dto.NewUsername,
                NewPassword = dto.NewPassword
            };

            var result = _mediator.Send(command).Result; // Keep sync
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { 
                message = result.Message, 
                user = result.UpdatedUser  
            });
        }
    }
}