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
    public class DeliveryServiceZoneLogic:BusinessBaseLogic<DeliveryServiceZone,DELIVERY_SERVICE_ZONE>
    {
        public DeliveryServiceZoneLogic()
        {
            translator = new DeliveryServiceZoneTranslator();
        }

        public bool Modify(DeliveryServiceZone model)
        {
            try
            {
                Expression<Func<DELIVERY_SERVICE_ZONE, bool>> selector = a => a.Delivery_Service_Zone_Id == model.Id;
                DELIVERY_SERVICE_ZONE entity = GetEntityBy(selector);

                
                if (model.DeliveryService != null)
                {
                    entity.Delivery_Service_Id = model.DeliveryService.Id;
                }
                if (model.GeoZone != null)
                {
                    entity.Geo_Zone_Id = model.GeoZone.Id;
                }
                if (model.FeeType != null)
                {
                    entity.Fee_Type_Id = model.FeeType.Id;
                }
                if (model.DeliveryServiceAmount != null)
                {
                    entity.Delivery_Service_Amount = model.DeliveryServiceAmount;
                }
                if (model.LLoydantAmount != null)
                {
                    entity.LLoydant_Amount = model.LLoydantAmount;
                }
                if (model.SchoolAmount != null)
                {
                    entity.School_Amount = model.SchoolAmount;
                }
               
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
