namespace CleanRestApi.Models
{
    public class User
    {
        public int Id { get; set; } // primary key and is automatically detected by EF
        public string Username { get; set; } = string.Empty; //unique username
        public string Password { get; set; } = string.Empty; //plain text password for learning 
       public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // When user signed up

    }
}

