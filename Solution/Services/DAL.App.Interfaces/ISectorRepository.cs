using Solution.Models;
using Solution.Services.DAL.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Solution.Services.DAL.App.Interfaces
{
    public interface ISectorRepository : IRepository<Sector>
    {
        IEnumerable<Sector> GetSectorList(Sector sector);

        IEnumerable<Sector> GetCompleteList(IEnumerable<Sector> sectors);

        Task DeleteBranch(Sector sector);
    }
}