using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class ApplicantStatus : BasicSetup
    {
        public enum Status
        {
            SubmittedApplicationForm = 1,
            OfferedAdmission = 2,
            GeneratedAcceptanceInvoice = 3,
            GeneratedAcceptanceReceipt = 4,
            OLevelResultVerified = 5,
            ClearedAndAccepted = 6,
            ClearedAndRejected = 7,
            GeneratedSchoolFeesInvoice = 8,
            GeneratedSchoolFeesReceipt = 9,
            CompletedStudentInformationForm = 10,
            CompeledCourseRegistrationForm = 11,
        }

        [Display(Name = "Applicant Status")]
        public override int Id { get; set; }
    }
}