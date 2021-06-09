using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentInterswitchTranslator:TranslatorBase<PaymentInterswitch,PAYMENT_INTERSWITCH>
    {
        private PaymentTranslator paymentTranslator;

        public PaymentInterswitchTranslator()
        {
            paymentTranslator = new PaymentTranslator();
        }
        public override PaymentInterswitch TranslateToModel(PAYMENT_INTERSWITCH entity)
        {
            try
            {
                PaymentInterswitch model = null;
                if (entity != null)
                {
                    model = new PaymentInterswitch();
                    model.Id = entity.Payment_Interswitch_Id;
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.Amount = entity.Amount;
                    model.CardNumber = entity.Card_Number;
                    model.LeadBankCbnCode = entity.LeadBankCbnCode;
                    model.LeadBankName = entity.LeadBankName;
                    model.MerchantReference = entity.MerchantReference;
                    model.PaymentReference = entity.PaymentReference;
                    model.RetrievalReferenceNumber = entity.RetrievalReferenceNumber;
                    model.ResponseCode = entity.ResponseCode;
                    model.ResponseDescription = entity.ResponseDescription;
                    model.SplitAccounts = new string[1];
                    model.SplitAccounts[0]= entity.SplitAccounts ?? "-";
                    model.TransactionDate = entity.TransactionDate ?? DateTime.Now;


                }
                return model;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public override PAYMENT_INTERSWITCH TranslateToEntity(PaymentInterswitch model)
        {
            try
            {
                PAYMENT_INTERSWITCH entity = null;
                if (model != null)
                {
                    entity = new PAYMENT_INTERSWITCH();
                    entity.Amount = model.Amount;
                    entity.Card_Number = model.CardNumber;
                    entity.LeadBankCbnCode = model.LeadBankCbnCode;
                    entity.LeadBankName = model.LeadBankName;
                    entity.MerchantReference = model.MerchantReference;
                    entity.PaymentReference = model.PaymentReference;
                    entity.RetrievalReferenceNumber = model.RetrievalReferenceNumber;
                    entity.Payment_Id = model.Payment.Id;
                    entity.ResponseCode = model.ResponseCode;
                    entity.ResponseDescription = model.ResponseDescription;
                    entity.SplitAccounts =  "-";
                    entity.TransactionDate = model.TransactionDate;
                    entity.RetrievalReferenceNumber = model.RetrievalReferenceNumber;
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
