using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Solution.Data;
using Solution.Models;
using Solution.Models.UserSectorViewModels;
using Solution.Services.DAL.App.EF;
using Solution.Services.DAL.App.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Solution.Controllers
{
    [Authorize]
    public class UserSectorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserSectorRepository _userSectorRepository;
        private readonly ISectorRepository _sectorRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserSectorController(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userSectorRepository = new UserSectorRepository(_context);
            _sectorRepository = new SectorRepository(_context);
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var ownerId = _userManager.GetUserId(User);

            var userSector = _userSectorRepository.GetAll()
                                    .FirstOrDefault(r => r.UserId.Equals(ownerId));
            var vm = new UserSectorIndexViewModel();

            if (userSector != null)
            {
                string[] sectorIds = userSector.SelectedSectors.Split(',');
                foreach (var sectorId in sectorIds)
                {
                    vm.SelectedSectors.Add(_sectorRepository.Find(Int32.Parse(sectorId)));
                }
                vm.UserSector = userSector;

                return View(vm);
            }

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            var vm = new UserSectorCreateViewModel();
            var sectors = await _sectorRepository.GetAllAsync();
            vm.SectorSelectList = GetCompleteList(sectors);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserSectorCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var userID = _userManager.GetUserId(User);
                StringBuilder builder = new StringBuilder();
                string lastSelection = vm.SelectedSectors.Last();

                foreach (var selection in vm.SelectedSectors)
                {
                    if (!selection.Equals(lastSelection, StringComparison.Ordinal))
                    {
                        builder.Append(selection);
                        builder.Append(",");
                    }
                    else
                    {
                        builder.Append(selection);
                    }
                }

                var userSector = new UserSector
                {
                    UserId = userID,
                    UserName = vm.UserName,
                    SelectedSectors = builder.ToString(),
                    Agreement = vm.Agreement
                };

                await _userSectorRepository.AddAsync(userSector);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var sectors = _sectorRepository.GetAll();
            vm.SectorSelectList = GetCompleteList(sectors);
            return View(vm);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userSector = _userSectorRepository.Find((int)id);
            if (userSector == null)
            {
                return NotFound();
            }

            List<Sector> selectedSectors = new List<Sector>();
            string[] selectionIds = userSector.SelectedSectors.Split(',');

            foreach (var selectionId in selectionIds)
            {
                selectedSectors.Add(_sectorRepository.Find(Int32.Parse(selectionId)));
            }

            var sectors = _sectorRepository.GetAll();
            var vm = new UserSectorEditViewModel
            {
                UserName = userSector.UserName,
                ID = userSector.UserSectorId,
                SectorSelectList = GetCompleteList(sectors),
                SelectionList = new SelectList(selectedSectors, nameof(Sector.Id), nameof(Sector.Name))
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserSectorEditViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var oldVersion = _userSectorRepository.Find(vm.ID);

                if (vm.NewSelection.Any())
                {
                    StringBuilder builder = new StringBuilder();
                    string last = vm.NewSelection.Last();

                    foreach (var selection in vm.NewSelection)
                    {
                        if (!(selection.Equals(last, StringComparison.Ordinal)))
                        {
                            builder.Append(selection);
                            builder.Append(",");
                        }
                        else
                        {
                            builder.Append(selection);
                        }
                    }

                    oldVersion.SelectedSectors = builder.ToString();
                }

                oldVersion.UserName = vm.UserName;

                _userSectorRepository.Update(oldVersion);
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

            var userSector = _userSectorRepository.Find((int)id);
            if (userSector == null)
            {
                return NotFound();
            }

            var vm = new UserSectorDeleteViewModel();
            string[] sectorIds = userSector.SelectedSectors.Split(',');
            foreach (var sectorId in sectorIds)
            {
                vm.SelectedSectors.Add(_sectorRepository.Find(Int32.Parse(sectorId)));
            }

            vm.UserSector = userSector;

            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = _userSectorRepository.Find(id);
            _userSectorRepository.Remove(entity);
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

        public List<Sector> GetCompleteList(IEnumerable<Sector> sectors)
        {
            List<Sector> menuItems = new List<Sector>();

            foreach (var item in sectors)
            {
                if (item.Children.Count() > 0)
                {
                    menuItems.AddRange(GetSectorList(item));
                }
            }

            return menuItems;
        }
    }
}