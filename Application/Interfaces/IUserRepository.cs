using CleanRestApi.Models; //importing User.cs classes 

namespace CleanRestApi.Application.Interfaces
{
    public interface IUserRepository
    {
        User? GetByUsername(string Username); //nullable return type
        void Add(User user);                  //add new user to storage
        List<User> GetAll();  //get all users - READ operation
        User? GetById(int id);  // get users by ID - READ operation
        void Update(User user); //update existing user - UPDATE operation
        void Delete(int id); //delete user ID - Delete operation
    }
}

