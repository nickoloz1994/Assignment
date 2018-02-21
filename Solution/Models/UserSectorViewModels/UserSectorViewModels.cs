using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Solution.Models.UserSectorViewModels
{
    public class UserSectorIndexViewModel
    {
        public UserSector UserSector { get; set; }
        public List<Sector> SelectedSectors { get; set; } = new List<Sector>();
    }

    public class UserSectorCreateViewModel
    {
        [Required(ErrorMessage = "Please, enter your name")]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        public List<Sector> SectorSelectList { get; set; } = new List<Sector>();

        [Required(ErrorMessage = "Please, select sectors you are involved in")]
        public string[] SelectedSectors { get; set; } = new string[] { };

        [Required]
        [Display(Name = "Terms & Conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please, agree to our terms & conditions")]
        public bool Agreement { get; set; }
    }

    public class UserSectorEditViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        public int ID { get; set; }
        public List<Sector> SectorSelectList { get; set; } = new List<Sector>();

        [Display(Name = "Old selection")]
        public SelectList SelectionList { get; set; }

        [Display(Name = "New selection")]
        public string[] NewSelection { get; set; } = new string[] { };
    }

    public class UserSectorDeleteViewModel
    {
        public UserSector UserSector { get; set; }
        public List<Sector> SelectedSectors { get; set; } = new List<Sector>();
    }
}