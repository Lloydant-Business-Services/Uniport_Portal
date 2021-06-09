using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PaymentVerificationLogic:BusinessBaseLogic<PaymentVerification,PAYMENT_VERIFICATION>
    {
        public PaymentVerificationLogic()
        {
            translator = new PaymentVerificationTranslator();
        }
        public PaymentVerification GetBy(Int64 PaymentId)
       {
            try
            {
                return GetModelBy(a => a.Payment_Id == PaymentId);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        public async Task<PaymentVerification> GetByAsync(Int64 PaymentId)
        {
            try
            {
                return await GetModelByAsync(a => a.Payment_Id == PaymentId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public override PaymentVerification Create(PaymentVerification model)
        {
            PaymentVerification paymentVerification = GetBy(model.Payment.Id);
            if (paymentVerification == null)
            {
                paymentVerification = base.Create(model);
            }
            return paymentVerification;

        }
        public List<PaymentVerificationReport> GetVerificationReport(Department department,Session session, Programme programme, Level level,FeeType feeType)
       {
           List<PaymentVerificationReport> paymentVerifications = null;
           if (level.Id == 1)
           {
               paymentVerifications =(from a in
            repository.GetBy<VW_PAYMENT_VERIFICATION_NEW_STUDENT>( a => a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Session_Id == session.Id && a.Fee_Type_Id == feeType.Id)
            select new PaymentVerificationReport
            {
                Department = a.Department_Name,
                PaymentMode = a.Payment_Mode_Name,
                PaymentType = a.Fee_Type_Name,
                Level = "100 Level",
                StudentName = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                PaymentReference = a.Confirmation_No,
                Session = a.Session_Name,
                PaymentAmount = a.Transaction_Amount.ToString(),
                Programme = a.Programme_Name,
                FeeType = a.Fee_Type_Name,
                ReceiptNumber = a.Payment_Serial_Number.ToString(),
                VerificationOfficer = a.VerificationOfficer

            }).ToList();
           }
           else
           {
                paymentVerifications =(from a in
                repository.GetBy<VW_PAYMENT_VERIFICATION_OLD_STUDENT>( a => a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Session_Id == session.Id && a.Level_Id == level.Id && a.Fee_Type_Id == feeType.Id)
                select new PaymentVerificationReport
                {
                    Department = a.Department_Name,
                    PaymentMode = a.Payment_Mode_Name,
                    PaymentType = a.Fee_Type_Name,
                    Level = a.Level_Name,
                    StudentName = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                    PaymentReference = a.Confirmation_No,
                    Session = a.Session_Name,
                    PaymentAmount = a.Transaction_Amount.ToString(),
                    ReceiptNumber = a.Payment_Serial_Number.ToString(),
                    VerificationOfficer = a.VerificationOfficer

                }).ToList();
           }
           
            return paymentVerifications;
        }
        public List<PaymentVerificationReportAlt> GetVerificationReport(Department department, Session session, Programme programme, Level level, FeeType feeType, string dateFrom, string dateTo)
        {
            DateTime processedDateFrom = ConvertToDate(dateFrom);
            DateTime processedDateTo = ConvertToDate(dateTo);
            TimeSpan ts = new TimeSpan(00, 00, 0);
            processedDateFrom = processedDateFrom.Date + ts;
            ts = new TimeSpan(23, 59, 0);
            processedDateTo = processedDateTo.Date + ts;
            List<PaymentVerificationReportAlt> paymentVerifications = null;
            if (level.Id == 1)
            {
                paymentVerifications = (from a in
                                            repository.GetBy<VW_PAYMENT_VERIFICATION_NEW_STUDENT>(a => a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Session_Id == session.Id && a.Fee_Type_Id == feeType.Id && (a.DateVerified >= processedDateFrom && a.DateVerified <= processedDateTo))
                                        select new PaymentVerificationReportAlt
                                        {
                                            ReceiptNumber = a.Payment_Serial_Number.ToString(),
                                            Department = a.Department_Name,
                                            PaymentMode = a.Payment_Mode_Name,
                                            PaymentType = a.Fee_Type_Name,
                                            Level = "100 Level",
                                            StudentName = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                                            PaymentReference = a.Confirmation_No,
                                            Session = a.Session_Name,
                                            PaymentAmount = a.Transaction_Amount.ToString(),
                                            Programme = a.Programme_Name,
                                            FeeType = a.Fee_Type_Name,
                                            VerificationOfficer = a.VerificationOfficer,
                                            DateVerified = a.DateVerified,
                                            StartDate = processedDateFrom,
                                            EndDate = processedDateTo,
                                            DatePaid = a.Transaction_Date
                                        }).ToList();
            }
            else
            {
                paymentVerifications = (from a in
                                            repository.GetBy<VW_PAYMENT_VERIFICATION_OLD_STUDENT>(a => a.Programme_Id == programme.Id && a.Department_Id == department.Id && a.Session_Id == session.Id && a.Level_Id == level.Id && a.Fee_Type_Id == feeType.Id && (a.DateVerified >= processedDateFrom && a.DateVerified <= processedDateTo))
                                        select new PaymentVerificationReportAlt
                                        {
                                            ReceiptNumber = a.Payment_Serial_Number.ToString(),
                                            Department = a.Department_Name,
                                            PaymentMode = a.Payment_Mode_Name,
                                            PaymentType = a.Fee_Type_Name,
                                            Level = a.Level_Name,
                                            StudentName = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                                            PaymentReference = a.Confirmation_No,
                                            Session = a.Session_Name,
                                            PaymentAmount = a.Transaction_Amount.ToString(),
                                            VerificationOfficer = a.VerificationOfficer,
                                            DateVerified = a.DateVerified,
                                            StartDate = processedDateFrom,
                                            EndDate = processedDateTo,
                                            DatePaid = a.Transaction_Date
                                        }).ToList();
            }

            return paymentVerifications;
        }
        public List<PaymentVerificationReportAlt> GetVerificationReport(string dateFrom, string dateTo)
        {
            DateTime processedDateFrom = ConvertToDate(dateFrom);
            DateTime processedDateTo = ConvertToDate(dateTo);
            TimeSpan ts = new TimeSpan(00, 00, 0);
            processedDateFrom = processedDateFrom.Date + ts;
            ts = new TimeSpan(23, 59, 0);
            processedDateTo = processedDateTo.Date + ts;
            List<PaymentVerificationReportAlt> paymentVerifications = null;
           
                paymentVerifications = (from a in
                                            repository.GetBy<VW_PAYMENT_VERIFICATION>(a => (a.DateVerified >= processedDateFrom && a.DateVerified <= processedDateTo))
                                        select new PaymentVerificationReportAlt
                                        {
                                            ReceiptNumber = a.Payment_Serial_Number.ToString(),
                                            Department = a.Department_Name,
                                            PaymentMode = a.Payment_Mode_Name,
                                            PaymentType = a.Fee_Type_Name,
                                            Level = a.Level_Name,
                                            StudentName = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                                            PaymentReference = a.Confirmation_No,
                                            Session = a.Session_Name,
                                            PaymentAmount = a.Transaction_Amount.ToString(),
                                            Programme = a.Programme_Name,
                                            FeeType = a.Fee_Type_Name,
                                            VerificationOfficer = a.VerificationOfficer,
                                            DateVerified = a.DateVerified,
                                            StartDate = processedDateFrom,
                                            EndDate = processedDateTo,
                                            DatePaid = a.Transaction_Date,
                                            UserId = a.User_Id,
                                            MatricNumber = a.Matric_Number
                                            
                                        }).ToList();
            

            return paymentVerifications;
        }
        private DateTime ConvertToDate(string date)
        {
            DateTime newDate = new DateTime();
            try
            {
                
                if (date.Contains("/"))
                {
                   string[] dateSplit = date.Split('/');
                      newDate = new DateTime(Convert.ToInt32(dateSplit[2]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[0]));
                }
                else if(date.Contains("-"))
                {
                    string[] dateSplit = date.Split('-');
                    newDate = new DateTime(Convert.ToInt32(dateSplit[0]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[2]));
                }
                
              
            }
            catch (Exception)
            {
                throw;
            }

            return newDate;
        }
    }
}
