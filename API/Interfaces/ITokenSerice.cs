using API.Entities;

namespace API.Interfaces
{
    public interface ITokenSerice
    {
         string CreateToken(AppUser user);
    }
}