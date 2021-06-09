using System;
using System.Collections.Generic;
using System.Linq;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class CurrentSessionSemesterLogic : BusinessBaseLogic<CurrentSessionSemester, CURRENT_SESSION_SEMESTER>
    {
        public CurrentSessionSemesterLogic()
        {
            base.translator = new CurrentSessionSemesterTranslator();
        }

        public override CurrentSessionSemester Add(CurrentSessionSemester currentSessionSemester)
        {
            try
            {
                Func<CURRENT_SESSION_SEMESTER, bool> selector =
                    css => css.Session_Semester_Id == GetAll()[0].SessionSemester.Id;

                Remove(selector);
                return Add(currentSessionSemester);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public CurrentSessionSemester GetCurrentSessionSemester()
        {
            try
            {
                List<CurrentSessionSemester> currentSessionTerms = GetAll();
                if (currentSessionTerms.Count > 0)
                {
                    return currentSessionTerms.LastOrDefault();
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
         public CurrentSessionSemester GetCurrentSessionSemesterById(int id)
        {
            try
            {
                List<CurrentSessionSemester> currentSessionTerms = GetModelsBy(a => a.Session_Semester_Id == id);
                if (currentSessionTerms.Count > 0)
                {
                    return currentSessionTerms.LastOrDefault();
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public CurrentSessionSemester GetCurrentSessionTerm()
        {
            try
            {
                List<CurrentSessionSemester> currentSessionTerms = GetAll();
                if (currentSessionTerms.Count > 0)
                {
                    return currentSessionTerms.LastOrDefault();
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Remove(Func<CURRENT_SESSION_SEMESTER, bool> selector)
        {
            try
            {
                repository.Delete(selector);
                return Save() > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}