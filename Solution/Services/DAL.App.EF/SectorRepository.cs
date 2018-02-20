using Microsoft.EntityFrameworkCore;
using Solution.Models;
using Solution.Services.DAL.App.Interfaces;
using Solution.Services.DAL.EF;

namespace Solution.Services.DAL.App.EF
{
    public class SectorRepository : EFRepository<Sector>, ISectorRepository
    {
        public SectorRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}