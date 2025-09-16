using Microsoft.EntityFrameworkCore;
using RestAPI.Models;

namespace RestAPI.Data
{
    /*
    DbContext - a base class from Microsoft.EntityFrameworkCore
    - Represents a session with the database
    - Handles connection management and change tracking
    */
    public class AppDbContext : DbContext
    {
        /*
        - Construction receives configuration settings
        - DbContextOptions : configuration settings (Connection string , provider , etc)
        - <AppDbContext> : a generic parameter specifying this context type
        */
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        //represents a table called 'Users' in the database 
        //provides CRUD operations
        public DbSet<User> Users { get; set; }

    }
}

