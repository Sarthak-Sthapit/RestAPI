using RestAPI.Application.Commands;
using RestAPI.Application.Queries;
using RestAPI.Models;
using RestAPI.Repositories;
using RestAPI.Services;

namespace RestAPI.Application.Handlers
{
    public class CreateUserCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public CreateUserCommandHandler(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        //handle method

        public CreateUserResult Handle(CreateUserCommand command)
        {
            //validation
            if (string.IsNullOrEmpty(command.Username) || string.IsNullOrEmpty(command.Password))
            {
                return new CreateUserResult
                {
                    Success = false,
                    Message = "Username and Password are required"
                };
            }

            var existingUser = _userRepository.GetByUsername(command.Username);
            if (existingUser != null)
            {
                return new CreateUserResult
                {
                    Success = false,
                    Message = "User already exists!"
                };
            }

            var newUser = new User
            {
                Username = command.Username,
                Password = command.Password,
                CreatedAt = DateTime.UtcNow
            };

            _userRepository.Add(newUser);

            var token = _jwtService.CreateToken(newUser);

            return new CreateUserResult
            {
                Success = true,
                UserId = newUser.Id,
                Message = "User Created Successfully!",
                Token = token,
                User = new CreateUserResult.UserData
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    MemberSince = newUser.CreatedAt
                }
            };
        }
    }


    public class UpdateUserCommandHandler
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UpdateUserResult Handle(UpdateUserCommand command)
        {
            //Finding user
            var user = _userRepository.GetById(command.UserId);
            if (user == null)
            {
                return new UpdateUserResult
                {
                    Success = false,
                    Message = "User not found!"
                };
            }

            //Checking username availability
            if (!string.IsNullOrEmpty(command.NewUsername) && command.NewUsername != user.Username)
            {
                var existingUser = _userRepository.GetByUsername(command.NewUsername);
                if (existingUser != null)
                {
                    return new UpdateUserResult
                    {
                        Success = false,
                        Message = "Username already taken!"
                    };
                }
            }

            //Applying updates
            if (!string.IsNullOrEmpty(command.NewUsername))
                user.Username = command.NewUsername;
            
            if (!string.IsNullOrEmpty(command.NewPassword))
                user.Password = command.NewPassword;

            _userRepository.Update(user);

            //Return result
            return new UpdateUserResult
            {
                Success = true,
                Message = "User updated successfully!",
                UpdatedUser = new UpdateUserResult.UserData
                {
                    Id = user.Id,
                    Username = user.Username,
                    MemberSince = user.CreatedAt
                }
            };
        }
    }

    //Query Handlers

    public class GetUserByIdQueryHandler
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public GetUserResult Handle(GetUserByIdQuery query)
        {
            var user = _userRepository.GetById(query.UserId);
            
            if (user == null)
            {
                return new GetUserResult
                {
                    Success = false,
                    Message = "User not found!"
                };
            }

            return new GetUserResult
            {
                Success = true,
                Message = "User found",
                User = new GetUserResult.UserData
                {
                    Id = user.Id,
                    Username = user.Username,
                    MemberSince = user.CreatedAt
                }
            };
        }
    }

    public class GetAllUsersQueryHandler
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public GetAllUsersResult Handle(GetAllUsersQuery query)
        {
            var users = _userRepository.GetAll();

            var userSummaries = users.Select(user => new GetAllUsersResult.UserSummary
            {
                Id = user.Id,
                Username = user.Username,
                MemberSince = user.CreatedAt,
            }).ToList();

            return new GetAllUsersResult
            {
                Success = true,
                Users = userSummaries
            };
        }

    }

    public class AuthenticateUserQueryHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;

        public AuthenticateUserQueryHandler(IUserRepository userRepository, JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public AuthenticateUserResult Handle(AuthenticateUserQuery query)
        {
            // Validation
            if (string.IsNullOrEmpty(query.Username) || string.IsNullOrEmpty(query.Password))
            {
                return new AuthenticateUserResult
                {
                    Success = false,
                    Message = "Username and password are required"
                };
            }

            // Find and validate user
            var user = _userRepository.GetByUsername(query.Username);
            if (user == null || user.Password != query.Password)
            {
                return new AuthenticateUserResult
                {
                    Success = false,
                    Message = "Invalid credentials!"
                };
            }

            // Generate token
            var token = _jwtService.CreateToken(user);

            return new AuthenticateUserResult
            {
                Success = true,
                Message = "Authentication successful!",
                Token = token,
                User = new AuthenticateUserResult.UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    MemberSince = user.CreatedAt
                }
            };
        }
    }

        public class DeleteUserCommandHandler
        {
            private readonly IUserRepository _userRepository;

            public DeleteUserCommandHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            // Executes the delete logic
            public DeleteUserResult Handle(DeleteUserCommand command)
            {
                var user = _userRepository.GetById(command.UserId);

                if (user == null)
                {
                    return new DeleteUserResult
                    {
                        Success = false,
                        Message = "User not found!"
                    };
                }

                _userRepository.Delete(command.UserId);

                return new DeleteUserResult
                {
                    Success = true,
                    Message = "User deleted successfully!"
                };
            }
        }
    }