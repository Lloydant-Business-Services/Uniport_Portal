namespace Abundance_Nk.Model.Model
{
    public class ProgrammeDepartment
    {
        public int Id { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public bool Activate { get; set; }
        public bool ActivePUTMEApplication { get; set; }
    }
}