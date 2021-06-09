using System;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PaymentScholarshipLogic : BusinessBaseLogic<PaymentScholarship, PAYMENT_SCHOLARSHIP>
    {
        public PaymentScholarshipLogic()
        {
            translator = new PaymentScholarshipTranslator();
        }

        public PaymentScholarship GetBy(Person person)
        {
            Expression<Func<PAYMENT_SCHOLARSHIP, bool>> selector = a => a.Person_Id == person.Id;
            return GetModelBy(selector);
        }

        public bool IsStudentOnScholarship(Person person, Session session)
        {
            Expression<Func<PAYMENT_SCHOLARSHIP, bool>> selector =
                a => a.Person_Id == person.Id && a.Session_id == session.Id;
            PaymentScholarship scholarship = GetModelBy(selector);
            if (scholarship != null)
            {
                return true;
            }
            return false;
        }
    }
}