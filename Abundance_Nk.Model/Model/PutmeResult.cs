using System.ComponentModel.DataAnnotations;

namespace Abundance_Nk.Model.Model
{
    public class PutmeResult
    {
        public int Id { get; set; }

        [RegularExpression("^(5)([0-9]{7}[A-Z]{2})$", ErrorMessage = "JAMB Registration No is not valid")]
        public string RegNo { get; set; }

        public string ExamNo { get; set; }

        public string FullName { get; set; }

        public string Jambscore { get; set; }

        public string Course { get; set; }

        public double? RawScore { get; set; }

        public double? Total { get; set; }

        public Person Person { get; set; }
    }

    public class PutmeSlip
    {
        public string JambNumber { get; set; }
        public string Fullname { get; set; }
        public double? JambScore { get; set; }
        public double? ExamScore { get; set; }
        public double? AverageScore { get; set; }
        public string passportUrl { get; set; }
        public string Department { get; set; }
        public string ApplicationNumber { get; set; }
        public string ExamNumber { get; set; }
        public string State { get; set; }
        public string LocalGoverment { get; set; }
        public string Sex { get; set; }
        public long? PersonId { get; set; }
        public string OLevelExamYear { get; set; }
        public string OLevelExamNumber { get; set; }
        public string OLevelExamType { get; set; }
        public int OLevelSitting { get; set; }
        public string OLevelResult { get; set; }
        public string Sitting { get; set; }
        public string OLevelSubject { get; set; }
        public string OLevelGrade { get; set; }
        public string SchoolChoice { get; set; }
        

    }


}