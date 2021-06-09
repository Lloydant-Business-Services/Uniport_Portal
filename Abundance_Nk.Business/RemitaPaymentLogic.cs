using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class RemitaPaymentLogic : BusinessBaseLogic<RemitaPayment, REMITA_PAYMENT>
    {
        public RemitaPaymentLogic()
        {
            translator = new RemitaPaymentTranslator();
        }


        public RemitaPayment GetBy(long PaymentID)
        {
            try
            {
                Expression<Func<REMITA_PAYMENT, bool>> selector = a => a.PAYMENT.Payment_Id == PaymentID;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private REMITA_PAYMENT GetEntityBy(RemitaPayment remitaPayment)
        {
            try
            {
                Expression<Func<REMITA_PAYMENT, bool>> selector = s => s.PAYMENT.Payment_Id == remitaPayment.payment.Id;
                REMITA_PAYMENT entity = GetEntityBy(selector);

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public RemitaPayment GetBy(string OrderId)
        {
            try
            {
                Expression<Func<REMITA_PAYMENT, bool>> selector = s => s.OrderId == OrderId;
                return GetModelBy(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool Modify(RemitaPayment remitaPayment)
        {
            try
            {
                REMITA_PAYMENT entity = GetEntityBy(remitaPayment);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.RRR = remitaPayment.RRR;
                entity.Status = remitaPayment.Status;

                if (remitaPayment.BankCode != null)
                {
                    entity.Bank_Code = remitaPayment.BankCode;
                }
                if (remitaPayment.MerchantCode != null)
                {
                    entity.Merchant_Code = remitaPayment.MerchantCode;
                }

                if (remitaPayment.TransactionAmount > 0)
                {
                    entity.Transaction_Amount = remitaPayment.TransactionAmount;
                }


                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteBy(long PaymentID)
        {
            try
            {
                Expression<Func<REMITA_PAYMENT, bool>> selector = a => a.PAYMENT.Payment_Id == PaymentID;
                Delete(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool ValidatePin(RemitaPayment remitaPayment, Payment payment, decimal Amount)
        {
            try
            {
                decimal accepatanceDecrease = Amount + 10000M;
                Expression<Func<REMITA_PAYMENT, bool>> selector =
                    p =>
                        p.RRR == remitaPayment.RRR  &&
                        (p.Transaction_Amount == Amount
                         || (p.Transaction_Amount == accepatanceDecrease));
                List<RemitaPayment> remitaPayments = GetModelsBy(selector);
                if (remitaPayments != null && remitaPayments.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool HasPaidFirstInstallmentFees(long StudentId, Session session, FeeType feeType)
        {
            try
            {

                Expression<Func<REMITA_PAYMENT, bool>> selector = p => (p.PAYMENT.Person_Id == StudentId && p.PAYMENT.Session_Id == session.Id
                && p.PAYMENT.Fee_Type_Id == feeType.Id && (p.PAYMENT.Payment_Mode_Id == 1 || p.PAYMENT.Payment_Mode_Id == 2)
                && (p.Status.Contains("01") || p.Description.Contains("manual")));
                RemitaPayment payment = GetModelBy(selector);
                if (payment != null && payment.RRR != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }
        public bool HasPaidCompleteFees(long StudentId, Session session, FeeType feeType)
        {
            try
            {

                Expression<Func<REMITA_PAYMENT, bool>> selector = p => (p.PAYMENT.Person_Id == StudentId && p.PAYMENT.Session_Id == session.Id && p.PAYMENT.Fee_Type_Id == feeType.Id
                && (p.PAYMENT.Payment_Mode_Id == 1 || p.PAYMENT.Payment_Mode_Id == 3) && (p.Status.Contains("01") || p.Description.Contains("manual")));
                RemitaPayment payment = GetModelBy(selector);
                if (payment != null && payment.RRR != null)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;
        }
    }
}