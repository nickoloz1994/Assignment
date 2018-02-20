using Microsoft.EntityFrameworkCore;
using Solution.Models;
using Solution.Services.DAL.App.Interfaces;
using Solution.Services.DAL.EF;

namespace Solution.Services.DAL.App.EF
{
    public class UserSectorRepository : EFRepository<UserSector>, IUserSectorRepository
    {
        public UserSectorRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}