using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class DeliveryServiceTranslator : TranslatorBase<DeliveryService, DELIVERY_SERVICE>
    {
        public override DeliveryService TranslateToModel(DELIVERY_SERVICE entity)
        {
            try
            {
                DeliveryService model = null;
                if (entity != null)
                {
                    model = new DeliveryService();
                    model.Id = entity.Delivery_Service_Id;
                    model.Activated = entity.Activated;
                    model.DeliveryServiceAccount = entity.Delivery_Service_Account;
                    model.BankCode = entity.Bank_Code;
                    model.Description = entity.Description;
                    model.Name = entity.Name;

                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override DELIVERY_SERVICE TranslateToEntity(DeliveryService model)
        {
            try
            {
                DELIVERY_SERVICE entity = null;
                if (model != null)
                {
                    entity = new DELIVERY_SERVICE();
                    entity.Activated = model.Activated;
                    entity.Description = model.Description;
                    entity.Delivery_Service_Id = model.Id;
                    entity.Bank_Code = model.BankCode;
                    entity.Delivery_Service_Account = model.DeliveryServiceAccount;
                    entity.Name = model.Name;
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
