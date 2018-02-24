using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task DeleteBranch(Sector sector)
        {
            if (sector.Children.Count > 0)
            {
                foreach (var child in sector.Children)
                {
                    await DeleteBranch(child);
                }
            }

            RepositoryDbSet.Remove(sector);
        }

        public IEnumerable<Sector> GetCompleteList(IEnumerable<Sector> sectors)
        {
            List<Sector> menuItems = new List<Sector>();

            foreach (var item in sectors)
            {
                if (item.HierarchyLevel == 0)
                {
                    menuItems.AddRange(GetSectorList(item));
                }
            }

            return menuItems;
        }

        public IEnumerable<Sector> GetSectorList(Sector sector)
        {
            List<Sector> list = new List<Sector>();

            if (sector.Children.Count > 0)
            {
                list.Add(sector);

                foreach (var child in sector.Children)
                {
                    list.AddRange(GetSectorList(child));
                }
            }
            else
            {
                list.Add(sector);
            }

            return list;
        }
    }
}