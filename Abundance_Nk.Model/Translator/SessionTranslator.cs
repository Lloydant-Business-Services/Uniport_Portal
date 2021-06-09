using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class SessionTranslator : TranslatorBase<Session, SESSION>
    {
        public override Session TranslateToModel(SESSION sessionEntity)
        {
            try
            {
                Session session = null;
                if (sessionEntity != null)
                {
                    session = new Session();
                    session.Id = sessionEntity.Session_Id;
                    session.Name = sessionEntity.Session_Name;
                    session.StartDate = sessionEntity.Start_Date;
                    session.EndDate = sessionEntity.End_date;
                    session.Activated = sessionEntity.Activated;
                    session.ActiveApplication = sessionEntity.Active_Application;
                    session.ActiveCourseRegistration = sessionEntity.Active_Course_Registration;
                    session.ActiveHostelApplication = sessionEntity.Active_Hostel_Application;
                }

                return session;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SESSION TranslateToEntity(Session session)
        {
            try
            {
                SESSION sessionEntity = null;
                if (session != null)
                {
                    sessionEntity = new SESSION();
                    sessionEntity.Session_Name = session.Name;
                    sessionEntity.Start_Date = session.StartDate;
                    sessionEntity.End_date = session.EndDate;
                    sessionEntity.Activated = session.Activated;
                    sessionEntity.Active_Application = session.ActiveApplication;
                    sessionEntity.Active_Course_Registration = session.ActiveCourseRegistration;
                    sessionEntity.Active_Hostel_Application = session.ActiveHostelApplication;
                }

                return sessionEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}