using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentPaystackView
    {
        public long PaymentId { get; set; }
        public long PersonId { get; set; }
        public int PaymentModeId { get; set; }
        public int PaymentTypeId { get; set; }
        public int PersonTypeId { get; set; }
        public int FeeTypeId { get; set; }
        public int? SessionId { get; set; }
        public long? PaymenSerialNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime DatePaid { get; set; }
        public string ConfirmationNo { get; set; }
        public string CustomerName { get; set; }
        public decimal? TransactionAmount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string FeeTypeName { get; set; }
        public string FullName { get; set; }
        public string MatricNumber { get; set; }
        public string JambNumber { get; set; }
        public string LevelName { get; set; }
        public string DepartmentName { get; set; }
        public string FacultyName { get; set; }
        public string ProgrammeName { get; set; }
        public string SessionName { get; set; }
        public string PaymentMode { get; set; }
    }
}
