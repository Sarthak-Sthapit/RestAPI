namespace CleanRestApi.Domain.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(int userId) 
            : base($"User with ID {userId} was not found") { }
        
        public UserNotFoundException(string username) 
            : base($"User with username '{username}' was not found") { }
    }

    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException(string username) 
            : base($"User with username '{username}' already exists") { }
    }
}