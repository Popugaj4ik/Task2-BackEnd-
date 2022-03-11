#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task2.Models;
using Task2.Models.DTO;

namespace Task2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUser()
        {
            var list = await _context.User.ToListAsync();

            var listDto = new List<UserDTO>();

            list.ForEach(user => listDto.Add(new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName
            }));

            return listDto;
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(long id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDTO = new UserDTO
            {
                UserName = user.UserName,
                Id = user.Id
            };

            return userDTO;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> PostUser(User user)
        {
            if (_context.User.Any(u => u.eMail == user.eMail || u.UserName == user.UserName))
            {
                return NoContent();
            }

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> LoginUser(User user)
        {
            if (
                await _context.User.AnyAsync(u => u.eMail != user.eMail || u.Password != user.Password)
                && await _context.User.ContainsAsync(user))
            {
                return BadRequest("Bad login or password");
            }

            var list = await _context.User.Where(u => u.eMail == user.eMail).ToListAsync();

            var userDTO = list.Count > 0 ? UserToUserDTO(list.First()) : new UserDTO();

            return CreatedAtAction("GetUser", new { id = userDTO.Id }, userDTO);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        private UserDTO UserToUserDTO(User user) => new UserDTO() { Id = user.Id, UserName = user.UserName };
    }
}
