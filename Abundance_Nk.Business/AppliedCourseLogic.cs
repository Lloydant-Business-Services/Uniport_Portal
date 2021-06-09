using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class AppliedCourseLogic : BusinessBaseLogic<AppliedCourse, APPLICANT_APPLIED_COURSE>
    {
        private AppliedCourseAuditLogic appliedCourseAuditLogic;

        public AppliedCourseLogic()
        {
            translator = new AppliedCourseTranslator();
        }
        public AppliedCourse GetBy(ApplicationForm applicationForm)
        {
            try
            {
                Expression<Func<APPLICANT_APPLIED_COURSE, bool>> selector = s => s.Application_Form_Id == applicationForm.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

       
        public AppliedCourse GetBy(Person person)
        {
            try
            {
                Expression<Func<APPLICANT_APPLIED_COURSE, bool>> selector = s => s.Person_Id == person.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AppliedCourse> GetByAsync(Person person)
        {
            try
            {
                Expression<Func<APPLICANT_APPLIED_COURSE, bool>> selector = s => s.Person_Id == person.Id;
                return await GetModelByAsync(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool Modify(AppliedCourse appliedCourse)
        {
            try
            {
                APPLICANT_APPLIED_COURSE entity = GetEntityBy(appliedCourse.Person);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Programme_Id = appliedCourse.Programme.Id;
                entity.Department_Id = appliedCourse.Department.Id;

                if (appliedCourse.ApplicationForm != null && appliedCourse.ApplicationForm.Id > 0)
                {
                    entity.Application_Form_Id = appliedCourse.ApplicationForm.Id;
                }

                if (appliedCourse.Option != null && appliedCourse.Option.Id > 0)
                {
                    entity.Department_Option_Id = appliedCourse.Option.Id;
                }

                if(appliedCourse.Setting != null && appliedCourse.Setting.Id > 0)
                {
                    entity.Application_Form_Setting_Id = appliedCourse.Setting.Id;
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

        private APPLICANT_APPLIED_COURSE GetEntityBy(Person person)
        {
            try
            {
                Expression<Func<APPLICANT_APPLIED_COURSE, bool>> selector = s => s.Person_Id == person.Id;
                APPLICANT_APPLIED_COURSE entity = GetEntityBy(selector);

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(AppliedCourse appliedCourse, AppliedCourseAudit audit)
        {
            try
            {
                APPLICANT_APPLIED_COURSE entity = GetEntityBy(appliedCourse.Person);
                bool audited = CreateAudit(appliedCourse, audit, entity);
                if (audited)
                {
                    return Modify(appliedCourse);
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool CreateAudit(AppliedCourse appliedCourse, AppliedCourseAudit audit, APPLICANT_APPLIED_COURSE entity)
        {
            try
            {
                audit.AppliedCourse = appliedCourse;
                //audit.AppliedCourse.Person = appliedCourse.Person;
                audit.Programme = appliedCourse.Programme;
                audit.Department = appliedCourse.Department;
                audit.Option = appliedCourse.Option;
                audit.ApplicationForm = appliedCourse.ApplicationForm;

                AppliedCourse oldAppliedCourse = translator.Translate(entity);
                audit.OldProgramme = oldAppliedCourse.Programme;
                audit.OldDepartment = oldAppliedCourse.Department;
                audit.OldOption = oldAppliedCourse.Option;
                audit.OldApplicationForm = oldAppliedCourse.ApplicationForm;

                appliedCourseAuditLogic = new AppliedCourseAuditLogic();
                AppliedCourseAudit appliedCourseAudit = appliedCourseAuditLogic.Create(audit);
                if (appliedCourseAudit == null || appliedCourseAudit.Id <= 0)
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
        public bool ModifyNewSetting(AppliedCourse appliedCourse)
        {
            try
            {
                Expression<Func<APPLICANT_APPLIED_COURSE, bool>> selector = s => s.Person_Id == appliedCourse.Person.Id;
                APPLICANT_APPLIED_COURSE entity = GetEntityBy(selector);
                if (entity != null)
                {
                    entity.Application_Form_Setting_Id = appliedCourse.Setting.Id;
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