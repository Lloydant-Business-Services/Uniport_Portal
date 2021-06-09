namespace Abundance_Nk.Model.Model
{
    public class CourseUnit
    {
        public int Id { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public Level Level { get; set; }
        public Semester Semester { get; set; }
        public byte MinimumUnit { get; set; }
        public byte MaximumUnit { get; set; }
    }
}