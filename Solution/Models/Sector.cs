using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Solution.Models
{
    public class Sector
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Sector Name")]
        public string Name { get; set; }

        //public Sector ParentSector { get; set; }

        //[Display(Name = "Category")]
        //public int? ParentSectorId { get; set; }

        public int HierarchyLevel { get; set; }

        public ICollection<Sector> Children { get; set; } = new List<Sector>();
    }
}