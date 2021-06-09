using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class Fee : BasicSetup
    {
        [Display(Name = "Fee Type")]
        public override int Id { get; set; }

        [Required]
        [Display(Name = "Fee Type")]
        public override string Name { get; set; }

        [Required]
        public decimal Amount { get; set; }

        //[Required]
        //[DataType(DataType.Date)]
        //[DisplayName("Date Entered")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        //public DateTime DateEntered { get; set; }
    }
}