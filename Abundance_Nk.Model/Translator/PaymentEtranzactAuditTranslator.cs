using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abundance_Nk.Model.Translator
{
    public class PaymentEtranzactAuditTranslator : TranslatorBase<PaymentEtranzactAudit, PAYMENT_ETRANZACT_AUDIT>
    {
        private readonly OnlinePaymentTranslator onlinePaymentTranslator;
        private readonly PaymentEtranzactTypeTranslator paymentEtranzactTypeTranslator;
        private readonly PaymentTerminalTranslator paymentTerminalTranslator;
        private readonly PaymentTranslator paymentTranslator;

        public PaymentEtranzactAuditTranslator()
        {
            onlinePaymentTranslator = new OnlinePaymentTranslator();
            paymentTranslator = new PaymentTranslator();
            paymentTerminalTranslator = new PaymentTerminalTranslator();
            paymentEtranzactTypeTranslator = new PaymentEtranzactTypeTranslator();
        }

        public override PaymentEtranzactAudit TranslateToModel(PAYMENT_ETRANZACT_AUDIT entity)
        {
            try
            {
                PaymentEtranzactAudit model = null;
                if (entity != null)
                {
                    model = new PaymentEtranzactAudit();
                    model.Payment = new OnlinePayment();
                    model.Payment.Payment = new Payment();
                    model.Payment.Payment.Id = entity.Payment_Id;
                    model.Terminal = new PaymentTerminal();
                    model.Terminal.Id = entity.Payment_Terminal_Id;
                    model.EtranzactType = new PaymentEtranzactType();
                    model.EtranzactType.Id = entity.Payment_Etranzact_Type_Id;
                    model.BankCode = entity.Bank_Code;
                    model.BranchCode = entity.Branch_Code;
                    model.ConfirmationNo = entity.Confirmation_No;
                    model.CustomerAddress = entity.Customer_Address;
                    model.CustomerID = entity.Customer_Id;
                    model.CustomerName = entity.Customer_Name;
                    model.MerchantCode = entity.Merchant_Code;
                    model.PaymentCode = entity.Payment_Code;
                    model.ReceiptNo = entity.Receipt_No;
                    model.TransactionAmount = entity.Transaction_Amount;
                    model.TransactionDate = entity.Transaction_Date;
                    model.TransactionDescription = entity.Transaction_Description;
                    model.Used = Convert.ToBoolean(entity.Used);
                    model.Client = entity.Client;
                    model.Operation = entity.Operation;
                    model.Action = entity.Action;
                    model.Time = entity.Time;
                    model.User = new User();
                    model.User.Id = entity.User_Id;
                    //model.UsedBy = Convert.ToInt64(entity.USED_BY_PERSON);
                    //model.SessionId = Convert.ToInt16(entity.SESSION_ID);
                }

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override PAYMENT_ETRANZACT_AUDIT TranslateToEntity(PaymentEtranzactAudit model)
        {
            try
            {
                PAYMENT_ETRANZACT_AUDIT entity = null;
                if (model != null)
                {
                    entity = new PAYMENT_ETRANZACT_AUDIT();
                    entity.Payment_Id = model.Payment.Payment.Id;
                    entity.Payment_Terminal_Id = model.Terminal.Id;
                    entity.Payment_Etranzact_Type_Id = model.EtranzactType.Id;
                    entity.Bank_Code = model.BankCode;
                    entity.Branch_Code = model.BranchCode;
                    entity.Confirmation_No = model.ConfirmationNo;
                    entity.Customer_Address = model.CustomerAddress;
                    entity.Customer_Id = model.CustomerID;
                    entity.Customer_Name = model.CustomerName;
                    entity.Merchant_Code = model.MerchantCode;
                    entity.Payment_Code = model.PaymentCode;
                    entity.Receipt_No = model.ReceiptNo;
                    entity.Transaction_Amount = model.TransactionAmount;
                    entity.Transaction_Date = model.TransactionDate;
                    entity.Transaction_Description = model.TransactionDescription;
                    entity.Used = model.Used;
                    entity.Action = model.Action;
                    entity.Client = model.Client;
                    entity.Operation = model.Operation;
                    entity.Time = model.Time;
                    entity.User_Id = model.User.Id;
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
