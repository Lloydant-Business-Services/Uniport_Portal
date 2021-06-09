using System.Collections.Generic;

namespace Abundance_Nk.Model.Model
{
    public class AdmissionCriteriaForOLevelSubject
    {
        public int Id { get; set; }
        public AdmissionCriteria MainCriteria { get; set; }
        public OLevelSubject Subject { get; set; }
        public OLevelGrade MinimumGrade { get; set; }
        public bool IsCompulsory { get; set; }

        public List<AdmissionCriteriaForOLevelSubjectAlternative> Alternatives { get; set; }
    }
}