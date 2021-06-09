using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class PaymentEtranzactView
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
        public string MearchantReference { get; set; }

        public bool Issuccessful { get; set; }
        public string ResponseDecription { get; set; }
        public string ResponseCode { get; set; }
        public string PaymentMode { get; set; }
    }

    public class PaymentReportArray
    {
        public string FeeTypeName { get; set; }
        public string Count { get; set; }
        public string Amount { get; set; }
        public string FeeTypeId { get; set; }
        public string FullName { get; set; }
        public string MatricNumber { get; set; }
        public string JambNumber { get; set; }
        public string LevelName { get; set; }
        public string DepartmentName { get; set; }
        public string FacultyName { get; set; }
        public string ProgrammeName { get; set; }
        public string SessionName { get; set; }
        public string TransactionAmount { get; set; }
        public string TransactionDate { get; set; }
        public string ConfirmationNo { get; set; }
        public string InvoiceNumber { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsError { get; set; }
        public string PaymentEtranzactView { get; set; }
        public string PaymentMode { get; set; }

        public bool Issuccessful { get; set; }
        public string ResponseDecription { get; set; }
        public string MearchantReference { get; set; }
    }

    public class PaymentReportDetail
    {
        public string Message { get; set; }
        public bool IsError { get; set; }
        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }
        public int TotalCount { get; set; }
        public string TotalAmount { get; set; }
        public string OverallAmount { get; set; }
        public int OverallCount { get; set; }
        public string TransactionDate { get; set; }
        public string MatricNumber { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Programme { get; set; }
        public string Session { get; set; }

        public string InvoiceNumber { get; set; }
        public string ConfirmationNumber { get; set; }
        public string Amount { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentModeName { get; set; }
        public int ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string FeeName { get; set; }
        public decimal FeeAmount { get; set; }
        public int FeeId { get; set; }
        public decimal TotalFeeAmount { get; set; }
        public string PaymentSummary { get; set; }
    }
    public class PaymentVerificationReportData
    {
        public string FeeTypeName { get; set; }
        public string Count { get; set; }
        public string Amount { get; set; }
        public string FeeTypeId { get; set; }
        public string FullName { get; set; }
        public string MatricNumber { get; set; }
        public string LevelName { get; set; }
        public string DepartmentName { get; set; }
        public string FacultyName { get; set; }
        public string ProgrammeName { get; set; }
        public string SessionName { get; set; }
        public string TransactionAmount { get; set; }
        public string TransactionDate { get; set; }
        public string ConfirmationNo { get; set; }
        public string InvoiceNumber { get; set; }
        public string ErrorMessage { get; set; }
        public string Receipt { get; set; }
        public bool IsError { get; set; }
        public string PaymentVerificationView { get; set; }
        public string VerificationOfficer { get; set; }
        public long UserId { get; set; }
    }

}

