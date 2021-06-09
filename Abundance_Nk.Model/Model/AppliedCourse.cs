namespace Abundance_Nk.Model.Model
{
    public class AppliedCourse
    {
        public Person Person { get; set; }
        public Programme Programme { get; set; }
        public Department Department { get; set; }
        public ApplicationForm ApplicationForm { get; set; }
        public DepartmentOption Option { get; set; }
        public ApplicationFormSetting Setting { get; set; }
    }
}