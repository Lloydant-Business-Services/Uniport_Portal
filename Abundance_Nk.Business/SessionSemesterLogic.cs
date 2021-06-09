using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class SessionSemesterLogic : BusinessBaseLogic<SessionSemester, SESSION_SEMESTER>
    {
        private CurrentSessionSemesterLogic currentSessionSemesterLogic;

        public SessionSemesterLogic()
        {
            translator = new SessionSemesterTranslator();
            currentSessionSemesterLogic = new CurrentSessionSemesterLogic();
        }

        public SessionSemester GetBy(int id)
        {
            try
            {
                Expression<Func<SESSION_SEMESTER, bool>> selector = s => s.Session_Semester_Id == id;
                return base.GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<SessionSemester> GetByAsync(int id)
        {
            try
            {
                Expression<Func<SESSION_SEMESTER, bool>> selector = s => s.Session_Semester_Id == id;
                return await base.GetModelByAsync(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public SessionSemester GetBySessionSemester(int semesterId, int sessionId)
        {
            try
            {
                Expression<Func<SESSION_SEMESTER, bool>> selector = s => s.Session_Id == sessionId && s.Semester_Id == semesterId;
                return base.GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

      
        public bool Modify(SessionSemester sessionSemester)
        {
            try
            {
                Expression<Func<SESSION_SEMESTER, bool>> selector = s => s.Session_Semester_Id == sessionSemester.Id;
                SESSION_SEMESTER entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Session_Id = sessionSemester.Session.Id;
                entity.Semester_Id = sessionSemester.Semester.Id;
                entity.Start_Date = sessionSemester.StartDate;
                entity.End_Date = sessionSemester.EndDate;

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
    }
}