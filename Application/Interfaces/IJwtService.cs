using CleanRestApi.Models;

namespace CleanRestApi.Application.Interfaces
{
    public interface IJwtService
    {
        string CreateToken(User user);
    }
}