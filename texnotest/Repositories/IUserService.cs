using texnotest.Models;
namespace texnotest.Repositories
{
    public interface IUserService
    {
        Task<User> GetUserById(int id);
        Task<List<User>> GetAllUsers();
        Task<User> AddUser(User userDto);
        Task UpdateUser(User userDto);
        Task DeleteUser(int id);
    }

}
