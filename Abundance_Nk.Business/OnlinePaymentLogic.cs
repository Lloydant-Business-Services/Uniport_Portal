using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class OnlinePaymentLogic : BusinessBaseLogic<OnlinePayment, ONLINE_PAYMENT>
    {
        public OnlinePaymentLogic()
        {
            translator = new OnlinePaymentTranslator();
        }

        public bool UpdateTransactionNumber(Payment payment, string confirmationOrderNumber)
        {
            try
            {
                Expression<Func<ONLINE_PAYMENT, bool>> selector = o => o.Payment_Id == payment.Id;
                ONLINE_PAYMENT entity = GetEntityBy(selector);

                if (entity == null)
                {
                    return false;
                }

                entity.Transaction_Number = confirmationOrderNumber;
                entity.Transaction_Date = DateTime.Now;

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

        public bool ConfirmationOrderNumberAlreadyExist(Payment payment)
        {
            try
            {
                Expression<Func<ONLINE_PAYMENT, bool>> selector = o => o.Payment_Id == payment.Id;
                OnlinePayment onlinePayment = GetModelBy(selector);
                if (onlinePayment != null)
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

        public OnlinePayment GetBy(long PaymentID)
        {
            try
            {
                Expression<Func<ONLINE_PAYMENT, bool>> selector = a => a.PAYMENT.Payment_Id == PaymentID;
                return GetModelBy(selector);
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
                Expression<Func<ONLINE_PAYMENT, bool>> selector = a => a.PAYMENT.Payment_Id == PaymentID;
                Delete(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}