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
    public class StaffLogic : BusinessBaseLogic<Staff, STAFF>
    {
        public StaffLogic()
        {
            translator = new StaffTranslator();
        }

        public Staff GetBy(long UserId)
        {
            Staff staff = null;
            try
            {
                staff = GetModelsBy(a => a.User_Id == UserId).FirstOrDefault();
            }
            catch (Exception)
            {
                
                throw;
            }
            return staff;
        }

        public bool Modify(Staff model)
        {
            try
            {
                Expression<Func<STAFF, bool>> selector = f => f.Staff_Id == model.Id;
                STAFF entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Is_Management = model.isManagement;
                entity.Profile_Description = model.ProfileDescription;

                if (model.Type != null)
                {
                    entity.Staff_Type_Id = model.Type.Id;
                }
              
                
                if (model.Genotype != null)
                {
                    entity.Genotype_Id = model.Genotype.Id;
                }
                if (model.BloodGroup != null)
                {
                    entity.Blood_Group_Id = model.BloodGroup.Id;
                }
                
                if(model.Person != null && model.Person.Id > 0)
                {
                    entity.Person_Id = model.Person.Id;
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