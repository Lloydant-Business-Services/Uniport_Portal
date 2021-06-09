namespace Abundance_Nk.Model.Model
{
    public class AdmissionCriteriaForOLevelType
    {
        //[Display(Name = "O-Level Type Admission Criteria")]
        public int Id { get; set; }
        public AdmissionCriteria MainCriteria { get; set; }
        public OLevelType OLevelType { get; set; }
    }
}