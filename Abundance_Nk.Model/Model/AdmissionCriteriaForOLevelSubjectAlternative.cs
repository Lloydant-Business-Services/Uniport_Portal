namespace Abundance_Nk.Model.Model
{
    public class AdmissionCriteriaForOLevelSubjectAlternative
    {
        public int Id { get; set; }
        public AdmissionCriteriaForOLevelSubject Alternative { get; set; }
        public OLevelSubject OLevelSubject { get; set; }
    }
}