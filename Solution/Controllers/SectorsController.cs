using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.Data;
using Solution.Models;
using Solution.Models.SectorViewModels;
using Solution.Services.DAL.App.EF;
using Solution.Services.DAL.App.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solution.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class SectorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISectorRepository _sectorRepository;

        public SectorsController(ApplicationDbContext dbContext)
        {
            _context = dbContext;

            _sectorRepository = new SectorRepository(_context);
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(int? page)
        {
            var sectors = from s in _context.Sectors
                          select s;

            int pageSize = 10;
            return View(await PaginatedList<Sector>.CreateAsync(sectors, page ?? 1, pageSize));
        }

        public async Task<IActionResult> Create()
        {
            var sectors = await _sectorRepository.GetAllAsync();

            var vm = new SectorCreateViewModel
            {
                ParentSectorSelectList = GetCompleteList(sectors)
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SectorCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var newSector = new Sector { Name = vm.Name };

                if (vm.ParentSectorId == null)
                {
                    newSector.HierarchyLevel = 0;
                    await _sectorRepository.AddAsync(newSector);
                }
                else
                {
                    var parent = _sectorRepository.Find(vm.ParentSectorId);
                    newSector.HierarchyLevel = parent.HierarchyLevel + 1;
                    parent.Children.Add(newSector);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var sectors = await _sectorRepository.GetAllAsync();

            if (id == null)
            {
                return NotFound();
            }

            var sector = _sectorRepository.Find((int)id);
            if (sector == null)
            {
                return NotFound();
            }

            var vm = new SectorEditViewModel
            {
                Sector = sector
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SectorEditViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _sectorRepository.Update(vm.Sector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sectorToDelete = _sectorRepository.Find((int)id);
            if (sectorToDelete == null)
            {
                return NotFound();
            }

            return View(sectorToDelete);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sector = _sectorRepository.GetAll().FirstOrDefault(s => s.Id == id);
            await DeleteBranch(sector, _sectorRepository, _context);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // method for getting sorted list of sectors
        public List<Sector> GetSectorList(Sector sector)
        {
            List<Sector> list = new List<Sector>();

            if (sector.Children.Count() > 0)
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

        // method for generating complete list for menu
        public List<Sector> GetCompleteList(IEnumerable<Sector> sectors)
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

        // method for cascade DELETE operation
        public async Task DeleteBranch(Sector sector, ISectorRepository repository, ApplicationDbContext context)
        {
            if (sector.Children.Count() > 0)
            {
                foreach (var child in sector.Children)
                {
                    await DeleteBranch(child, repository, context);
                }
            }

            repository.Remove(sector);
        }
    }
}