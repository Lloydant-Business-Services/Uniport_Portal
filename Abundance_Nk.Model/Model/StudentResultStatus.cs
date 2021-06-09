namespace Abundance_Nk.Model.Model
{
    public class StudentResultStatus
    {
        public int Id { get; set; }
        public Department Department { get; set; }
        public Level Level { get; set; }
        public Programme Programme { get; set; }
        public bool Activated { get; set; }
    }
}