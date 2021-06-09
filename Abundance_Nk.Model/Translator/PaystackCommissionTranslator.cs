using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class PaystackCommissionTranslator : TranslatorBase<PaymentPaystackCommission,PAYMENT_PAYSTACK_COMMISSION>
    {
        private FeeTypeTranslator feeTypeTranslator;
        private FeeTranslator feeTranslator;
        private SessionTranslator sessionTranslator;
        private ProgrammeTranslator programmeTranslator;
        private UserTranslator userTranslator;

        public PaystackCommissionTranslator()
        {
            feeTypeTranslator = new FeeTypeTranslator();
            feeTranslator = new FeeTranslator();
            sessionTranslator = new SessionTranslator();
            programmeTranslator = new ProgrammeTranslator();
            userTranslator = new UserTranslator();


        }
        public override PaymentPaystackCommission TranslateToModel(PAYMENT_PAYSTACK_COMMISSION entity)
        {
            try
            {
                PaymentPaystackCommission model = null;
                if (entity != null)
                {
                    model = new PaymentPaystackCommission();
                    model.Id = entity.Payment_PayStack_Commission_Id;
                    //model.Fee = feeTranslator.Translate(entity.FEE);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.Amount = entity.Amount;
                    model.Activated = (bool) entity.Activated;
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.AddOnFee = entity.AddedFee;
                    model.User = userTranslator.Translate(entity.USER);

                }
                return model;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public override PAYMENT_PAYSTACK_COMMISSION TranslateToEntity(PaymentPaystackCommission model)
        {
            try
            {
                PAYMENT_PAYSTACK_COMMISSION entity = null;
                if (model != null)
                {
                    entity = new PAYMENT_PAYSTACK_COMMISSION();
                    entity.FeeType_Id = model.FeeType.Id;
                    //entity.Fee_Id = model.Fee.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Activated = model.Activated;
                    entity.Amount = model.Amount;
                    entity.Programme_Id = model.Programme.Id;
                    entity.AddedFee = model.AddOnFee;
                    entity.SetUpBy = model.User.Id;
                }
                return entity;
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }
    }
}
