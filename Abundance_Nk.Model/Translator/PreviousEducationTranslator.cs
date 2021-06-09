using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class PreviousEducationTranslator : TranslatorBase<PreviousEducation, APPLICANT_PREVIOUS_EDUCATION>
    {
        private readonly ApplicationFormTranslator applicationFormTranslator;
        private readonly EducationalQualificationTranslator educationalQualificationTranslator;
        private readonly ITDurationTranslator iTDurationTranslator;
        private readonly PersonTranslator personTranslator;
        private readonly ResultGradeTranslator resultGradeTranslator;
        private PersonTypeTranslator personTypeTranslator;

        public PreviousEducationTranslator()
        {
            personTranslator = new PersonTranslator();
            personTypeTranslator = new PersonTypeTranslator();
            resultGradeTranslator = new ResultGradeTranslator();
            educationalQualificationTranslator = new EducationalQualificationTranslator();
            applicationFormTranslator = new ApplicationFormTranslator();
            iTDurationTranslator = new ITDurationTranslator();
        }

        public override PreviousEducation TranslateToModel(APPLICANT_PREVIOUS_EDUCATION entity)
        {
            try
            {
                PreviousEducation previousEducation = null;
                if (entity != null)
                {
                    previousEducation = new PreviousEducation();
                    previousEducation.Id = entity.Applicant_Previous_Education_Id;
                    previousEducation.Person = personTranslator.Translate(entity.PERSON);
                    previousEducation.SchoolName = entity.Previous_School_Name;
                    previousEducation.Course = entity.Previous_Course;
                    previousEducation.StartDate = entity.Previous_Education_Start_Date;
                    previousEducation.EndDate = entity.Previous_Education_End_Date;
                    previousEducation.Qualification =
                        educationalQualificationTranslator.Translate(entity.EDUCATIONAL_QUALIFICATION);
                    previousEducation.ResultGrade = resultGradeTranslator.Translate(entity.RESULT_GRADE);
                    previousEducation.ITDuration = iTDurationTranslator.Translate(entity.IT_DURATION);
                    previousEducation.ApplicationForm = applicationFormTranslator.Translate(entity.APPLICATION_FORM);
                    previousEducation.MatricNumber = entity.Previous_Matric_No;
                    if (entity.Previous_Education_Start_Date != null)
                    {
                        previousEducation.StartDay = new Value {Id = entity.Previous_Education_Start_Date.Day};
                        previousEducation.StartMonth = new Value {Id = entity.Previous_Education_Start_Date.Month};
                        previousEducation.StartYear = new Value {Id = entity.Previous_Education_Start_Date.Year};
                    }

                    if (entity.Previous_Education_End_Date != null)
                    {
                        previousEducation.EndDay = new Value {Id = entity.Previous_Education_End_Date.Day};
                        previousEducation.EndMonth = new Value {Id = entity.Previous_Education_End_Date.Month};
                        previousEducation.EndYear = new Value {Id = entity.Previous_Education_End_Date.Year};
                    }
                }

                return previousEducation;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override APPLICANT_PREVIOUS_EDUCATION TranslateToEntity(PreviousEducation previousEducation)
        {
            try
            {
                APPLICANT_PREVIOUS_EDUCATION entity = null;
                if (previousEducation != null)
                {
                    entity = new APPLICANT_PREVIOUS_EDUCATION();
                    entity.Applicant_Previous_Education_Id = previousEducation.Id;
                    entity.Person_Id = previousEducation.Person.Id;
                    entity.Previous_School_Name = previousEducation.SchoolName;
                    entity.Previous_Course = previousEducation.Course;
                    entity.Previous_Education_Start_Date = previousEducation.StartDate;
                    entity.Previous_Education_End_Date = previousEducation.EndDate;
                    entity.Result_Grade_Id = previousEducation.ResultGrade.Id;
                    entity.Previous_Matric_No = previousEducation.MatricNumber;
                    if (previousEducation.Qualification != null && previousEducation.Qualification.Id > 0)
                    {
                        entity.Educational_Qualification_Id = previousEducation.Qualification.Id;
                    }

                    if (previousEducation.ITDuration != null && previousEducation.ITDuration.Id > 0)
                    {
                        entity.IT_Duration_Id = previousEducation.ITDuration.Id;
                    }

                    if (previousEducation.ApplicationForm != null && previousEducation.ApplicationForm.Id > 0)
                    {
                        entity.Application_Form_Id = previousEducation.ApplicationForm.Id;
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