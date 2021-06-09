using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentNdResultTranslator : TranslatorBase<StudentNdResult, STUDENT_ND_RESULT>
    {
        private readonly StudentTranslator studentTranslator;

        public StudentNdResultTranslator()
        {
            studentTranslator = new StudentTranslator();
        }

        public override StudentNdResult TranslateToModel(STUDENT_ND_RESULT entity)
        {
            try
            {
                StudentNdResult model = null;
                if (entity != null)
                {
                    model = new StudentNdResult();
                    model.Student = studentTranslator.Translate(entity.STUDENT);
                    model.DateAwarded = entity.Date_Awarded;

                    model.DayAwarded = new Value {Id = entity.Date_Awarded.Day};
                    model.MonthAwarded = new Value {Id = entity.Date_Awarded.Month};
                    model.YearAwarded = new Value {Id = entity.Date_Awarded.Year};
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_ND_RESULT TranslateToEntity(StudentNdResult model)
        {
            try
            {
                STUDENT_ND_RESULT entity = null;
                if (model != null)
                {
                    entity = new STUDENT_ND_RESULT();
                    entity.Person_Id = model.Student.Id;
                    entity.Date_Awarded = model.DateAwarded;
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