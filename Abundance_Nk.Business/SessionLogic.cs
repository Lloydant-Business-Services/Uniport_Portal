using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class SessionLogic : BusinessBaseLogic<Session, SESSION>
    {
        public SessionLogic()
        {
            translator = new SessionTranslator();
        }

        public bool Modify(Session session)
        {
            try
            {
                Expression<Func<SESSION, bool>> selector = s => s.Session_Id == session.Id;
                SESSION entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Session_Name = session.Name;
                entity.Start_Date = session.StartDate;
                entity.End_date = session.EndDate;
                entity.Active_Application = session.ActiveApplication;
                entity.Active_Course_Registration = session.ActiveCourseRegistration;
                //entity.Active_Hostel_Application = session.ActiveHostelApplication;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Session> GetActiveSessions()
        {
            var sessions = new List<Session>();
            try
            {
                return GetModelsBy(a => a.Activated == true).OrderByDescending(k => k.Name).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}