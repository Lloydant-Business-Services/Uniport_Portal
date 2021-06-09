using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentFinanceInformationTranslator :
        TranslatorBase<StudentFinanceInformation, STUDENT_FINANCE_INFORMATION>
    {
        private readonly ModeOfFinanceTranslator modeOfFinanceTranslator;
        private readonly StudentTranslator studentTranslator;

        public StudentFinanceInformationTranslator()
        {
            studentTranslator = new StudentTranslator();
            modeOfFinanceTranslator = new ModeOfFinanceTranslator();
        }

        public override StudentFinanceInformation TranslateToModel(STUDENT_FINANCE_INFORMATION entity)
        {
            try
            {
                StudentFinanceInformation model = null;
                if (entity != null)
                {
                    model = new StudentFinanceInformation();
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.Mode = modeOfFinanceTranslator.Translate(entity.MODE_OF_FINANCE);
                    model.ScholarshipTitle = entity.Scholarship_Title;
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_FINANCE_INFORMATION TranslateToEntity(StudentFinanceInformation model)
        {
            try
            {
                STUDENT_FINANCE_INFORMATION entity = null;
                if (model != null)
                {
                    entity = new STUDENT_FINANCE_INFORMATION();
                    entity.Person_Id = model.Student.Id;
                    entity.Mode_Of_Finance_Id = model.Mode.Id;
                    entity.Scholarship_Title = model.ScholarshipTitle;
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