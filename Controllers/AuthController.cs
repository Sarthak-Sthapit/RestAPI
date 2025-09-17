using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RestAPI.Services;
using RestAPI.DTOs;
using RestAPI.Mappers;
using RestAPI.Application.Commands;
using RestAPI.Application.Queries;
using RestAPI.Application.Handlers;

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
        // CQRS Handlers injected instead of repository/service directly
        private readonly CreateUserCommandHandler _createUserHandler;
        private readonly UpdateUserCommandHandler _updateUserHandler;
        private readonly GetUserByIdQueryHandler _getUserHandler;
        private readonly GetAllUsersQueryHandler _getAllUsersHandler;
        private readonly AuthenticateUserQueryHandler _authenticateHandler;
        private readonly DeleteUserCommandHandler _deleteUserHandler; 

        public AuthController(
            CreateUserCommandHandler createUserHandler,
            UpdateUserCommandHandler updateUserHandler,
            GetUserByIdQueryHandler getUserHandler,
            GetAllUsersQueryHandler getAllUsersHandler,
            AuthenticateUserQueryHandler authenticateHandler,
            DeleteUserCommandHandler deleteUserHandler)
        {
            _createUserHandler = createUserHandler;
            _updateUserHandler = updateUserHandler;
            _getUserHandler = getUserHandler;
            _getAllUsersHandler = getAllUsersHandler;
            _authenticateHandler = authenticateHandler;
            _deleteUserHandler = deleteUserHandler;
        }

        /*
        IActionResult is also from namespace Microsoft.AspNetCore
        - is a standard return type for API actions
        - allows different methods (login/signup) to return different HTTP responses whilst being under same 
        return type . for eg. Ok() , BadRequest() have different HTTP codes but same return type.
        */
        // Anyone can signup (no token needed)
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupDto dto)
        {
            // 1. MAP: DTO → Command
            var command = new CreateUserCommand
            {
                Username = dto.Username,
                Password = dto.Password
            };

            // 2. CALL HANDLER
            var result = _createUserHandler.Handle(command);

            // 3. RESPONSE
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

        // no token needed
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            // 1. MAP: DTO → Query
            var query = new AuthenticateUserQuery
            {
                Username = dto.Username,
                Password = dto.Password
            };


            var result = _authenticateHandler.Handle(query);
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
        
        
        //token needed
        [HttpGet]
        [Authorize] //this makes it require a token
        public IActionResult GetAllUsers()
        {
            var query = new GetAllUsersQuery();
            var result = _getAllUsersHandler.Handle(query);
            return Ok(result.Users);  // DTO OUTPUT
        }

        //token needed
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUserById(int id)
        {
            // 1. CREATE QUERY
            var query = new GetUserByIdQuery { UserId = id };

            // 2. CALL HANDLER
            var result = _getUserHandler.Handle(query);

            // 3. RESPONSE
            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(result.User);
        }

        //Delete by ID , token needed
        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteUser(int id)
        {
            // 1. CREATE COMMAND
            var command = new DeleteUserCommand { UserId = id };

            // 2. CALL HANDLER
            var result = _deleteUserHandler.Handle(command);

            // 3. RESPONSE
            if (!result.Success)
            {
                return NotFound(result.Message);
            }

            return Ok(new { message = result.Message });
        }

        // UPDATE - Update user information , token needed
        [HttpPut("{id}")]
        [Authorize]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            // 1. MAP: DTO → Command
            var command = new UpdateUserCommand
            {
                UserId = id,
                NewUsername = dto.NewUsername,
                NewPassword = dto.NewPassword
            };

            var result = _updateUserHandler.Handle(command);
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
