namespace Abundance_Nk.Model.Model
{
    public class StudentPayment : Payment
    {
        public Person Student { get; set; }
        public Level Level { get; set; }
        public decimal Amount { get; set; }
        public bool Status { get; set; }
    }
}