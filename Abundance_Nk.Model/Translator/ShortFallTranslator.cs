using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class ShortFallTranslator : TranslatorBase<ShortFall, SHORT_FALL>
    {
        private readonly PaymentTranslator paymentTranslator;

        public ShortFallTranslator()
        {
            paymentTranslator = new PaymentTranslator();
        }

        public override ShortFall TranslateToModel(SHORT_FALL entity)
        {
            try
            {
                ShortFall model = null;
                if (entity != null)
                {
                    model = new ShortFall();
                    model.Amount = entity.Amount;
                    model.Id = entity.Short_Fall_Id;
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.Description = entity.Description;
                }
                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override SHORT_FALL TranslateToEntity(ShortFall model)
        {
            try
            {
                SHORT_FALL entity = null;
                if (model != null)
                {
                    entity = new SHORT_FALL();
                    entity.Short_Fall_Id = model.Id;
                    entity.Amount = model.Amount;
                    entity.Payment_Id = model.Payment.Id;
                    entity.Description = model.Description;
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