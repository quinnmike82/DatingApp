using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.Users.ToListAsync(); 
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUsers(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if(user == null){
                return NotFound();
            }
            return user;
        }
    }
}