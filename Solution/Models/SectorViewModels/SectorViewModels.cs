using System.Collections.Generic;

namespace Solution.Models.SectorViewModels
{
    public class SectorCreateViewModel
    {
        public int? ParentSectorId { get; set; }
        public string Name { get; set; }
        public IEnumerable<Sector> ParentSectorSelectList { get; set; } = new List<Sector>();
    }

    public class SectorEditViewModel
    {
        public Sector Sector { get; set; }
    }
}