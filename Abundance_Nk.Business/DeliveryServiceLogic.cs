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
    public class DeliveryServiceLogic:BusinessBaseLogic<DeliveryService,DELIVERY_SERVICE>
    {
        public DeliveryServiceLogic()
        {
            translator = new DeliveryServiceTranslator();
        }

        public bool Modify(DeliveryService model)
        {
            try
            {
                Expression<Func<DELIVERY_SERVICE, bool>> selector = a => a.Delivery_Service_Id == model.Id;
                DELIVERY_SERVICE entity = GetEntityBy(selector);

                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.Activated = model.Activated;

                int modifiedCount = Save();
                if (modifiedCount > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return false;
        }
    }
}
