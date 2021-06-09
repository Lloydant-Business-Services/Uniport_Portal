using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentAcademicInformationTranslator :
        TranslatorBase<StudentAcademicInformation, STUDENT_ACADEMIC_INFORMATION>
    {
        private readonly LevelTranslator levelTranslator;
        private readonly ModeOfEntryTranslator modeOfEntryTranslator;
        private readonly ModeOfStudyTranslator modeOfStudyTranslator;
        private readonly StudentTranslator studentTranslator;

        public StudentAcademicInformationTranslator()
        {
            studentTranslator = new StudentTranslator();
            modeOfEntryTranslator = new ModeOfEntryTranslator();
            modeOfStudyTranslator = new ModeOfStudyTranslator();
            levelTranslator = new LevelTranslator();
        }

        public override StudentAcademicInformation TranslateToModel(STUDENT_ACADEMIC_INFORMATION entity)
        {
            try
            {
                StudentAcademicInformation model = null;
                if (entity != null)
                {
                    model = new StudentAcademicInformation();
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.ModeOfEntry = modeOfEntryTranslator.Translate(entity.MODE_OF_ENTRY);
                    model.ModeOfStudy = modeOfStudyTranslator.TranslateToModel(entity.MODE_OF_STUDY);
                    model.YearOfAdmission = entity.Year_Of_Admission;
                    model.YearOfGraduation = entity.Year_Of_Graduation;
                    model.Level = levelTranslator.Translate(entity.LEVEL);
                    model.DateEntered = entity.Date_Entered;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_ACADEMIC_INFORMATION TranslateToEntity(StudentAcademicInformation model)
        {
            try
            {
                STUDENT_ACADEMIC_INFORMATION entity = null;
                if (model != null)
                {
                    entity = new STUDENT_ACADEMIC_INFORMATION();
                    entity.Person_Id = model.Student.Id;
                    entity.Mode_Of_Entry_Id = model.ModeOfEntry.Id;
                    entity.Mode_Of_Study_Id = model.ModeOfStudy.Id;
                    entity.Year_Of_Admission = model.YearOfAdmission;
                    entity.Year_Of_Graduation = model.YearOfGraduation;
                    entity.Level_Id = model.Level.Id;
                    entity.Date_Entered = model.DateEntered;
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