using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Model
{
    public class CourseEvaluationReport
    {

        public string Programme { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string Level { get; set; }
        public string Session { get; set; }
        public string Semester { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public long? Score { get; set; }
        public int? NumberOfStudent { get; set; }
        public string LecturerName { get; set; }
        public long PersonId { get; set; }
        public long CourseId { get; set; }
        public string Question { get; set; }
        public int Section { get; set; }
        public List<CourseEvaluationAnswer> CourseEvaluationAnswerSection1 { get; set; }
        public List<CourseEvaluationAnswer> CourseEvaluationAnswerSection2 { get; set; }
        public int QuestionId { get; set; }
    }
}
