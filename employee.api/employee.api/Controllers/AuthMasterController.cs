//using employee.api.Modal;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity.Data;
//using Microsoft.AspNetCore.Mvc;

//namespace employee.api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthMasterController : ControllerBase
//    {
//    }
//}


using employee.api.DTOs;
using employee.api.Modal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthMasterController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public AuthMasterController(EmployeeDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _context.Auths
                .FirstOrDefaultAsync(x => x.Email == request.Email || x.UserName == request.UserName);

            if (existingUser != null)
                return BadRequest("Username or email already exists.");

            var user = new Auth
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            _context.Auths.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully",
                userId = user.UserId
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Auths
                .FirstOrDefaultAsync(x =>
                    (x.UserName == request.UserNameOrEmail || x.Email == request.UserNameOrEmail)
                    && x.Password == request.Password);

            if (user == null)
                return Unauthorized("Invalid username/email or password.");

            return Ok(new
            {
                message = "Login successful",   
                user = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName
                }
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Auths
                .FirstOrDefaultAsync(x => x.Email == request.Email);

            if (user == null)
                return NotFound("User not found.");

            user.Password = request.NewPassword;

            await _context.SaveChangesAsync();

            return Ok("Password reset successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Auths
                .Select(user => new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}