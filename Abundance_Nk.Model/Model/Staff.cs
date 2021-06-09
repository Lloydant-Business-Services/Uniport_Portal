namespace Abundance_Nk.Model.Model
{
    public class Staff 
    {
        public long Id { get; set; }
        public StaffType Type { get; set; }
        public Person Person { get; set; }
        public User User { get; set; }
        public Department Department { get; set; }
        public bool isHead { get; set; }
        public bool isManagement { get; set; }
        public string ProfileDescription { get; set; }
        public int GenotypeId { get; set; }
        public int BloodGroopId { get; set; }
        public int MaritalStatusId { get; set; }
        public Genotype Genotype { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public Designation Designation { get; set; }
        public DepartmentOption DepartmentOption { get; set; }
    }
}