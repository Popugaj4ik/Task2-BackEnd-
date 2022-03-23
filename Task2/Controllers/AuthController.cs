using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Task2.Models;
using Task2.Models.DTO;

namespace Task2.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet,Route("refreshToken")]
        [Authorize]
        public ActionResult<string> RefreshToken()
        {
            return Ok(new { Token = CreateToken() });
        }

        [HttpPost, Route("login")]
        public async Task<ActionResult<UserDTO>> LoginUser(User user)
        {
            if (user == null)
                return BadRequest("Invalid client request");

            user.Password = HashPassword(user.Password);

            if (_context.User.Any(u => u.eMail == user.eMail || u.UserName == user.UserName))
            {
                var list = await _context.User.Where(u => u.eMail == user.eMail).ToListAsync();

                var userDTO = list.Count > 0 ? UserToUserDTO(list.First()) : new UserDTO();

                userDTO.Token = CreateToken();

                return Ok(userDTO);
            }

            return Unauthorized();
        }

        [HttpPost, Route("registerUser")]
        public async Task<ActionResult<UserDTO>> RegisterUser(User user)
        {
            if (_context.User.Any(u => u.eMail == user.eMail || u.UserName == user.UserName))
            {
                return NoContent();
            }

            user.Password = HashPassword(user.Password);

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var dto = UserToUserDTO(user);
            dto.Token = CreateToken();

            return Ok(dto);
        }

        private string HashPassword(string password)
        {
            var md5hash = MD5.Create();

            var sourcebytesBytes = Encoding.UTF8.GetBytes(password);

            var hashbytes = md5hash.ComputeHash(sourcebytesBytes);

            var hash = BitConverter.ToString(hashbytes).Replace("-", String.Empty);

            return hash;
        }
        private string CreateToken()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretKey@667890"));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptins = new JwtSecurityToken(
                issuer: "https://localhost:7047",
                audience: "https://localhost:4200/",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler();

            var token = handler.WriteToken(tokenOptins);

            return token;
        }
        private UserDTO UserToUserDTO(User user) => new UserDTO() { Id = user.Id, UserName = user.UserName };
    }
}
