namespace RestAPI.Application.Commands
{
      public class UpdateUserCommand
    {
        public int UserId { get; set; }
        public string? NewUsername { get; set; }
        public string? NewPassword { get; set; }
    }

    public class UpdateUserResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserData? UpdatedUser { get; set; }
        
        public class UserData
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public DateTime MemberSince { get; set; }
        }
    }
}