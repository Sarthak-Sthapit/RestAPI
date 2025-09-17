namespace RestAPI.Application.Commands
{

    public class DeleteUserCommand
    {
        public int UserId { get; set; }  // The ID of the user we want to delete
    }
    
    public class DeleteUserResult
    {
        public bool Success { get; set; }     // True if delete succeeded
        public string Message { get; set; }   // Informational message
    }
}
