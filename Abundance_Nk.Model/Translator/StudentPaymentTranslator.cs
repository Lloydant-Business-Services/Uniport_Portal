using System;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;

namespace Abundance_Nk.Model.Translator
{
    public class StudentPaymentTranslator : TranslatorBase<StudentPayment, STUDENT_PAYMENT>
    {
        private readonly LevelTranslator levelTranslator;
        private readonly SessionTranslator sessionTranslator;
        private readonly PersonTranslator personTranslator;
        private FeeTranslator feeTranslator;
        private PaymentModeTranslator paymentModeTranslator;
        private PaymentTranslator paymentTranslator;
        private PaymentTypeTranslator paymentTypeTranslator;
        private PersonTypeTranslator personTypeTranslator;

        public StudentPaymentTranslator()
        {
            personTypeTranslator = new PersonTypeTranslator();
            paymentTypeTranslator = new PaymentTypeTranslator();
            paymentTranslator = new PaymentTranslator();
            personTranslator = new PersonTranslator();
            sessionTranslator = new SessionTranslator();
            levelTranslator = new LevelTranslator();
            paymentModeTranslator = new PaymentModeTranslator();
            feeTranslator = new FeeTranslator();
        }

        public override StudentPayment TranslateToModel(STUDENT_PAYMENT studentPaymentEntity)
        {
            try
            {
                StudentPayment studentPayment = null;
                if (studentPaymentEntity != null)
                {
                    studentPayment = new StudentPayment();
                    studentPayment.Id = studentPaymentEntity.Payment_Id;
                    studentPayment.Student = personTranslator.TranslateToModel(studentPaymentEntity.PERSON);
                    studentPayment.Session = sessionTranslator.TranslateToModel(studentPaymentEntity.SESSION);
                    studentPayment.Level = levelTranslator.TranslateToModel(studentPaymentEntity.LEVEL);
                    studentPayment.Amount = studentPaymentEntity.Amount;
                    studentPayment.Status = studentPaymentEntity.Status;
                }

                return studentPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override STUDENT_PAYMENT TranslateToEntity(StudentPayment studentPayment)
        {
            try
            {
                STUDENT_PAYMENT studentPaymentEntity = null;
                if (studentPayment != null)
                {
                    studentPaymentEntity = new STUDENT_PAYMENT();
                    studentPaymentEntity.Payment_Id = studentPayment.Id;
                    studentPaymentEntity.Person_Id = studentPayment.Student.Id;
                    studentPaymentEntity.Session_Id = studentPayment.Session.Id;
                    studentPaymentEntity.Level_Id = studentPayment.Level.Id;
                    studentPaymentEntity.Status = studentPayment.Status;
                    studentPaymentEntity.Amount = studentPayment.Amount;
                }

                return studentPaymentEntity;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}