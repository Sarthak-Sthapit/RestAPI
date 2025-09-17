namespace RestAPI.DTOs
{
    public class UserResponseDto //for sending user data to client (get methods)
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime MemberSince { get; set; }
        //no password , sensitive data isn't exposed
    }

    //for sign up (post)

    public class SignupDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    //for user login (post)

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    //for user updating (put)
    public class UpdateUserDto
    {
        public string? NewUsername { get; set; } = string.Empty;
        public string? NewPassword { get; set; } = string.Empty;
    }
        // For list views - minimal info
    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime MemberSince { get; set; }
    }

    // For successful authentication responses
    public class AuthResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public UserResponseDto User { get; set; } = new();
    }
}