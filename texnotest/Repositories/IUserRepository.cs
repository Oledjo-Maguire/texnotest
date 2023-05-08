using System.Collections.Generic;
using System.Threading.Tasks;
using texnotest.Models;

namespace texnotest.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task<List<User>> GetAllUsersAsync();
        Task<User> AddUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
    }
}