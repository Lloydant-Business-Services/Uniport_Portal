using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class DepartmentOption : Setup
    {
        [Display(Name = "Course Option")]
        public override int Id { get; set; }

        [Display(Name = "Course Option")]
        public override string Name { get; set; }

        [Display(Name = "Course Option Status")]
        public bool Activated { get; set; }

        public Department Department { get; set; }
    }
}