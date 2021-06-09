namespace Abundance_Nk.Model.Model
{
    public class PaymentChannel : BasicSetup
    {
        public enum Channels
        {
            Etranzact = 1
        }

        public bool Status { get; set; }
    }
}