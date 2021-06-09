namespace Abundance_Nk.Model.Model
{
    public class PaymentTerminal
    {
        public long Id { get; set; }
        public string TerminalId { get; set; }
        public FeeType FeeType { get; set; }
        public Session Session { get; set; }
    }
}