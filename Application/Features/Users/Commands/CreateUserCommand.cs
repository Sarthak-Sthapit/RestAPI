using MediatR;
namespace RestAPI.Application.Commands
{

    //command - data controller
    public class CreateUserCommand : IRequest<CreateUserResult>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }

    //result - what the command returns
    public class CreateUserResult
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public UserData User { get; set; } = new();

        public class UserData
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public DateTime MemberSince { get; set; }

        }



    }


}