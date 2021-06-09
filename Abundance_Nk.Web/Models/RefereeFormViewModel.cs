using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Web.Models
{
    public class RefereeFormViewModel
    {
        public long ApplicationFormId { get; set; }

        public long ApplicantRefereeId { get; set; }

        [Display(Name = "Candidate's Name")]
        public string CandidateName { get; set; }

        [Display(Name = "Department/Faculty Name")]
        public string DepartmentFacultyName { get; set; }

        [Display(Name = "Programme")]
        public string ProgrammeName { get; set; }

        [Display(Name = "How long have you known the Candidate?")]
        public decimal LengthOfFamiliarityTime { get; set; }

        [Display(Name = "Comment on the Candidate's personality")]
        public string CharacterComment { get; set; }

        [Display(Name ="Would you accept the candidate as a grad student?")]
        public bool AcceptAsGraduateStudent { get; set; }

        [Display(Name = "Candidate's Overall Promise")]
        public string OverallPromise { get; set; }//lower table holding details from: select * from REFEREE_GRADING_SYSTEM where([Type] = 2)

        public string CandidateSuitabilty { get; set; }

        public string CandidateEligibity { get; set; }

        public string RefereeFullName { get; set; }

        public string InstitutionName { get; set; }

        [Display(Name = "Select Session")]
        public int SessionId { get; set; }

        [Display(Name = "Select Programme")]
        public int ProgrammeId { get; set; }

        [Display(Name = "Select Department")]
        public int DepartmentId { get; set; }

        public List<RefereeGradingCategory> RefereeGradingCategories { get; set; } = new List<RefereeGradingCategory>();

        public List<RefereeGradingSystem> RefereeGradingSystems { get; set; } = new List<RefereeGradingSystem>();

        public List<ApplicantRefereeGradingResponse> ApplicantRefereeGradingResponses { get; set; } = new List<ApplicantRefereeGradingResponse>();

        public List<RefereeData> ApplicantReferees { get; set; } = new List<RefereeData>();

        public List<RefereeData> GetApplicantReferees(long applicationFormId) {
            try
            {
                List<RefereeData> refereeList = new List<RefereeData>();
                var applicantRefereeLogic = new ApplicantRefereeLogic();
                var applicantReferees = applicantRefereeLogic.GetModelsBy(ap => ap.Application_Form_Id == applicationFormId);

                if (applicantReferees?.Count() > 0)
                {
                    var applicantRefereeResponseLogic = new ApplicantRefereeResponseLogic();
                    foreach (var item in applicantReferees)
                    {
                        var applicantRefData = applicantRefereeResponseLogic.GetModelsBy(ap => ap.Applicant_Referee_Id == item.Id).FirstOrDefault();
                        var applicantRefereeData = new RefereeData() {
                            RefereeId = item.Id,
                            Department = item.Department,
                            Institution = item.Institution,
                            Rank = item.Rank,
                            RefereeName = item.Name,
                            RespondedTo = applicantRefData == null ? "No Response" : "Response Available",
                            IsRespondedTo = applicantRefData != null
                        };

                        refereeList.Add(applicantRefereeData);
                    }
                }

                return refereeList;
            }
            catch (Exception ex) { throw ex; }
        }

        public static List<RefereeFormViewModel> SetRefereeDetailsList(long formId)
        {
            try
            {
                List<RefereeFormViewModel> referenceDataList = new List<RefereeFormViewModel>();

                var applicantRefereeLogic = new ApplicantRefereeLogic();
                var applicantReferee = applicantRefereeLogic.GetModelsBy(ap => ap.Application_Form_Id == formId);
                if (applicantReferee != null)
                {
                    foreach (var item in applicantReferee)
                    {
                        RefereeFormViewModel referenceData = new RefereeFormViewModel();
                        referenceData.ApplicantRefereeId = item.Id;
                        referenceData.RefereeFullName = item.Name;
                        referenceData.DepartmentFacultyName = item.Department;
                        referenceData.InstitutionName = item.Institution;
                        referenceData.CandidateName = referenceData.GetCandidateNameByFormId(item.ApplicationForm.Id);

                        var applicantAppliedCourseLogic = new AppliedCourseLogic();
                        var applicantAppliedCourse = applicantAppliedCourseLogic.GetModelsBy(ap => ap.Application_Form_Id == item.ApplicationForm.Id)
                                                                                .Select(ap => new { ap.Programme })
                                                                                .FirstOrDefault();

                        if (applicantAppliedCourse != null)
                        {
                            referenceData.ProgrammeName = applicantAppliedCourse.Programme.Name;
                        }

                        var applicantRefereeResponseLogic = new ApplicantRefereeResponseLogic();
                        var applicantRefereeResponse = applicantRefereeResponseLogic.GetModelsBy(ap => ap.Applicant_Referee_Id == item.Id)
                                                                                    .FirstOrDefault();
                        if (applicantRefereeResponse != null)
                        {
                            referenceData.LengthOfFamiliarityTime = applicantRefereeResponse.DurationKnownApplicant;
                            referenceData.OverallPromise = applicantRefereeResponse.RefereeGradingSystem.Score;
                            referenceData.AcceptAsGraduateStudent = applicantRefereeResponse.CanAcceptApplicant;
                            //referenceData.RefereeFullName = applicantRefereeResponse.FullName;
                            referenceData.CharacterComment = applicantRefereeResponse.RelevantInformation;
                            referenceData.CandidateSuitabilty = applicantRefereeResponse.Remark;

                            //get the rating for Traits
                            var applicantRefereeGradingResponseLogic = new ApplicantRefereeGradingResponseLogic();
                            var applicantRefereeGradingResponse = applicantRefereeGradingResponseLogic.GetModelsBy(ap => ap.Applicant_Response_Id == applicantRefereeResponse.RefereeResponseId).ToList();
                            referenceData.ApplicantRefereeGradingResponses = applicantRefereeGradingResponse;

                            var referenceGradingCategoryLogic = new RefereeGradingCategoryLogic();
                            referenceData.RefereeGradingCategories = referenceGradingCategoryLogic.GetModelsBy(g => g.Active);

                            var referenceGradingSystemLogic = new RefereeGradingSystemLogic();
                            referenceData.RefereeGradingSystems = referenceGradingSystemLogic.GetModelsBy(g => g.Active && g.Type == 1);
                        }

                        referenceDataList.Add(referenceData);
                    }
                }

                return referenceDataList;
            }
            catch(Exception ex) { throw ex; }
        }

        public static RefereeFormViewModel SetRefereeDetails(long refereeId)
        {
            try
            {
                RefereeFormViewModel referenceData = new RefereeFormViewModel();

                var applicantRefereeLogic = new ApplicantRefereeLogic();
                var applicantReferee = applicantRefereeLogic.GetModelsBy(ap => ap.Applicant_Referee_Id == refereeId)
                                                            .FirstOrDefault();
                if (applicantReferee != null)
                {
                    referenceData.ApplicantRefereeId = applicantReferee.Id;
                    referenceData.RefereeFullName = applicantReferee.Name;
                    referenceData.DepartmentFacultyName = applicantReferee.Department;
                    referenceData.InstitutionName = applicantReferee.Institution;
                    referenceData.CandidateName = referenceData.GetCandidateNameByFormId(applicantReferee.ApplicationForm.Id);

                    var applicantAppliedCourseLogic = new AppliedCourseLogic();
                    var applicantAppliedCourse = applicantAppliedCourseLogic.GetModelsBy(ap => ap.Application_Form_Id == applicantReferee.ApplicationForm.Id)
                                                                            .Select(ap => new { ap.Programme })
                                                                            .FirstOrDefault();

                    if (applicantAppliedCourse != null)
                    {
                        referenceData.ProgrammeName = applicantAppliedCourse.Programme.Name;
                    }

                    var applicantRefereeResponseLogic = new ApplicantRefereeResponseLogic();
                    var applicantRefereeResponse = applicantRefereeResponseLogic.GetModelsBy(ap => ap.Applicant_Referee_Id == applicantReferee.Id)
                                                                                .FirstOrDefault();
                    if (applicantRefereeResponse != null)
                    {
                        referenceData.LengthOfFamiliarityTime = applicantRefereeResponse.DurationKnownApplicant;
                        referenceData.OverallPromise = applicantRefereeResponse.RefereeGradingSystem.Score;
                        referenceData.AcceptAsGraduateStudent = applicantRefereeResponse.CanAcceptApplicant;
                        //referenceData.RefereeFullName = applicantRefereeResponse.FullName;
                        referenceData.CharacterComment = applicantRefereeResponse.RelevantInformation;
                        referenceData.CandidateSuitabilty = applicantRefereeResponse.Remark;
                        //referenceData.CandidateEligibity = applicantRefereeResponse.e

                        //get the rating for Traits
                        var applicantRefereeGradingResponseLogic = new ApplicantRefereeGradingResponseLogic();
                        var applicantRefereeGradingResponse = applicantRefereeGradingResponseLogic.GetModelsBy(ap => ap.Applicant_Response_Id == applicantRefereeResponse.RefereeResponseId).ToList();
                        referenceData.ApplicantRefereeGradingResponses = applicantRefereeGradingResponse;

                        var referenceGradingCategoryLogic = new RefereeGradingCategoryLogic();
                        referenceData.RefereeGradingCategories = referenceGradingCategoryLogic.GetModelsBy(g => g.Active);

                        var referenceGradingSystemLogic = new RefereeGradingSystemLogic();
                        referenceData.RefereeGradingSystems = referenceGradingSystemLogic.GetModelsBy(g => g.Active && g.Type == 1);
                    }
                }

                return referenceData;
            }
            catch(Exception ex) { throw ex; }
        }

        public List<ApplicantDetail> Applicants { get; set; } = new List<ApplicantDetail>();

        public List<SelectListItem> SessionSL { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> DepartmentSL { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ProgrammeSL { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> SetProgrammeSL()
        {
            try
            {
                List<SelectListItem> ProgrammeSL = new List<SelectListItem>();
                var programmeLogic = new ProgrammeLogic();
                var programmeList = programmeLogic.GetModelsBy(s => s.Activated == true)
                                              .Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Name })
                                              .ToList();

                if (programmeList?.Count() > 0)
                {
                    ProgrammeSL.Add(new SelectListItem() { Value = "", Text = "----Select Programme----" });
                    ProgrammeSL.AddRange(programmeList);
                }
                return ProgrammeSL;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<SelectListItem> SetDepartmentSL(long programmeId)
        {
            try
            {
                List<SelectListItem> DepartmentSL = new List<SelectListItem>();
                var programmeDepartmentLogic = new ProgrammeDepartmentLogic();
                var departmentList = programmeDepartmentLogic.GetModelsBy(s => s.Programme_Id == programmeId && s.Activate == true)
                                                     .Select(s => new SelectListItem()
                                                     {
                                                         Value = s.Department.Id.ToString(),
                                                         Text = s.Department.Name
                                                     })
                                                     .ToList();

                if (departmentList?.Count() > 0)
                {
                    DepartmentSL.Add(new SelectListItem() { Value = "", Text = "----Select Department----" });
                    DepartmentSL.AddRange(departmentList);
                }
                return DepartmentSL;
            }
            catch (Exception ex) { throw ex; }
        }

        public List<SelectListItem> SetSessionSL()
        {
            try
            {
                List<SelectListItem> SessionSL = new List<SelectListItem>();
                var sessionLogic = new SessionLogic();
                var sessionList = sessionLogic.GetModelsBy(s => s.Activated == true)
                                              .Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Name })
                                              .ToList();

                if (sessionList?.Count() > 0)
                {
                    SessionSL.Add(new SelectListItem() { Value = "", Text = "----Select Session----" });
                    SessionSL.AddRange(sessionList);
                }
                return SessionSL;
            }
            catch(Exception ex) { throw ex; }
        }

        public List<ApplicantDetail> GetApplicantDetails(int departmentId, int programmeId, int sessionId)
        {
            try
            {
                List<ApplicantDetail> applicantDetails = new List<ApplicantDetail>();
                var admissionListLogic = new AdmissionListLogic();

                return admissionListLogic.GetModelsBy(ad => ad.Department_Id == departmentId && ad.Programme_Id == programmeId && ad.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == sessionId)
                                         .Select(ad => new ApplicantDetail()
                                         {
                                            ApplicantFormId = ad.Form.Id,
                                            ApplicantName = ad.Form.Person.FullName,
                                            Department = ad.Deprtment.Name,
                                            Programme = ad.Programme.Name
                                         })
                                         .ToList();
            }
            catch (Exception ex) { throw ex; }
        }

        public string GetCandidateNameByFormId(long applicationFormId)
        {
            try
            {
                var appFormLogic = new ApplicationFormLogic();
                var appForm = appFormLogic.GetModelsBy(ap => ap.Application_Form_Id == applicationFormId)
                                          .Select(ap => new { ap.Person.FullName })
                                          .FirstOrDefault();

                return appForm.FullName;
            }
            catch (Exception ex) { throw ex; }
        }
    }

    public class RefereeData
    {
        public bool IsRespondedTo { get; set; }

        public long RefereeId { get; set; }

        public string RefereeName { get; set; }

        public string Rank { get; set; }

        public string Department { get; set; }

        public string Institution { get; set; }

        public string RespondedTo { get; set; }
    }

    public class ApplicantDetail
    {
        public long ApplicantFormId { get; set; }

        public string ApplicantName { get; set; }

        public string Department { get; set; }

        public string Programme { get; set; }
    }
}