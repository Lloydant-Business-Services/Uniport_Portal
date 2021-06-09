using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Models;
using System;

namespace Abundance_Nk.Web.Areas.Admin.ViewModels
{
    public class ApplicationFormViewModel :ApplicationFormViewModelBase
    {
        public ApplicationFormViewModel()
        {
            AppliedCourse = new AppliedCourse();
            AppliedCourse.Department = new Department();
            AppliedCourse.Programme = new Programme();
            AppliedCourse.Person = new Person();
            AppliedCourse.ApplicationForm = new ApplicationForm();
            AppliedCourse.Option = new DepartmentOption();
        }

        public AppliedCourse AppliedCourse { get; set; }

        public void SetApplicantAppliedCourse(Person person)
        {
            try
            {
                var appliedCourseLogic = new AppliedCourseLogic();
                AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id);

                if(appliedCourse != null)
                {
                    if(appliedCourse.Programme != null)
                    {
                        AppliedCourse.Programme = appliedCourse.Programme;
                    }
                    else
                    {
                        AppliedCourse.Programme = new Programme();
                    }

                    if(appliedCourse.Department != null)
                    {
                        AppliedCourse.Department = appliedCourse.Department;
                    }
                    else
                    {
                        AppliedCourse.Department = new Department();
                    }

                    if(appliedCourse.Option != null)
                    {
                        AppliedCourse.Option = appliedCourse.Option;
                    }
                    else
                    {
                        AppliedCourse.Option = new DepartmentOption();
                    }

                    if(appliedCourse.Person != null)
                    {
                        AppliedCourse.Person = appliedCourse.Person;
                    }
                    else
                    {
                        AppliedCourse.Person = new Person();
                    }
                }
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}