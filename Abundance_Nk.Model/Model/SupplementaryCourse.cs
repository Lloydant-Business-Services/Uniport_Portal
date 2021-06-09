using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class SupplementaryCourse
    {
        public Person Person { get; set; }

        [Display(Name = "Supplementary Department")]
        public Department Department { get; set; }

        public ApplicationForm ApplicationForm { get; set; }
        public DepartmentOption Option { get; set; }
        public int Putme_Score { get; set; }
        public decimal Average_Score { get; set; }
    }
}