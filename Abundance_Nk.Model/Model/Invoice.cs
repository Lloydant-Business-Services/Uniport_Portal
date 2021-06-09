namespace Abundance_Nk.Model.Model
{
    public class Invoice
    {
        public Person Person { get; set; }
        public Payment Payment { get; set; }
        public string JambRegistrationNumber { get; set; }
        public string MatricNumber { get; set; }
        public string Level { get; set; }
        public string Department { get; set; }
        public string Session { get; set; }
        public bool Paid { get; set; }
        public RemitaPayment remitaPayment { get; set; }
        public Paystack Paystack { get; set; }
        public PaymentEtranzactType paymentEtranzactType { get; set; }
        public PaymentScholarship paymentScholarship { get; set; }
        public PaymentInterswitch PaymentInterswitch { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public PaymentMonnify PaymentMonnify { get; set; }
        public string ProgrammeName { get; set; }
    }
}