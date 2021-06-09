using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Student.ViewModels
{
    public class ResultViewModel
    {
        public ResultViewModel()
        {
            ScoreSelectListItem = Utility.PopulateJambScoreSelectListItem(1,4);
        }

        public string MatricNumber { get; set; }
        public StudentLevel StudentLevel { get; set; }
        public CourseEvaluationQuestion CourseEvaluationQuestion { get; set; }
        public List<CourseEvaluationQuestion> CourseEvaluationQuestionsForSectionOne { get; set; }
        public List<CourseEvaluationQuestion> CourseEvaluationQuestionsForSectionTwo { get; set; }
        public List<CourseEvaluation> CourseEvaluations { get; set; }
        public List<CourseEvaluation> CourseEvaluationsTwo { get; set; }
        public List<Course> Courses { get; set; }
        public List<SelectListItem> ScoreSelectListItem { get; set; }
        public Semester Semester { get; set; }
    }
}