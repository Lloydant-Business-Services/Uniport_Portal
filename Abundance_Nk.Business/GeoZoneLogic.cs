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
    public class GeoZoneLogic : BusinessBaseLogic<GeoZone,GEO_ZONE>
    {
        public GeoZoneLogic()
        {
            translator = new GeoZoneTranslator();
        }

        public bool Modify(GeoZone model)
        {
            try
            {
                Expression<Func<GEO_ZONE, bool>> selector = a => a.Geo_Zone_Id == model.Id;
                GEO_ZONE entity = GetEntityBy(selector);

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
