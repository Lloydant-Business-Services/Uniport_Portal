using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class StaffDepartmentLogic : BusinessBaseLogic<StaffDepartment, STAFF_DEPARTMENT>
    {
        public StaffDepartmentLogic()
        {
            translator = new StaffDepartmentTranslator();
        }
        public bool Modify(StaffDepartment model)
        {
            try
            {
                Expression<Func<STAFF_DEPARTMENT, bool>> selector = f => f.Staff_Department_Id == model.Id;
                STAFF_DEPARTMENT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.IsHead = model.IsHead;
               
                entity.Date_Entered = model.DateEntered;

                if (model.Department != null)
                {
                    entity.Department_Id = model.Department.Id;
                }

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
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
