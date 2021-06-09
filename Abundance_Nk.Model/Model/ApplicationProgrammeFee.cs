using System;

namespace Abundance_Nk.Model.Model
{
    public class ApplicationProgrammeFee
    {
        public int Id { get; set; }
        public Programme Programme { get; set; }
        public FeeType FeeType { get; set; }
        public Session Session { get; set; }
        public DateTime DateEntered { get; set; }
    }
}