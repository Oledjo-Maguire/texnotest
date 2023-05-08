using Microsoft.EntityFrameworkCore;
using texnotest.Models;
using texnotest.Repositories;

namespace texnotest
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserState _activeUserState;
        private readonly UserState _blockedUserState;
        private readonly UserGroup _adminUserGroup;
        private readonly UserGroup _userUserGroup;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
            _activeUserState = _context.UserStates.Single(us => us.Code == "Active");
            _blockedUserState = _context.UserStates.Single(us => us.Code == "Blocked");
            _adminUserGroup = _context.UserGroups.Single(ug => ug.Code == "Admin");
            _userUserGroup = _context.UserGroups.Single(ug => ug.Code == "User");
        }

        public async Task<User> AddUserAsync(string login, string password)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Login == login);
            if (existingUser != null)
            {
                throw new ApplicationException("Пользователь с таким логином уже существует.");
            }

            var user = new User
            {
                Login = login,
                Password = password,
                CreatedDate = DateTime.Now,
                UserStateId = _activeUserState.Id,
                UserGroupId = _userUserGroup.Id
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
             return await _context.Users.ToListAsync();
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(User user)
        {
            user.UserStateId = _context.UserStates.FirstOrDefault(us => us.Code == "Blocked").Id;
            await _context.SaveChangesAsync();
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                throw new ArgumentException($"User with id {id} not found");
            }

            if (user.UserGroupId == _adminUserGroup.Id)
            {
                throw new InvalidOperationException("Cannot delete admin user");
            }

            user.UserStateId = _blockedUserState.Id;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
