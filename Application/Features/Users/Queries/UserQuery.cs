using MediatR;

namespace RestAPI.Application.Queries
{
    public class GetUserByIdQuery : IRequest<GetUserResult>
    {
        public int UserId { get; set; }
    }

    public class GetAllUsersQuery : IRequest<GetAllUsersResult>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class AuthenticateUserQuery : IRequest<AuthenticateUserResult>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

   
    public class GetUserResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserData? User { get; set; }
        
        public class UserData
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public DateTime MemberSince { get; set; }
        }
    }

    public class GetAllUsersResult
    {
        public bool Success { get; set; }
        public List<UserSummary> Users { get; set; } = new();
        
        public class UserSummary
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public DateTime MemberSince { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }

    public class AuthenticateUserResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public UserInfo? User { get; set; }
        
        public class UserInfo
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public DateTime MemberSince { get; set; }
        }
    }
}
