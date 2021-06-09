namespace Abundance_Nk.Model.Model
{
    public class FeeDetail
    {
        public int SN { get; set; }
        public int Id { get; set; }
        public Fee Fee { get; set; }
        public FeeType FeeType { get; set; }
        public Programme Programme { get; set; }
        public Level Level { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public Department Department { get; set; }
        public Session Session { get; set; }
    }
}