using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentMonnifyTranslator : TranslatorBase<PaymentMonnify, PAYMENT_MONNIFY>
    {

        private readonly PaymentTranslator paymentTranslator;

        public PaymentMonnifyTranslator()
        {
            paymentTranslator = new PaymentTranslator();
        }

        public override PAYMENT_MONNIFY TranslateToEntity(PaymentMonnify model)
        {
            try
            {
                PAYMENT_MONNIFY entity = null;
                if (model != null)
                {
                    entity = new PAYMENT_MONNIFY();
                    entity.Account_Bank_Code = model.BankCode;
                    entity.Account_Bank_Name = model.BankName;
                    entity.Account_Name = model.AccountName;
                    entity.Account_Number = model.AccountNumber;
                    entity.Amount = model.Amount;
                    entity.Checkout_Url = model.CheckoutUrl;
                    entity.Completed = model.Completed;
                    entity.Completed_Date = model.CompletedOn;
                    entity.Date_Created = model.DateCreated;
                    entity.Description = model.Description;
                    entity.Expiry_Date = model.ExpiryDate;
                    entity.Fee = model.Fee ?? 0;
                    entity.Invoice_Status = model.InvoiceStatus;
                    entity.Payment_Id = model.Payment.Id;
                    entity.Payment_Method = model.PaymentMethod;
                    entity.Transaction_Reference = model.TransactionReference;
                }

                return entity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override PaymentMonnify TranslateToModel(PAYMENT_MONNIFY entity)
        {
            try
            {
                PaymentMonnify model = null;
                if (entity != null)
                {
                    model = new PaymentMonnify();
                    model.TransactionReference = entity.Transaction_Reference;
                    model.PaymentMethod = entity.Payment_Method;
                    model.Payment = paymentTranslator.Translate(entity.PAYMENT);
                    model.PayableAmount = entity.Amount;
                    model.Amount = entity.Amount;
                    model.InvoiceStatus = entity.Invoice_Status;
                    model.Fee = entity.Fee ?? 0;
                    model.ExpiryDate = entity.Expiry_Date;
                    model.Description = entity.Description;
                    model.DateCreated = entity.Date_Created;
                    model.CompletedOn = entity.Completed_Date;
                    model.Completed = entity.Completed;
                    model.CheckoutUrl = entity.Checkout_Url;
                    model.BankName = entity.Account_Bank_Name;
                    model.BankCode = entity.Account_Bank_Code;
                    model.AccountNumber = entity.Account_Number;
                    model.AccountName = entity.Account_Name;
                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
