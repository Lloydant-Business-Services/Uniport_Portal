using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class AdmittedStudentDataView
    {
        public long PersonId { get; set; }
        public string Name { get; set; }
        public string LocalGovernmentName { get; set; }
        public string StateName { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public string ProgrammeName { get; set; }
        public int ProgrammeId { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string SponsorName { get; set; }
        public string RelationshipName { get; set; }
        public string SponsorContactAddress { get; set; }
        public string SponsorMobilePhone { get; set; }
        public string OLevelSubjectName { get; set; }
        public string OLevelGradeName { get; set; }
        public string SessionName { get; set; }
        public int SessionId { get; set; }
        public string OLevelTypeName { get; set; }
        public string OLevelExamSittingName { get; set; }
        public string ImageFileUrl { get; set; }
        public int OLevelExamSittingId { get; set; }
        public string ExamNumber { get; set; }
        public int ExamYear { get; set; }
        public short? ApplicantJambScore { get; set; }
        public string ApplicantJambRegistrationNumber { get; set; }
    }
    public class OlevelSitting
    {
        public string OLevelSubjectName { get; set; }
        public string OLevelGradeName { get; set; }
        public string SessionName { get; set; }
        public int SessionId { get; set; }
        public string OLevelTypeName { get; set; }
        public string OLevelExamSittingName { get; set; }
        public string ImageFileUrl { get; set; }
        public int OLevelExamSittingId { get; set; }
        public string ExamNumber { get; set; }
        public string ExamYear { get; set; }
    }
}
