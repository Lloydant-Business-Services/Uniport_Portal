namespace Abundance_Nk.Model.Model
{
    public class ShortFall
    {
        public long Id { get; set; }
        public Payment Payment { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
    }
}