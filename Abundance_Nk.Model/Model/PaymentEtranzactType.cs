namespace Abundance_Nk.Model.Model
{
    public class PaymentEtranzactType : Setup
    {
        public FeeType FeeType { get; set; }

        public Level Level { get; set; }

        public PaymentMode PaymentMode { get; set; }
        public Programme programme { get; set; }
        public Session Session { get; set; }
    }
}