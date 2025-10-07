using RestAPI.Application.Commands;
using RestAPI.Application.Queries;
using RestAPI.Models;
using RestAPI.Repositories;
using RestAPI.Services;
using MediatR;
using AutoMapper;

namespace RestAPI.Application.Handlers
{
    // Add IRequestHandler interface to your existing handler
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IMapper _mapper; // Add AutoMapper

        public CreateUserCommandHandler(IUserRepository userRepository, JwtService jwtService, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        // Change return type and add Task 
        public Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            // Keep ALL your existing logic exactly the same
            if (string.IsNullOrEmpty(command.Username) || string.IsNullOrEmpty(command.Password))
            {
                return Task.FromResult(new CreateUserResult
                {
                    Success = false,
                    Message = "Username and Password are required"
                });
            }

            var existingUser = _userRepository.GetByUsername(command.Username);
            if (existingUser != null)
            {
                return Task.FromResult(new CreateUserResult
                {
                    Success = false,
                    Message = "User already exists!"
                });
            }

            var newUser = new User
            {
                Username = command.Username,
                Password = command.Password,
                CreatedAt = DateTime.UtcNow
            };

            _userRepository.Add(newUser);

            var token = _jwtService.CreateToken(newUser);

            // Use AutoMapper instead of manual mapping
            var userData = _mapper.Map<CreateUserResult.UserData>(newUser);

            return Task.FromResult(new CreateUserResult
            {
                Success = true,
                UserId = newUser.Id,
                Message = "User Created Successfully!",
                Token = token,
                User = userData
            });
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            // Keep ALL your existing logic
            var user = _userRepository.GetById(command.UserId);
            if (user == null)
            {
                return Task.FromResult(new UpdateUserResult
                {
                    Success = false,
                    Message = "User not found!"
                });
            }

            if (!string.IsNullOrEmpty(command.NewUsername) && command.NewUsername != user.Username)
            {
                var existingUser = _userRepository.GetByUsername(command.NewUsername);
                if (existingUser != null)
                {
                    return Task.FromResult(new UpdateUserResult
                    {
                        Success = false,
                        Message = "Username already taken!"
                    });
                }
            }

            if (!string.IsNullOrEmpty(command.NewUsername))
                user.Username = command.NewUsername;
            
            if (!string.IsNullOrEmpty(command.NewPassword))
                user.Password = command.NewPassword;

            _userRepository.Update(user);

            // Use AutoMapper
            var userData = _mapper.Map<UpdateUserResult.UserData>(user);

            return Task.FromResult(new UpdateUserResult
            {
                Success = true,
                Message = "User updated successfully!",
                UpdatedUser = userData
            });
        }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public Task<GetUserResult> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var user = _userRepository.GetById(query.UserId);
            
            if (user == null)
            {
                return Task.FromResult(new GetUserResult
                {
                    Success = false,
                    Message = "User not found!"
                });
            }

            // Use AutoMapper
            var userData = _mapper.Map<GetUserResult.UserData>(user);

            return Task.FromResult(new GetUserResult
            {
                Success = true,
                Message = "User found",
                User = userData
            });
        }
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, GetAllUsersResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public Task<GetAllUsersResult> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var users = _userRepository.GetAll();

            // Use AutoMapper for list mapping
            var userSummaries = _mapper.Map<List<GetAllUsersResult.UserSummary>>(users);

            return Task.FromResult(new GetAllUsersResult
            {
                Success = true,
                Users = userSummaries
            });
        }
    }

    public class AuthenticateUserQueryHandler : IRequestHandler<AuthenticateUserQuery, AuthenticateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IMapper _mapper;

        public AuthenticateUserQueryHandler(IUserRepository userRepository, JwtService jwtService, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public Task<AuthenticateUserResult> Handle(AuthenticateUserQuery query, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(query.Username) || string.IsNullOrEmpty(query.Password))
            {
                return Task.FromResult(new AuthenticateUserResult
                {
                    Success = false,
                    Message = "Username and password are required"
                });
            }

            var user = _userRepository.GetByUsername(query.Username);
            if (user == null || user.Password != query.Password)
            {
                return Task.FromResult(new AuthenticateUserResult
                {
                    Success = false,
                    Message = "Invalid credentials!"
                });
            }

            var token = _jwtService.CreateToken(user);

            // Use AutoMapper
            var userInfo = _mapper.Map<AuthenticateUserResult.UserInfo>(user);

            return Task.FromResult(new AuthenticateUserResult
            {
                Success = true,
                Message = "Authentication successful!",
                Token = token,
                User = userInfo
            });
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, DeleteUserResult>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<DeleteUserResult> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var user = _userRepository.GetById(command.UserId);

            if (user == null)
            {
                return Task.FromResult(new DeleteUserResult
                {
                    Success = false,
                    Message = "User not found!"
                });
            }

            _userRepository.Delete(command.UserId);

            return Task.FromResult(new DeleteUserResult
            {
                Success = true,
                Message = "User deleted successfully!"
            });
        }
    }
}