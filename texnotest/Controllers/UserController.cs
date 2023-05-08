using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using texnotest.Models;
using texnotest.Repositories;

namespace texnotest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
            
        }
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        // methods for adding, deleting, and getting users will go here

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserGroup)
                .Include(u => u.UserState)
                .ToListAsync();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserGroup)
                .Include(u => u.UserState)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Login == user.Login))
            {
                return BadRequest("A user with this login already exists.");
            }

            var userGroup = await _context.UserGroups.FirstOrDefaultAsync(g => g.Code == "User");
            var userState = await _context.UserStates.FirstOrDefaultAsync(s => s.Code == "Active");

            if (userGroup == null || userState == null)
            {
                return BadRequest("Cannot create user. UserGroup or UserState is missing.");
            }

            user.CreatedDate = DateTime.Now;
            user.UserStateId = userState.Id;
            user.UserGroupId = userGroup.Id;

            // добавляем пользователя в контекст базы данных
            _context.Users.Add(user);

            // сохраняем изменения
            await _context.SaveChangesAsync();

            // возвращаем созданный объект пользователь вместе с кодом статуса 201 Created
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);

        }
        public async Task<User> AddUserAsync(User user)
        {
            if (await _context.Users.AnyAsync(x => x.Login == user.Login))
            {
                throw new Exception("User with this login already exists");
            }

            var userState = await _context.UserStates.FirstOrDefaultAsync(x => x.Code == "Active");
            user.CreatedDate = DateTime.Now;
            user.UserStateId = userState.Id;

            var userGroup = await _context.UserGroups.FirstOrDefaultAsync(x => x.Code == "User");
            user.UserGroupId = userGroup.Id;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUser(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserGroup)
                .Include(u => u.UserState)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

    }

    /*   public class UserController : Controller
       {
           // GET: UserController
           public ActionResult Index()
           {
               return View();
           }

           // GET: UserController/Details/5
           public ActionResult Details(int id)
           {
               return View();
           }

           // GET: UserController/Create
           public ActionResult Create()
           {
               return View();
           }

           // POST: UserController/Create
           [HttpPost]
           [ValidateAntiForgeryToken]
           public ActionResult Create(IFormCollection collection)
           {
               try
               {
                   return RedirectToAction(nameof(Index));
               }
               catch
               {
                   return View();
               }
           }

           // GET: UserController/Edit/5
           public ActionResult Edit(int id)
           {
               return View();
           }

           // POST: UserController/Edit/5
           [HttpPost]
           [ValidateAntiForgeryToken]
           public ActionResult Edit(int id, IFormCollection collection)
           {
               try
               {
                   return RedirectToAction(nameof(Index));
               }
               catch
               {
                   return View();
               }
           }

           // GET: UserController/Delete/5
           public ActionResult Delete(int id)
           {
               return View();
           }

           // POST: UserController/Delete/5
           [HttpPost]
           [ValidateAntiForgeryToken]
           public ActionResult Delete(int id, IFormCollection collection)
           {
               try
               {
                   return RedirectToAction(nameof(Index));
               }
               catch
               {
                   return View();
               }
           }
       }*/
}
