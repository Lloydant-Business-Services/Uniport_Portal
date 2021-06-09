namespace Abundance_Nk.Model.Model
{

    public class AdmissionList
    {
        public long Id { get; set; }
        public ApplicantJambDetail ApplicantJambDetail {get;set;}
        public ApplicationForm Form { get; set; }
        public AdmissionListBatch Batch { get; set; }
        public Programme Programme { get; set; }
        public Department Deprtment { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
        public bool Activated { get; set; }
        public string JambNumber { get; set; }
    }
    public class UnregisteredStudent
    {
        public string Surname { get; set; }
        public string Firstname { get; set; }
        public string Othername { get; set; }
        public string JambNumberNumber { get; set; }
        public Department Deprtment { get; set; }
        public Programme Programme { get; set; }
        public Session Session { get; set; }

    }

}