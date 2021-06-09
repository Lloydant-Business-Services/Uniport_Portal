using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentExtraYearSessionTranslator : TranslatorBase<StudentExtraYearSession, STUDENT_EXTRA_YEAR_SESSION>
    {
        private readonly PersonTranslator personTranslator;
        private readonly SessionTranslator sessionTranslator;

        public StudentExtraYearSessionTranslator()
        {
            personTranslator = new PersonTranslator();
            sessionTranslator = new SessionTranslator();
        }

        public override StudentExtraYearSession TranslateToModel(STUDENT_EXTRA_YEAR_SESSION entity)
        {
            try
            {
                StudentExtraYearSession model = null;
                if (entity != null)
                {
                    model = new StudentExtraYearSession();
                    model.Person = personTranslator.Translate(entity.PERSON);
                    model.Id = entity.Student_Extra_Year_Session_Id;
                    model.Session = sessionTranslator.Translate(entity.SESSION1);
                    model.Sessions_Registered = entity.Sessions_Registered;
                    model.LastSessionRegistered = sessionTranslator.Translate(entity.SESSION2);
                    model.DeferementCommencedSession = sessionTranslator.Translate(entity.SESSION);
                }
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_EXTRA_YEAR_SESSION TranslateToEntity(StudentExtraYearSession model)
        {
            STUDENT_EXTRA_YEAR_SESSION entity = null;
            try
            {
                if (model != null)
                {
                    entity = new STUDENT_EXTRA_YEAR_SESSION();
                    entity.Student_Extra_Year_Session_Id = model.Id;
                    if (model.DeferementCommencedSession != null)
                    {
                        entity.Differement_Commenced_Session = model.DeferementCommencedSession.Id;
                    }
                    entity.Last_Session_Registered = model.LastSessionRegistered.Id;
                    entity.Person_Id = model.Person.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Sessions_Registered = model.Sessions_Registered;
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