using Microsoft.EntityFrameworkCore;
using CleanRestApi.Models;

namespace CleanRestApi.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

    }
}

