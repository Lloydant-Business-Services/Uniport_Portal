namespace Abundance_Nk.Model.Model
{
    public class StudentMatricNumberAssignment
    {
        public Faculty Faculty { get; set; }
        public Level Level { get; set; }
        public Department Department { get; set; }
        public Programme Programme { get; set; }
        public Session Session { get; set; }
        public int MatricSerialNoStartFrom { get; set; }
        public string MatricNoStartFrom { get; set; }
        public bool Used { get; set; }
    }
}