using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Areas.PGApplicant.ViewModels
{
    public class PGRefereeViewModel
    {
        public Model.Model.Applicant Applicant { get; set; }
        public ApplicantReferee ApplicantReferee { get; set; }
        public ApplicantRefereeResponse ApplicantRefereeResponse { get; set; }
        public AppliedCourse AppliedCourse { get; set; }
        public List<RefereeEvaluation> RefereeEvaluations { get; set; }
        public List<GradingSystem> GradingSystems { get; set; }
        public List<GradingSystem> OverallGradingSystems { get; set; }
        public int OverallRating { get; set; }
        public bool Completed { get; set; }
        public void SetRefereeGradingResponse(List<RefereeGradingCategory> refereeGradingCategories, List<RefereeGradingSystem> refereeGradingSystems)
        {
            try
            {
                RefereeEvaluations = new List<RefereeEvaluation>();
                
                if (refereeGradingCategories?.Count > 0 && refereeGradingSystems?.Count>0)
                {
                    foreach(var item in refereeGradingCategories)
                    {
                        RefereeEvaluation refereeEvaluation = new RefereeEvaluation();
                        refereeEvaluation.GradeCategoryId = item.Id;
                        refereeEvaluation.GradeCategory = item.GradeCategory;
                        refereeEvaluation.GradingSystems = new List<GradingSystem>();
                        foreach (var element in refereeGradingSystems)
                        {
                            GradingSystem gradingSystem = new GradingSystem();
                            gradingSystem.Id = element.Id;
                            gradingSystem.score = element.Score;
                            gradingSystem.isChecked = false;
                            refereeEvaluation.GradingSystems.Add(gradingSystem);
                        }
                        RefereeEvaluations.Add(refereeEvaluation);
                    }
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void SetOverallGradingSysytem(List<RefereeGradingSystem> refereeGradingSystems)
        {
            OverallGradingSystems = new List<GradingSystem>();
            try
            {
                if (refereeGradingSystems?.Count > 0)
                {
                    foreach(var item in refereeGradingSystems)
                    {
                        GradingSystem gradingSystem = new GradingSystem();
                        gradingSystem.Id = item.Id;
                        gradingSystem.isChecked = false;
                        gradingSystem.score = item.Score;
                        OverallGradingSystems.Add(gradingSystem);
                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
    public class RefereeEvaluation
    {
        public int GradeCategoryId { get; set; }
        public bool isChecked { get; set; }
        public string GradeCategory { get; set; }
        public int GradeSystemId { get; set; }
        public List<GradingSystem> GradingSystems { get; set; }
    }
    public class GradingSystem
    {
        public int Id { get; set; }
        public string score { get; set; }
        public bool isChecked { get; set; }
    }
}