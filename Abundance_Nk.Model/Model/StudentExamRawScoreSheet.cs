namespace Abundance_Nk.Model.Model
{
    public class StudentExamRawScoreSheet
    {
        public long Id { get; set; }
        public Student Student { get; set; }
        public double? QU1 { get; set; }
        public double? QU2 { get; set; }
        public double? QU3 { get; set; }
        public double? QU4 { get; set; }
        public double? QU5 { get; set; }
        public double? QU6 { get; set; }
        public double? QU7 { get; set; }
        public double? QU8 { get; set; }
        public double? QU9 { get; set; }
        public double? T_EX { get; set; }
        public double? T_CA { get; set; }
        public double? EX_CA { get; set; }
        public string Remark { get; set; }
        public string Special_Case { get; set; }
        public Session Session { get; set; }
        public Semester Semester { get; set; }
        public Course Course { get; set; }
        public Programme Programme { get; set; }
        public Level Level { get; set; }
        public User Uploader { get; set; }
        public string MatricNumber { get; set; }
        public string FileUploadURL { get; set; }
    }
}