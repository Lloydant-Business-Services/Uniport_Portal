using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class SupplementaryCourseTranslator : TranslatorBase<SupplementaryCourse, APPLICANT_SUPPLEMENTARY_COURSE>
    {
        private readonly ApplicationFormTranslator applicationFormTranslator;
        private readonly DepartmentTranslator departmentTranslator;
        private readonly PersonTranslator personTranslator;
        private ProgrammeTranslator programmeTranslator;

        public SupplementaryCourseTranslator()
        {
            personTranslator = new PersonTranslator();
            departmentTranslator = new DepartmentTranslator();
            programmeTranslator = new ProgrammeTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
        }

        public override SupplementaryCourse TranslateToModel(APPLICANT_SUPPLEMENTARY_COURSE entity)
        {
            try
            {
                SupplementaryCourse supplementaryCourse = null;
                if (entity != null)
                {
                    supplementaryCourse = new SupplementaryCourse();
                    supplementaryCourse.Person = personTranslator.Translate(entity.PERSON);
                    supplementaryCourse.Department = departmentTranslator.Translate(entity.DEPARTMENT);
                    supplementaryCourse.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    supplementaryCourse.Putme_Score = entity.PUTME_Score ?? 0;
                    supplementaryCourse.Average_Score = entity.Avergae_Score ?? 0;
                }

                return supplementaryCourse;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_SUPPLEMENTARY_COURSE TranslateToEntity(SupplementaryCourse supplementaryCourse)
        {
            try
            {
                APPLICANT_SUPPLEMENTARY_COURSE entity = null;
                if (supplementaryCourse != null)
                {
                    entity = new APPLICANT_SUPPLEMENTARY_COURSE();
                    entity.Person_Id = supplementaryCourse.Person.Id;
                    entity.Department_Id = supplementaryCourse.Department.Id;
                    entity.Avergae_Score = supplementaryCourse.Average_Score;
                    entity.PUTME_Score = supplementaryCourse.Putme_Score;

                    if (supplementaryCourse.ApplicationForm != null)
                    {
                        entity.Application_From_Id = supplementaryCourse.ApplicationForm.Id;
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