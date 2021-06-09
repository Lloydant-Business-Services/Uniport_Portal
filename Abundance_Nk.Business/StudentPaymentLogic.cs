using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;

namespace Abundance_Nk.Business
{
    public class StudentPaymentLogic : BusinessBaseLogic<StudentPayment, STUDENT_PAYMENT>
    {
        private PaymentLogic paymentLogic;

        public StudentPaymentLogic()
        {
            base.translator = new StudentPaymentTranslator();
            paymentLogic = new PaymentLogic();
        }

        public StudentPayment Modify(StudentPayment studentPayment)
        {
            try
            {
                var payment = studentPayment;
                Expression<Func<STUDENT_PAYMENT, bool>> selector = p => p.Payment_Id == payment.Id;
                STUDENT_PAYMENT paymentEntity = GetEntityBy(selector);

                if (paymentEntity == null || paymentEntity.Payment_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }
                PaymentVerificationLogic paymentVerificationLogic = new PaymentVerificationLogic();
                PaymentVerification paymentVerification = paymentVerificationLogic.GetBy(studentPayment.Id);

                if (paymentVerification == null)
                {
                    if (studentPayment.Amount > 0)
                    {
                        paymentEntity.Amount = studentPayment.Amount;
                    }
                    if (studentPayment.Level != null)
                    {
                        paymentEntity.Level_Id = studentPayment.Level.Id;
                    }
                    if (studentPayment.Status)
                    {
                        paymentEntity.Status = studentPayment.Status;
                    }

                    int modifiedRecordCount = Save();
                    studentPayment = GetModelBy(p => p.Payment_Id == payment.Id);
                    
                }

                return studentPayment;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
        public StudentPayment GetBy(Payment payment)
        {
            return GetModelBy(a => a.Payment_Id == payment.Id);
        }

        public async Task<StudentPayment> GetByAsync(Payment payment)
        {
            return await GetModelByAsync(a => a.Payment_Id == payment.Id);
        }

    }
}