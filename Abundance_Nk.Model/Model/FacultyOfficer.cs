namespace Abundance_Nk.Model.Model
{
    public class FacultyOfficer
    {
        public User Officer { get; set; }
        public Faculty Faculty { get; set; }
        public string Description { get; set; }
    }
}