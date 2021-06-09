using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class FacultyOfficerLogic : BusinessBaseLogic<FacultyOfficer, FACULTY_OFFICER>
    {
        public FacultyOfficerLogic()
        {
            translator = new FacultyOfficerTranslator();
        }

        public FacultyOfficer GetBy(User user)
        {
            try
            {
                Expression<Func<FACULTY_OFFICER, bool>> selector = fo => fo.User_Id == user.Id;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Modify(FacultyOfficer model)
        {
            try
            {
                Expression<Func<FACULTY_OFFICER, bool>> selector = u => u.User_Id == model.Officer.Id;
                FACULTY_OFFICER entity = GetEntityBy(selector);
                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                if (model.Officer != null)
                {
                    entity.User_Id = model.Officer.Id;
                }

                if (model.Faculty != null)
                {
                    entity.Faculty_Id = model.Faculty.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}