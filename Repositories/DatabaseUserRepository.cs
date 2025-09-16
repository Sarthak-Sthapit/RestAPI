using RestAPI.Data;
using RestAPI.Models;

namespace RestAPI.Repositories
{
    public class DatabaseUserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public DatabaseUserRepository(AppDbContext context) //DI
        {
            _context = context;
        }

        public User? GetByUsername(string Username)
        {
            foreach (var user in _context.Users)
            {
                if (user.Username == Username)
                {
                    return user; //return matching user
                }
            }
            return null; //null if no match found
        }


        //add new user to the database
        public void Add(User user)
        {
            _context.Users.Add(user); //staging
            _context.SaveChanges(); //committing changes
        }

        //READ - Get All

        public List<User> GetAll()
        {
            var userList = new List<User>();
            foreach (var user in _context.Users)
            {
                userList.Add(user);
            }

            return userList;
        }

        public User? GetById(int id)
        {
            foreach (var user in _context.Users)
            {
                if (user.Id == id)
                    return user;
            }
            return null;
        }
        // UPDATE - Modify existing user
        public void Update(User user)
        {
            // Find existing user in database
            var existingUser = GetById(user.Id);
            
            if (existingUser != null)
            {
                // Update the properties
                existingUser.Username = user.Username;
                existingUser.Password = user.Password;
                // CreatedAt stays the same
                
                // Save changes to database
                _context.SaveChanges();
            }
        }
        //DELETE operation
        public void Delete(int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            } 

        } 
    }


}