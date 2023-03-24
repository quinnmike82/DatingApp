using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenSerice _tokenSerice;

        public AccountController(DataContext context, ITokenSerice tokenSerice)
        {
            _tokenSerice = tokenSerice;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExisted(registerDTO.Username)) return BadRequest("Username is taken!");

            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                Username = registerDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO(){
                Username = user.Username,
                Token = _tokenSerice.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDTO.Username);
            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            if (!computedHash.SequenceEqual(user.PasswordHash)) return Unauthorized("Invalid password");

            return new UserDTO(){
                Username = user.Username,
                Token = _tokenSerice.CreateToken(user)
            };
        }

        private async Task<bool> UserExisted(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username.ToLower());
        }
    }
}