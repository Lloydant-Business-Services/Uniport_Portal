using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class DeliveryServiceZoneTranslator : TranslatorBase<DeliveryServiceZone, DELIVERY_SERVICE_ZONE>
    {
        private DeliveryServiceTranslator deliveryServiceTranslator;
        private GeoZoneTranslator geoZoneTranslator;
        private FeeTypeTranslator feeTypeTranslator;
        private CountryTranslator countryTranslator;

        public DeliveryServiceZoneTranslator()
        {
            deliveryServiceTranslator = new DeliveryServiceTranslator();
            geoZoneTranslator = new GeoZoneTranslator();
            feeTypeTranslator = new FeeTypeTranslator();
            countryTranslator = new CountryTranslator();

        }

        public override DeliveryServiceZone TranslateToModel(DELIVERY_SERVICE_ZONE entity)
        {
            try
            {
                DeliveryServiceZone model = null;
                if (entity != null)
                {
                    model = new DeliveryServiceZone();
                    model.Id = entity.Delivery_Service_Zone_Id;
                    model.Activated = entity.Activated;
                    model.Country = countryTranslator.TranslateToModel(entity.COUNTRY);
                    model.DeliveryService = deliveryServiceTranslator.Translate(entity.DELIVERY_SERVICE);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.GeoZone = geoZoneTranslator.Translate(entity.GEO_ZONE);
                    model.DeliveryServiceAmount = entity.Delivery_Service_Amount;
                    model.LLoydantAmount = entity.LLoydant_Amount;
                    model.SchoolAmount = entity.School_Amount;

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override DELIVERY_SERVICE_ZONE TranslateToEntity(DeliveryServiceZone model)
        {
            try
            {
                DELIVERY_SERVICE_ZONE entity = null;
                if (model != null)
                {
                    entity = new DELIVERY_SERVICE_ZONE();
                    entity.Activated = model.Activated;
                    entity.Country_Id = model.Country.Id;
                    entity.Delivery_Service_Zone_Id = model.Id;
                    entity.Delivery_Service_Id = model.DeliveryService.Id;
                    entity.Fee_Type_Id = model.FeeType.Id;
                    entity.Geo_Zone_Id = model.GeoZone.Id;
                    entity.Delivery_Service_Amount = model.DeliveryServiceAmount;
                    entity.LLoydant_Amount = model.LLoydantAmount;
                    entity.School_Amount = model.SchoolAmount;

                }

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
