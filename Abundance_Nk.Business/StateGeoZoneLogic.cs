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
    public class StateGeoZoneLogic : BusinessBaseLogic<StateGeoZone, STATE_GEO_ZONE>
    {
        public StateGeoZoneLogic()
        {
            translator = new StateGeoZoneTranslator();
        }

        public bool Modify(StateGeoZone model)
        {
            try
            {
                Expression<Func<STATE_GEO_ZONE, bool>> selector = a => a.State_Geo_Zone_Id == model.Id;
                STATE_GEO_ZONE entity = GetEntityBy(selector);

                entity.State_Id = model.State.Id;
                entity.Geo_Zone_Id = model.GeoZone.Id;
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
