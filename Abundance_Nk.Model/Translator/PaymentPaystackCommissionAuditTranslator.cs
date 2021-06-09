using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentPaystackCommissionAuditTranslator : TranslatorBase<PaymentPaystackCommissionAudit, PAYMENT_PAYSTACK_COMMISSION_AUDIT>
    {
       private FeeTypeTranslator feeTypeTranslator;
        //private FeeTranslator feeTranslator;
        private SessionTranslator sessionTranslator;
        private UserTranslator userTranslator;
        private PaystackCommissionTranslator paystackCommissionTranslator;
        private ProgrammeTranslator programmeTranslator;

        public PaymentPaystackCommissionAuditTranslator()
        {
            feeTypeTranslator = new FeeTypeTranslator();
            //feeTranslator = new FeeTranslator();
            sessionTranslator = new SessionTranslator();
            userTranslator = new UserTranslator();
            paystackCommissionTranslator = new PaystackCommissionTranslator();
            programmeTranslator = new ProgrammeTranslator();

        }
        public override PaymentPaystackCommissionAudit TranslateToModel(PAYMENT_PAYSTACK_COMMISSION_AUDIT entity)
        {
            try
            {
                PaymentPaystackCommissionAudit model = null;
                if (entity != null)
                {
                    model = new PaymentPaystackCommissionAudit();
                    model.Id = entity.Payment_PayStack_Commission_Audit_Id;
                    model.Programme = programmeTranslator.Translate(entity.PROGRAMME);
                    model.FeeType = feeTypeTranslator.Translate(entity.FEE_TYPE);
                    model.Session = sessionTranslator.Translate(entity.SESSION);
                    model.User = userTranslator.Translate(entity.USER);
                    model.Operation = entity.Operation;
                    model.Client = entity.Client;
                    model.PaymentPaystackCommission = paystackCommissionTranslator.Translate(entity.PAYMENT_PAYSTACK_COMMISSION);
                    model.Amount = entity.Amount;
                    model.Date = entity.Date;

                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override PAYMENT_PAYSTACK_COMMISSION_AUDIT TranslateToEntity(PaymentPaystackCommissionAudit model)
        {
            try
            {
                PAYMENT_PAYSTACK_COMMISSION_AUDIT entity = null;
                if (model != null)
                {
                    entity = new PAYMENT_PAYSTACK_COMMISSION_AUDIT();
                    entity.FeeType_Id = model.FeeType.Id;
                    entity.Programme_Id = model.Programme.Id;
                    entity.Session_Id = model.Session.Id;
                    entity.Amount = model.Amount;
                    entity.Operation = model.Operation;
                    entity.Client = model.Client;
                    entity.Payment_PayStack_Commission_Id = model.PaymentPaystackCommission.Id;
                    entity.Date = DateTime.Now;
                    entity.User_Id = model.User.Id;
                    entity.Payment_PayStack_Commission_Audit_Id = model.Id;
                    entity.Action = model.Action;
                }
                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}

