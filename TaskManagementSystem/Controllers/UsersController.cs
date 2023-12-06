using System;
using System.Text;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using TaskManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Build.Framework;

namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TaskManagementContext _context;
        //private readonly UserManager<User> _userManager;

        public UsersController(TaskManagementContext context)
        {
            _context = context;
            //_userManager = userManager;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserID)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        public class UserLoginModel
        {
            [Required]
            public string UserName { get; set; }
            [Required]
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == loginModel.UserName && x.Password == loginModel.Password);
            if (user == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var identity = GetIdentity(user);

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return new JsonResult(response);
        }
        //[HttpPost("login")]
        //public async Task<IActionResult> Login(string username, string password)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username && x.Password == password);
        //    if (user == null)
        //    {
        //        return BadRequest(new { errorText = "Invalid username or password." });
        //    }

        //    var identity = GetIdentity(user);

        //    var now = DateTime.UtcNow;
        //    var jwt = new JwtSecurityToken(
        //        issuer: AuthOptions.ISSUER,
        //        audience: AuthOptions.AUDIENCE,
        //        notBefore: now,
        //        claims: identity.Claims,
        //        expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
        //        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        //    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        //    var response = new
        //    {
        //        access_token = encodedJwt,
        //        username = identity.Name
        //    };

        //    return new JsonResult(response);
        //}

        private ClaimsIdentity GetIdentity(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
                // Здесь можно добавить другие пользовательские claims
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
          if (_context.Users == null)
          {
              return Problem("Entity set 'TaskManagementContext.Users'  is null.");
          }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserID }, user);
        }


        [Authorize]
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserID == id)).GetValueOrDefault();
        }
    }
}
