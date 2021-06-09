using System;

namespace Abundance_Nk.Model.Model
{
    public class PaymentView
    {
        public long PersonId { get; set; }
        public long PaymentId { get; set; }
        public string InvoiceNumber { get; set; }
        public string ReceiptNumber { get; set; }
        public string ConfirmationOrderNumber { get; set; }

        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        public DateTime InvoiceGenerationDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; }

        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }

        public decimal? Amount { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentModeName { get; set; }
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string MobilePhone { get; set; }
        public string MatricNumber { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public int ProgrammeId { get; set; }
        public string ProgrammeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string TransactionDate { get; set; }
        public int NewStudentDebtorsCount { get; set; }
        public int OldStudentDebtorsCount { get; set; }
        public int TotalDebtorsCount { get; set; }
        public string FormatedAmount { get; set; }
        public string AccountNumber { get; set; }
    }
    public class BankPaymentCount
    {
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public int Count { get; set; }

    }
    public class FeeTypePaymentCount
    {
        public string FeeTypeName { get; set; }
        public int FeeTypeId { get; set; }
        public int Count { get; set; }

    }
    public class ChannelPaymentCount
    {
        public string Channel { get; set; }
        public int Count { get; set; }

    }
    public class PaymentCountAmount
    {
        public decimal? Amount { get; set; }
        public int Count { get; set; }

    }
}