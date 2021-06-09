using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
      public class AllPaymentGeneralView
    {
       public string Student_Name { get; set; }
       public string Department { get; set; }
       public int? DepartmentId { get; set; }
       public string FeeTypes { get; set; }
       public int FeeTypesId { get; set; }
       public string Programme { get; set; }
       public int ProgrammeId { get; set; }
       public string level { get; set; }
       public decimal? Amount { get; set; }
       public int Session { get; set; }
       public DateTime? Date { get; set; }
       public string StartDate { get; set; }
       public string EndDate { get; set; }

    }
}
