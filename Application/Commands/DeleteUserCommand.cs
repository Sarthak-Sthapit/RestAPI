
namespace CleanRestApi.Application.Commands
{
    public class DeleteUserCommand
    {
        public int UserId { get; set; }
    }
    
    public class DeleteUserResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}