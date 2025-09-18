namespace CleanRestApi.Application.Commands
{
    public class CreateUserCommand
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

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