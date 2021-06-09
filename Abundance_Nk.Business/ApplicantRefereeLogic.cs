using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class ApplicantRefereeLogic:BusinessBaseLogic<ApplicantReferee, APPLICANT_REFEREE>
    {
        public ApplicantRefereeLogic()
        {
            translator = new ApplicantRefereeTranslator();
        }

        public List<PostGraduateReportModel> GetRefereeDetails(ApplicationForm applicationForm)
        {
            try
            {
                List<PostGraduateReportModel> postGraduateReportModels = new List<PostGraduateReportModel>();
                postGraduateReportModels =
                    (from a in repository.GetBy<APPLICANT_REFEREE>(a => a.Application_Form_Id == applicationForm.Id)
                        select new PostGraduateReportModel
                        {
                            FullName = a.APPLICATION_FORM.PERSON.Last_Name + " " + a.APPLICATION_FORM.PERSON.First_Name + " " + a.APPLICATION_FORM.PERSON.Other_Name,
                            LastName = a.APPLICATION_FORM.PERSON.First_Name,
                            FirstName = a.APPLICATION_FORM.PERSON.Other_Name,
                            OtherName = a.APPLICATION_FORM.PERSON.Other_Name,
                            ApplicationNumber = a.APPLICATION_FORM.Serial_Number.ToString(),
                            Department = GetDepartment(new Person(){Id = a.APPLICATION_FORM.Person_Id}),
                            Course =  GetCourse(new Person(){Id = a.APPLICATION_FORM.Person_Id}),
                            Faculty =  GetFaculty(new Person(){Id = a.APPLICATION_FORM.Person_Id}),
                            Programme =  GetProgramme(new Person(){Id = a.APPLICATION_FORM.Person_Id}),
                            RefereeName = a.Name,
                            RefereeRank = a.Rank,
                            RefereeDepartment = a.Department,
                            RefereeInstitution = a.Institution,
                            RefereeEmail=a.Email=a.Email,
                            RefereePhone=a.Phone
                        }).ToList();

                return postGraduateReportModels;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public string GetCourse(Person person)
        {
            try
            {
                string course = "";
                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id);
                if (appliedCourse != null && appliedCourse.Option != null)
                {
                    course = appliedCourse.Option.Name.ToUpper();
                }
                return course;
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    
        public string GetDepartment(Person person)
        {
            try
            {
                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                return appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id).Department.Name.ToUpper();
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    
        public string GetFaculty(Person person)
        {
            try
            {
                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                return appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id).Department.Faculty.Name.ToUpper();
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    
        public string GetProgramme(Person person)
        {
            try
            {
                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                return appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id).Programme.Name.ToUpper();
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    
        public bool Modify(ApplicantReferee applicantReferee)
        {
            try
            {
                Expression<Func<APPLICANT_REFEREE, bool>> selector = p => p.Applicant_Referee_Id == applicantReferee.Id;
                APPLICANT_REFEREE applicantRefereeEntity = GetEntityBy(selector);

                if (applicantRefereeEntity == null || applicantRefereeEntity.Applicant_Referee_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                if (applicantReferee.Name != null)
                {
                    applicantRefereeEntity.Name = applicantReferee.Name;
                }

                if (applicantReferee.Rank != null)
                {
                    applicantRefereeEntity.Rank = applicantReferee.Rank;
                }
                if (applicantReferee.Email != null)
                {
                    applicantRefereeEntity.Email = applicantReferee.Email;
                }
                if (applicantReferee.PhoneNo != null)
                {
                    applicantRefereeEntity.Phone = applicantReferee.PhoneNo;
                }

                if (applicantReferee.Institution != null)
                {
                    applicantRefereeEntity.Institution = applicantReferee.Institution;
                }
                if (applicantReferee.Department != null)
                {
                    applicantRefereeEntity.Department = applicantReferee.Department;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
