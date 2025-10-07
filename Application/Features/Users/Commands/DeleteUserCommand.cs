using MediatR;

namespace RestAPI.Application.Commands
{
    public class DeleteUserCommand : IRequest<DeleteUserResult>
    {
        public int UserId { get; set; }
    }
    
    public class DeleteUserResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}