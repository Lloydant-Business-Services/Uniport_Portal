using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class AppliedCourseTranslator : TranslatorBase<AppliedCourse, APPLICANT_APPLIED_COURSE>
    {
        private readonly ApplicationFormTranslator applicationFormTranslator;
        private readonly DepartmentTranslator departmentTranslator;
        private readonly DepartmentOptionTranslator departmentOptionTranslator;
        private readonly PersonTranslator personTranslator;
        private readonly ProgrammeTranslator programmeTranslator;
        private readonly ApplicationFormSettingTranslator applicationFormSettingTranslator;

        public AppliedCourseTranslator()
        {
            personTranslator = new PersonTranslator();
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            departmentOptionTranslator = new DepartmentOptionTranslator();
            applicationFormSettingTranslator = new ApplicationFormSettingTranslator();
        }

        public override AppliedCourse TranslateToModel(APPLICANT_APPLIED_COURSE entity)
        {
            try
            {
                AppliedCourse appliedCourse = null;
                if (entity != null)
                {
                    appliedCourse = new AppliedCourse();
                    appliedCourse.Person = personTranslator.Translate(entity.PERSON);
                    appliedCourse.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    appliedCourse.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    appliedCourse.Option = departmentOptionTranslator.Translate(entity.DEPARTMENT_OPTION);
                    appliedCourse.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    appliedCourse.Setting = applicationFormSettingTranslator.Translate(entity.APPLICATION_FORM_SETTING);
                }

                return appliedCourse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_APPLIED_COURSE TranslateToEntity(AppliedCourse appliedCourse)
        {
            try
            {
                APPLICANT_APPLIED_COURSE entity = null;
                if (appliedCourse != null)
                {
                    entity = new APPLICANT_APPLIED_COURSE();
                    entity.Person_Id = appliedCourse.Person.Id;
                    entity.Programme_Id = appliedCourse.Programme.Id;
                    entity.Department_Id = appliedCourse.Department.Id;
                    if (appliedCourse.Option != null)
                    {
                        entity.Department_Option_Id = appliedCourse.Option.Id;
                    }
                    if (appliedCourse.ApplicationForm != null)
                    {
                        entity.Application_Form_Id = appliedCourse.ApplicationForm.Id;
                    }
                    if (appliedCourse.Setting != null)
                    {
                        entity.Application_Form_Setting_Id = appliedCourse.Setting.Id;
                    }
                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}