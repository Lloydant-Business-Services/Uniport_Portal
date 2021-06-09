using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;
using System.Threading.Tasks;
using System.Configuration;

namespace Abundance_Nk.Business
{
    public class PaymentLogic : BusinessBaseLogic<Payment, PAYMENT>
    {
        private readonly FeeDetailLogic feeDetailLogic;
        string QRVerificationUrl = ConfigurationManager.AppSettings["QRVerificationUrl"].ToString();

        public PaymentLogic()
        {
            feeDetailLogic = new FeeDetailLogic();
            translator = new PaymentTranslator();
        }

        public List<PaymentView> GetBy(Person person)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_PAYMENT>(p => p.Person_Id == person.Id)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ReceiptNumber = p.Receipt_No,
                                                  ConfirmationOrderNumber = p.Confirmation_No,
                                                  BankCode = p.Bank_Code,
                                                  BankName = p.Bank_Name,
                                                  BranchCode = p.Branch_Code,
                                                  BranchName = p.Branch_Name,
                                                  PaymentDate = p.Transaction_Date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  PaymentTypeName = p.Payment_Type_Name,
                                                  Amount = p.Transaction_Amount,
                                                  FormatedAmount = String.Format("{0:N}", p.Transaction_Amount),
                                                  SessionName =  p.Session_Name
                                                 
                                              }).ToList();
                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PaymentView> GetBy(RemitaPayment remitaPayment)
        {
            try
            {
                List<PaymentView> payments =
                    (from p in repository.GetBy<VW_REMITA_PAYMENT>(p => p.Person_Id == remitaPayment.payment.Person.Id)
                     select new PaymentView
                     {
                         PersonId = p.Person_Id,
                         PaymentId = p.Payment_Id,
                         InvoiceNumber = p.Invoice_Number,
                         ReceiptNumber = p.Invoice_Number,
                         ConfirmationOrderNumber = p.RRR,
                         BankCode = p.Bank_Code,
                         BankName = "",
                         BranchCode = p.Branch_Code,
                         BranchName = "",
                         PaymentDate = p.Transaction_Date,
                         FeeTypeId = p.Fee_Type_Id,
                         FeeTypeName = p.Fee_Type_Name,
                         PaymentTypeId = p.Payment_Type_Id,
                         PaymentTypeName = p.Payment_Type_Name,
                         Amount = p.Transaction_Amount,
                     }).ToList();
                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AcceptanceView> GetAcceptanceReportBy(Session session, Department department, Programme programme)
        {
            try
            {
                List<AcceptanceView> payments =
                    (from p in
                         repository.GetBy<ACCPTANCE__REPORT>(
                             p =>
                                 p.Department_Id == department.Id && p.Programme_Id == programme.Id &&
                                 p.Session_Id == session.Id)
                     select new AcceptanceView
                     {
                         Person_Id = p.Person_Id,
                         Application_Exam_Number = p.Application_Exam_Number,
                         Invoice_Number = p.Invoice_Number,
                         Application_Form_Number = p.Application_Form_Number,
                         First_Choice_Department_Name = p.Department_Name,
                         Name = p.SURNAME + ' ' + p.FIRSTNAME + ' ' + p.OTHER_NAMES,
                         RRR = " ",
                         Programme_Name = p.Programme_Name,
                     }).OrderBy(b => b.Name).ToList();
                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Payment GetBy(Person person, FeeType feeType)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Person_Id == person.Id && p.Fee_Type_Id == feeType.Id;
                Payment payment = GetModelsBy(selector).FirstOrDefault();
               // SetFeeDetails(payment);

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Payment GetBy(FeeType feeType, Person person, Session session)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Fee_Type_Id == feeType.Id && p.Person_Id == person.Id && p.Session_Id == session.Id;
                Payment payment = GetModelsBy(selector).LastOrDefault();
                //SetFeeDetails(payment);
                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Payment GetBy(long id)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Payment_Id == id;
                Payment payment = GetModelBy(selector);
                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Payment GetBy(FeeType feeType, Person person, PaymentMode paymentMode, Session session)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector =
                    p =>
                        p.Fee_Type_Id == feeType.Id && p.Payment_Mode_Id == paymentMode.Id && p.Person_Id == person.Id &&
                        p.Session_Id == session.Id;
                Payment payment = GetModelsBy(selector).FirstOrDefault();

                //SetFeeDetails(payment);

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Payment> GetByAsync(FeeType feeType, Person person, PaymentMode paymentMode, Session session)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector =
                    p =>
                        p.Fee_Type_Id == feeType.Id && p.Payment_Mode_Id == paymentMode.Id && p.Person_Id == person.Id &&
                        p.Session_Id == session.Id;
                Payment payment = await GetModelByAsync(selector);

                //SetFeeDetails(payment);

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }


        //private void SetFeeDetails(Payment payment)
        //{
        //    try
        //    {
        //        if (payment != null && payment.Id > 0)
        //        {
        //            payment.FeeDetails = feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == payment.FeeType.Id);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //private async Task SetFeeDetailsAsync(Payment payment)
        //{
        //    try
        //    {
        //        if (payment != null && payment.Id > 0)
        //        {
        //            payment.FeeDetails = await feeDetailLogic.GetModelsByAsync(f => f.Fee_Type_Id == payment.FeeType.Id);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public List<FeeDetail> SetFeeDetails(Payment payment, Int32? ProgrammeId, Int32? LevelId, Int32? PaymentModeId,
            Int32? DepartmentId, Int32? SessionId)
        {
            var feedetail = new List<FeeDetail>();
            try
            {
                if (payment != null && payment.Id > 0)
                {
                    feedetail =
                        feeDetailLogic.GetModelsBy(
                            f =>
                                f.Fee_Type_Id == payment.FeeType.Id && f.Programme_Id == ProgrammeId &&
                                f.Level_Id == LevelId && f.Payment_Mode_Id == PaymentModeId &&
                                f.Department_Id == DepartmentId && f.Session_Id == SessionId);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return feedetail;
        }

        public async Task<List<FeeDetail>> SetFeeDetailsListAsync(Payment payment, Int32? ProgrammeId, Int32? LevelId, Int32? PaymentModeId,
          Int32? DepartmentId, Int32? SessionId)
        {
            var feedetail = new List<FeeDetail>();
            try
            {
                if (payment != null && payment.Id > 0)
                {
                    feedetail =  await feeDetailLogic.GetModelsByAsync(
                            f =>
                                f.Fee_Type_Id == payment.FeeType.Id && f.Programme_Id == ProgrammeId &&
                                f.Level_Id == LevelId && f.Payment_Mode_Id == PaymentModeId &&
                                f.Department_Id == DepartmentId && f.Session_Id == SessionId);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return feedetail;
        }

        public async Task<decimal> SetFeeDetailsAsync(Payment payment, Int32? ProgrammeId, Int32? LevelId, Int32? PaymentModeId,
            Int32? DepartmentId, Int32? SessionId)
        {
            //var feedetail = new List<FeeDetail>();
            var amount = 0m;
            try
            {
                if (payment != null && payment.Id > 0)
                {
                    var feedetail = await
                        feeDetailLogic.GetModelsByAsync(
                            f =>
                                f.Fee_Type_Id == payment.FeeType.Id && f.Programme_Id == ProgrammeId &&
                                f.Level_Id == LevelId && f.Payment_Mode_Id == PaymentModeId &&
                                f.Department_Id == DepartmentId && f.Session_Id == SessionId);

                    amount = feedetail.Sum(a => a.Fee.Amount);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return amount;
        }

        //public async Task<List<FeeDetail>> SetFeeDetailsList (FeeType feeType)
        //{
        //    var feedetail = new List<FeeDetail>();
        //    try
        //    {
        //        if (feeType != null && feeType.Id > 0)
        //        {
        //            feedetail = await feeDetailLogic.GetModelsByAsync(f => f.Fee_Type_Id == feeType.Id);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return feedetail;
        //}

        //public List<FeeDetail> SetFeeDetails(FeeType feeType)
        //{
        //    var feedetail = new List<FeeDetail>();
        //    try
        //    {
        //        if (feeType != null && feeType.Id > 0)
        //        {
        //            feedetail = feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == feeType.Id);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return feedetail;
        //}


        //public async Task<List<FeeDetail>> SetFeeDetailsAsync(FeeType feeType)
        //{
        //    var feedetail = new List<FeeDetail>();
        //    try
        //    {
        //        if (feeType != null && feeType.Id > 0)
        //        {
        //            feedetail = await feeDetailLogic.GetModelsByAsync(f => f.Fee_Type_Id == feeType.Id);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return feedetail;
        //}

        //public List<FeeDetail> SetFeeDetails(long FeeId)
        //{
        //    var feedetail = new List<FeeDetail>();
        //    try
        //    {
        //        if (FeeId > 0)
        //        {
        //            feedetail = feeDetailLogic.GetModelsBy(f => f.Fee_Id == FeeId);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    return feedetail;
        //}

        public Payment GetBy(string invoiceNumber)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Invoice_Number == invoiceNumber;
                Payment payment = GetModelBy(selector);

               // SetFeeDetails(payment);

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Payment> GetByAsync(string invoiceNumber)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Invoice_Number == invoiceNumber;
                Payment payment = await GetModelByAsync(selector);

                //SetFeeDetails(payment);

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool PaymentAlreadyMade(Payment payment)
        {
            try
            {
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelBy(a => a.Payment_Id == payment.Id);
                if (paymentEtranzact != null && paymentEtranzact.Payment != null && paymentEtranzact.Payment.Payment.Id > 0)
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

        public bool HasPaidAcceptance(Person person)
        {
            PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
            PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelBy(a => a.ONLINE_PAYMENT.PAYMENT.Person_Id == person.Id && a.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.AcceptanceFee);
            if (paymentEtranzact != null && paymentEtranzact.Payment != null && paymentEtranzact.Payment.Payment.Id > 0)
            {
                return true;
            }
            else
            {
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                var remitapayment=remitaPaymentLogic.GetModelsBy(f => f.PAYMENT.Person_Id == person.Id && f.PAYMENT.Fee_Type_Id == (int)FeeTypes.AcceptanceFee && (f.Status.Contains("01") || f.Description.Contains("manual"))).FirstOrDefault();
                if (remitapayment?.payment?.Id > 0)
                    return true;

            }
            return false;
        }

        public async Task<bool> PaymentAlreadyMadeAsync(Payment payment)
        {
            try
            {
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact paymentEtranzact = await paymentEtranzactLogic.GetModelByAsync(a => a.Payment_Id == payment.Id);
                if (paymentEtranzact != null && paymentEtranzact.Payment != null && paymentEtranzact.Payment.Payment.Id > 0)
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

        public bool SetInvoiceNumber(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Payment_Id == payment.Id;
                PAYMENT entity = base.GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Payment_Serial_Number = payment.SerialNumber;
                entity.Invoice_Number = payment.InvoiceNumber;

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

        public override Payment Create(Payment payment)
        {
            try
            {
                Payment newPayment = base.Create(payment);
                if (newPayment == null || newPayment.Id <= 0)
                {
                    throw new Exception("Payment ID not set!");
                }

                newPayment = SetNextPaymentNumber(newPayment);
                SetInvoiceNumber(newPayment);
                newPayment.FeeType = payment.FeeType;
                //SetFeeDetails(newPayment);
                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async override Task<Payment> CreateAsync (Payment payment)
        {
            try
            {
                Payment newPayment = await  base.CreateAsync(payment);
                if (newPayment == null || newPayment.Id <= 0)
                {
                    throw new Exception("Payment ID not set!");
                }

                newPayment = SetNextPaymentNumber(newPayment);
                SetInvoiceNumber(newPayment);
                newPayment.FeeType = payment.FeeType;
                //await SetFeeDetailsAsync(newPayment);
                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Payment SetNextPaymentNumber(Payment payment)
        {
            try
            {
                payment.SerialNumber = payment.Id;
                payment.InvoiceNumber = "UNIPORT" + DateTime.Now.ToString("yy") + PaddNumber(payment.Id, 10);

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string PaddNumber(long id, int maxCount)
        {
            try
            {
                string idInString = id.ToString();
                string paddNumbers = "";
                if (idInString.Count() < maxCount)
                {
                    int zeroCount = maxCount - id.ToString().Count();
                    var builder = new StringBuilder();
                    for (int counter = 0; counter < zeroCount; counter++)
                    {
                        builder.Append("0");
                    }

                    builder.Append(id);
                    paddNumbers = builder.ToString();
                    return paddNumbers;
                }

                return paddNumbers;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InvalidConfirmationOrderNumber(string invoiceNo, string confirmationOrderNo)
        {
            try
            {
                List<PaymentEtranzact> payments =
                    (from p in repository.GetBy<VW_PAYMENT>(p => p.Invoice_Number == invoiceNo)
                     select new PaymentEtranzact
                     {
                         ConfirmationNo = p.Confirmation_No,
                     }).ToList();

                if (payments != null)
                {
                    if (payments.Count > 1)
                    {
                        throw new Exception("Duplicate Invoice Number '" + invoiceNo +
                                            "' detected! Please contact your system administrator.");
                    }
                    if (payments.Count == 1)
                    {
                        if (payments[0].ConfirmationNo == confirmationOrderNo)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Payment InvalidConfirmationOrderNumber(string confirmationOrderNumber, int feeType)
        {
            try
            {
                var payment = new Payment();
                var etranzactLogic = new PaymentEtranzactLogic();
                PaymentEtranzact etranzactDetails =
                    etranzactLogic.GetModelBy(m => m.Confirmation_No == confirmationOrderNumber);
                if (etranzactDetails == null || etranzactDetails.ReceiptNo == null)
                {
                    var paymentTerminal = new PaymentTerminal();
                    var paymentTerminalLogic = new PaymentTerminalLogic();
                    paymentTerminal = paymentTerminalLogic.GetModelBy(p => p.Fee_Type_Id == feeType && p.Session_Id == 1);

                    etranzactDetails = etranzactLogic.RetrievePinAlternative(confirmationOrderNumber, paymentTerminal);
                    if (etranzactDetails != null && etranzactDetails.ReceiptNo != null)
                    {
                        var paymentLogic = new PaymentLogic();
                        payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                        if (payment != null && payment.Id > 0)
                        {
                            var feeDetail = new FeeDetail();
                            var feeDetailLogic = new FeeDetailLogic();
                            feeDetail = feeDetailLogic.GetModelBy(a => a.Fee_Type_Id == payment.FeeType.Id);
                            if (!etranzactLogic.ValidatePin(etranzactDetails, payment, feeDetail.Fee.Amount))
                            {
                                throw new Exception(
                                    "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                            }
                        }
                        else
                        {
                            throw new Exception(
                                "The invoice number attached to the pin doesn't belong to you! Please cross check and try again.");
                        }
                    }
                    else
                    {
                        throw new Exception(
                            "Confirmation Order Number entered seems not to be valid! Please cross check and try again.");
                    }
                }
                else
                {
                    var paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                    if (payment != null && payment.Id > 0)
                    {
                        //FeeDetail feeDetail = new FeeDetail();
                        var feeDetailLogic = new FeeDetailLogic();
                        //feeDetail = feeDetailLogic.GetModelBy(a => a.Fee_Type_Id == payment.FeeType.Id);

                        List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(a => a.Fee_Type_Id == payment.FeeType.Id);
                        decimal amount = feeDetails.Sum(a => a.Fee.Amount);
                        if (!etranzactLogic.ValidatePin(etranzactDetails, payment, amount))
                        {
                            throw new Exception(
                                "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                            //payment = null;
                            //return payment;
                        }
                    }
                    else
                    {
                        throw new Exception(
                            "The invoice number attached to the pin doesn't belong to you! Please cross check and try again.");
                    }
                }

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Payment InvalidConfirmationOrderNumber(string confirmationOrderNumber, Session session, FeeType feetype)
        {
            try
            {
                var payment = new Payment();

                //if (confirmationOrderNumber.Contains("ABSU") && (feetype.Id != 3 && feetype.Id != 2))
                //{
                //    return payment;
                //}
                //if (confirmationOrderNumber.Contains("ABSU") && (feetype.Id ==81))
                //{
                //    return payment;
                //}

                var etranzactLogic = new PaymentEtranzactLogic();
                RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
                var remitaPayment = remitaPaymentLogic.GetModelsBy(f=>f.RRR==confirmationOrderNumber).FirstOrDefault();
                payment=remitaPayment.payment;
                    if (payment != null && payment.Id > 0)
                    {
                        var feeDetail = new List<FeeDetail>();
                        var feeDetailLogic = new FeeDetailLogic();

                        if (payment.FeeType.Id == (int)FeeTypes.SchoolFees)
                        {
                            var studentPayment = new StudentPayment();
                            var studentPaymentLogic = new StudentPaymentLogic();
                            studentPayment = studentPaymentLogic.GetModelBy(a => a.Payment_Id == payment.Id);
                            if (studentPayment != null)
                            {
                                if (!remitaPaymentLogic.ValidatePin(remitaPayment, payment, studentPayment.Amount))
                                {
                                    throw new Exception(
                                        "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                                }
                            }
                            else
                            {
                                var admissionList = new AdmissionList();
                                var admissionListLogic = new AdmissionListLogic();
                                admissionList = admissionListLogic.GetBy(payment.Person);
                                if (admissionList != null)
                                {
                                    var level = new Level { Id = 1 };
                                    decimal AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(
                                        admissionList.Deprtment, level, admissionList.Programme,
                                        payment.FeeType, payment.Session, payment.PaymentMode);

                                    if (!remitaPaymentLogic.ValidatePin(remitaPayment, payment, AmountToPay))
                                    {
                                        throw new Exception(
                                            "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                                    }
                                }
                                else
                                {
                                    if (session.Id == 7)
                                    {
                                        throw new Exception(
                                       "Unable to validate admission status. Please contact support@lloydant.com.");
                                    }
                                }
                                //var serviceResponse2 = SendDetailsToBIR(payment, etranzactDetails, "SCHF");

                            }
                        }
                        else if (payment.FeeType.Id == (int)FeeTypes.AcceptanceFee)
                        {
                            var admissionList = new AdmissionList();
                            var admissionListLogic = new AdmissionListLogic();
                            
                            
                            admissionList = admissionListLogic.GetBy(payment.Person);
                            if (admissionList != null)
                            {
                                var level = new Level { Id = 1 };
                                decimal AmountToPay = feeDetailLogic.GetFeeByDepartmentLevel(admissionList.Deprtment,
                                    level, admissionList.Programme, payment.FeeType, payment.Session,
                                    payment.PaymentMode);

                                if (!remitaPaymentLogic.ValidatePin(remitaPayment, payment, AmountToPay))
                                {
                                    throw new Exception(
                                        "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                                }
                            }
                            else
                            {
                                if (session.Id == 7)
                                {
                                    throw new Exception(
                                   "Unable to validate admission status. Please contact support@lloydant.com.");
                                }
                            }

                        }
                        else
                        {
                            throw new Exception("The pin amount could not be verified against your details. Please contact support@lloydant.com.");
                            //payment.FeeDetails = feeDetailLogic.GetModelsBy(a => a.Fee_Type_Id == payment.FeeType.Id);

                            //if (
                            //    !etranzactLogic.ValidatePin(etranzactDetails, payment,
                            //        payment.FeeDetails.Sum(p => p.Fee.Amount)))
                            //{
                            //    throw new Exception(
                            //        "The pin amount tied to the pin is not correct. Please contact support@lloydant.com.");
                            //}
                        }
                        //var serviceResponse = SendDetailsToBIR(payment, etranzactDetails, "ACPT");

                    }
                    else
                    {
                        throw new Exception(
                            "The invoice number attached to the pin doesn't belong to you! Please cross check and try again.");
                    }
             

                return payment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Response SendDetailsToBIR(Payment payment, PaymentEtranzact etranzactDetails, string paymentCode)
        {
            try
            {
               // return new Response();
                StudentRegistrationRequest studentRegistrationRequest = new StudentRegistrationRequest();
                if (payment.Person.ContactAddress != null)
                {
                    studentRegistrationRequest.address = payment.Person.ContactAddress;
                }
                else
                {
                    studentRegistrationRequest.address = "--";
                }
                studentRegistrationRequest.addressTown = "--";
                if (payment.Person.DateOfBirth != null)
                {
                    studentRegistrationRequest.dateOfBirth = payment.Person.DateOfBirth.Value.Day + "-" +
                                                             payment.Person.DateOfBirth.Value.Month + "-" +
                                                             payment.Person.DateOfBirth.Value.Year;
                }
                else
                {
                    studentRegistrationRequest.dateOfBirth = "2-2-1985";
                }

                studentRegistrationRequest.firstName = payment.Person.FirstName;
                if (payment.Person.Sex != null)
                {
                    studentRegistrationRequest.gender = payment.Person.Sex.Name.Substring(0, 1);
                }
                else
                {
                    studentRegistrationRequest.gender = "m";
                }

                studentRegistrationRequest.middleName = payment.Person.OtherName;
                studentRegistrationRequest.stateCode = "AB";
                studentRegistrationRequest.studentCurrentClass = "100LEVEL";
                studentRegistrationRequest.studentId = payment.Person.Id;
                studentRegistrationRequest.surname = payment.Person.LastName;
                studentRegistrationRequest.trackingId = DateTime.Now.Ticks.ToString();
                studentRegistrationRequest.yearOfRegistration = DateTime.Now.Year.ToString();

                PaymentService paymentService = new PaymentService();
                RegistrationResponse serviceResponse = paymentService.PostJsonDataToUrl2(paymentService.baseRegistrationURL,
                    studentRegistrationRequest);

                Request paymentRequest = new Request();
                paymentRequest.payerFirstName = payment.Person.FirstName;
                paymentRequest.payerSurname = payment.Person.LastName;
                paymentRequest.totalAmountInKobo = Convert.ToInt64(etranzactDetails.TransactionAmount * 100);
                paymentRequest.trackingId = DateTime.Now.Ticks.ToString();

                paymentRequest.studentId = payment.Person.Id;
                paymentRequest.dateOfPayment = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" +
                                               DateTime.Now.Year.ToString();
                paymentLineItems lineItem = new paymentLineItems();
                lineItem.amountInKobo = Convert.ToInt64(etranzactDetails.TransactionAmount * 100);
                lineItem.datePaid = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" +
                                    DateTime.Now.Year.ToString();
                lineItem.description = payment.FeeType.Name;
                lineItem.itemCode = paymentCode;

                paymentRequest.paymentLineItems = new List<paymentLineItems>();
                paymentRequest.paymentLineItems.Add(lineItem);

                Response serviceResponse2 = paymentService.PostJsonDataToUrl(paymentService.basePaymentURL, paymentRequest);
                return serviceResponse2;
            }
            catch (Exception)
            {

                throw;
            }
        }


        //public bool Modify(Payment payment)
        //{
        //    try
        //    {
        //        Expression<Func<PAYMENT, bool>> selector = p => p.Payment_Id == payment.Id;
        //        PAYMENT entity = GetEntityBy(selector);

        //        if (entity == null || entity.Person_Id <= 0)
        //        {
        //            throw new Exception(NoItemFound);
        //        }

        //        if (payment.Person != null)
        //        {
        //            entity.Person_Id = payment.Person.Id;
        //        }
        //        entity.Fee_Type_Id = payment.FeeType.Id;
        //        int modifiedRecordCount = Save();
        //        if (modifiedRecordCount <= 0)
        //        {
        //            return false;
        //        }

        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public decimal GetPaymentAmount(Payment payment)
        {
            decimal Amount = 0;
            try
            {
                var feeDetail = new FeeDetail();
                var feeDetailLogic = new FeeDetailLogic();
                feeDetail = feeDetailLogic.GetModelBy(f => f.Fee_Type_Id == payment.FeeType.Id);
                Amount = feeDetail.Fee.Amount;
            }
            catch (Exception ex)
            {
                throw;
            }
            return Amount;
        }

        public void DeleteBy(long PaymentID)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = a => a.Payment_Id == PaymentID;
                Delete(selector);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<RegistrationBalanceReport> GetRegistrationBalanceList(Session session, Semester semester)
        {
            var paymentEtranzactLogic = new PaymentEtranzactLogic();
            var RegistrationBalanceList = new List<RegistrationBalanceReport>();
            var studentLevelLogic = new StudentLevelLogic();
            var studentLevel = new STUDENT_LEVEL();

            var courseRegistrationLogic = new CourseRegistrationLogic();
            var courseRegistrationDetailLogic = new CourseRegistrationDetailLogic();
            var courseRegistration = new STUDENT_COURSE_REGISTRATION();
            var courseRegistrationDetail = new STUDENT_COURSE_REGISTRATION_DETAIL();

            try
            {
                if (session != null)
                {
                    List<PAYMENT> payments =
                        GetEntitiesBy(p => p.Fee_Type_Id == 3 && p.Session_Id == session.Id).Take(3000).ToList();
                    foreach (PAYMENT payment in payments)
                    {
                        int studentNumberPay = 0;
                        int studentNumberReg = 0;
                        var paymentEtranzact = new PAYMENT_ETRANZACT();
                        paymentEtranzact = paymentEtranzactLogic.GetEntityBy(p => p.Payment_Id == payment.Payment_Id);
                        if (paymentEtranzact != null)
                        {
                            var registrationBalanceReport = new RegistrationBalanceReport();
                            studentLevel =
                                studentLevelLogic.GetEntityBy(
                                    p => p.Person_Id == payment.Person_Id && p.Session_Id == session.Id);

                            if (session != null && semester != null)
                            {
                                courseRegistration =
                                    courseRegistrationLogic.GetEntityBy(
                                        p =>
                                            p.Session_Id == session.Id && p.Person_Id == payment.Person_Id &&
                                            p.Level_Id == studentLevel.Level_Id &&
                                            p.Department_Id == studentLevel.Department_Id &&
                                            p.Programme_Id == studentLevel.Programme_Id);
                                if (courseRegistration != null)
                                {
                                    courseRegistrationDetail =
                                        courseRegistrationDetailLogic.GetEntitiesBy(
                                            p =>
                                                p.Semester_Id == semester.Id &&
                                                p.Student_Course_Registration_Id ==
                                                courseRegistration.Student_Course_Registration_Id).FirstOrDefault();
                                }
                            }

                            registrationBalanceReport.Department = studentLevel.DEPARTMENT.Department_Name;

                            if (studentLevel.Level_Id == 1 && studentLevel.Programme_Id == 2)
                            {
                                registrationBalanceReport.ProgrammePayment = "PART TIME 1 (PAY)";

                                //registrationBalanceReport.Payment = "(PAY)";
                                registrationBalanceReport.PaymentNumber = studentNumberPay += 1;
                                if (courseRegistrationDetail != null)
                                {
                                    registrationBalanceReport.ProgrammeRegistration = " PART TIME 1 (REG)";
                                    registrationBalanceReport.RegistrationNumber = studentNumberReg += 1;
                                }

                                RegistrationBalanceList.Add(registrationBalanceReport);
                            }
                            else if (studentLevel.Level_Id == 2 && studentLevel.Programme_Id == 2)
                            {
                                registrationBalanceReport.ProgrammePayment = "PART TIME 2 (PAY)";
                                //registrationBalanceReport.Payment = "(PAY)";
                                registrationBalanceReport.PaymentNumber = studentNumberPay += 1;
                                if (courseRegistrationDetail != null)
                                {
                                    registrationBalanceReport.ProgrammeRegistration = "PART TIME 2 (REG)";
                                    registrationBalanceReport.RegistrationNumber = studentNumberReg += 1;
                                }

                                RegistrationBalanceList.Add(registrationBalanceReport);
                            }
                            else
                            {
                                registrationBalanceReport.ProgrammePayment = studentLevel.LEVEL.Level_Name + " (PAY)";
                                //registrationBalanceReport.Payment = "(PAY)";
                                registrationBalanceReport.PaymentNumber = studentNumberPay += 1;
                                if (courseRegistrationDetail != null)
                                {
                                    registrationBalanceReport.ProgrammeRegistration = studentLevel.LEVEL.Level_Name +
                                                                                      " (REG)";
                                    registrationBalanceReport.RegistrationNumber = studentNumberReg += 1;
                                }

                                RegistrationBalanceList.Add(registrationBalanceReport);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RegistrationBalanceList;
        }

        public List<PaymentReport> GetPaymentsBy(Session session)
        {
            var paymentEtranzactLogic = new PaymentEtranzactLogic();
            var PaymentReportList = new List<PaymentReport>();
            var studentLevelLogic = new StudentLevelLogic();
            var studentLevel = new StudentLevel();
            var studentPayment = new StudentPayment();
            var studentPaymentLogic = new StudentPaymentLogic();

            try
            {
                if (session != null)
                {
                    List<PAYMENT> payments = GetEntitiesBy(p => p.Fee_Type_Id == 3 && p.Session_Id == session.Id);
                    foreach (PAYMENT payment in payments)
                    {
                        int studentNumber = 0;
                        var paymentEtranzact = new PAYMENT_ETRANZACT();
                        paymentEtranzact = paymentEtranzactLogic.GetEntityBy(p => p.Payment_Id == payment.Payment_Id);
                        if (paymentEtranzact != null)
                        {
                            studentLevel = studentLevelLogic.GetBy(payment.Person_Id);
                            if (studentLevel != null && studentLevel.Id > 0)
                            {
                                var paymentReport = new PaymentReport();
                                studentPayment = studentPaymentLogic.GetModelBy(a => a.Payment_Id == payment.Payment_Id && a.Session_Id == session.Id);
                                paymentReport.Department = studentLevel.Department.Name;
                                paymentReport.Programme = studentLevel.Programme.Name;
                                //if (studentPayment != null && studentPayment.Id > 0)
                                //{
                                //    paymentReport. = studentPayment.Level.Name;
                                //}
                                //else
                                //{  
                                //    paymentReport.Programme = studentPayment.Level.Name;
                                //    paymentReport.Programme = studentPayment.Level.Name;
                                //}
                                paymentReport.StudentNumber = studentNumber += 1;
                                PaymentReportList.Add(paymentReport);

                            }


                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return PaymentReportList;
        }

        public List<DuplicateMatricNumberFix> GetPartTimeGuys()
        {
            var dupList = new List<DuplicateMatricNumberFix>();
            List<PAYMENT> paymentList = GetEntitiesBy(p => p.Fee_Type_Id == 6 && p.Session_Id == 1);
            var etranzactLogic = new PaymentEtranzactLogic();
            foreach (PAYMENT paymentItem in paymentList)
            {
                PAYMENT_ETRANZACT paymentEtranzact =
                    etranzactLogic.GetEntityBy(p => p.Payment_Id == paymentItem.Payment_Id);
                if (paymentEtranzact != null)
                {
                    var dup = new DuplicateMatricNumberFix();
                    dup.Fullname = paymentItem.PERSON.Last_Name + " " + paymentItem.PERSON.First_Name + " " +
                                   paymentItem.PERSON.Other_Name;
                    dup.ConfirmationOrder = paymentEtranzact.Confirmation_No;
                    dup.ReceiptNumber = paymentEtranzact.Receipt_No;
                    dupList.Add(dup);
                }
            }
            return dupList.OrderBy(p => p.Fullname).ToList();
        }

        public List<FeesPaymentReport> GetFeesPaymentBy(Session session, Programme programme, Department department,
            Level level)
        {
            var paymentEtranzactLogic = new PaymentEtranzactLogic();
            var feesPaymentReportList = new List<FeesPaymentReport>();
            var studentLevelLogic = new StudentLevelLogic();
            var remitaPaymentLogic = new RemitaPaymentLogic();
            var studentLevelList = new List<STUDENT_LEVEL>();
            var admissionList = new List<ADMISSION_LIST>();
            var payments = new List<PAYMENT>();
            var paymentEtranzact = new PAYMENT_ETRANZACT();
            var remitaPayment = new REMITA_PAYMENT();
            var thisPaymentEtranzact = new PAYMENT_ETRANZACT();
            var thisRemitaPayment = new REMITA_PAYMENT();

            try
            {
                if (session != null && programme != null && department != null && level != null)
                {
                    studentLevelList =
                        studentLevelLogic.GetEntitiesBy(
                            p =>
                                p.Department_Id == department.Id && p.Level_Id == level.Id &&
                                p.Programme_Id == programme.Id && p.Session_Id == session.Id);

                    foreach (STUDENT_LEVEL studentLevel in studentLevelList)
                    {
                        var confirmedPayments = new List<PAYMENT>();
                        payments =
                            GetEntitiesBy(
                                p => p.Person_Id == studentLevel.Person_Id && (p.Fee_Type_Id == 3 || p.Fee_Type_Id == 2));
                        foreach (PAYMENT payment in payments)
                        {
                            paymentEtranzact = paymentEtranzactLogic.GetEntityBy(p => p.Payment_Id == payment.Payment_Id);
                            if (paymentEtranzact != null)
                            {
                                confirmedPayments.Add(payment);
                            }
                            else
                            {
                                remitaPayment = remitaPaymentLogic.GetEntityBy(p => p.Payment_Id == payment.Payment_Id);
                                if (remitaPayment != null)
                                {
                                    confirmedPayments.Add(payment);
                                }
                            }
                        }

                        var feesPaymentReport = new FeesPaymentReport();
                        foreach (PAYMENT confirmedPayment in confirmedPayments)
                        {
                            thisPaymentEtranzact =
                                paymentEtranzactLogic.GetEntityBy(p => p.Payment_Id == confirmedPayment.Payment_Id);
                            thisRemitaPayment =
                                remitaPaymentLogic.GetEntityBy(p => p.Payment_Id == confirmedPayment.Payment_Id);
                            if (confirmedPayment.Fee_Type_Id == 2)
                            {
                                feesPaymentReport.AcceptanceFeeInvoiceNumber = confirmedPayment.Invoice_Number;
                                if (thisPaymentEtranzact != null)
                                {
                                    feesPaymentReport.AcceptanceTransactionAmount = "N" +
                                                                                    paymentEtranzactLogic.GetModelBy(
                                                                                        p =>
                                                                                            p.Payment_Id ==
                                                                                            confirmedPayment.Payment_Id)
                                                                                        .TransactionAmount;
                                }
                                else if (thisRemitaPayment != null)
                                {
                                    feesPaymentReport.AcceptanceTransactionAmount = "N" +
                                                                                    remitaPaymentLogic.GetModelBy(
                                                                                        p =>
                                                                                            p.Payment_Id ==
                                                                                            confirmedPayment.Payment_Id)
                                                                                        .TransactionAmount;
                                }
                            }
                            else if (confirmedPayment.Fee_Type_Id == 3 && confirmedPayment.Session_Id == session.Id)
                            {
                                if (studentLevel.Level_Id == 1 || studentLevel.Level_Id == 3)
                                {
                                    feesPaymentReport.FirstYearSchoolFeesInvoiceNumber = confirmedPayment.Invoice_Number;
                                    if (thisPaymentEtranzact != null)
                                    {
                                        feesPaymentReport.FirstYearFeesTransactionAmount = "N" +
                                                                                           paymentEtranzactLogic
                                                                                               .GetModelBy(
                                                                                                   p =>
                                                                                                       p.Payment_Id ==
                                                                                                       confirmedPayment
                                                                                                           .Payment_Id)
                                                                                               .TransactionAmount;
                                    }
                                    else if (thisRemitaPayment != null)
                                    {
                                        feesPaymentReport.FirstYearFeesTransactionAmount = "N" +
                                                                                           remitaPaymentLogic.GetModelBy
                                                                                               (p =>
                                                                                                   p.Payment_Id ==
                                                                                                   confirmedPayment
                                                                                                       .Payment_Id)
                                                                                               .TransactionAmount;
                                    }
                                }
                                if (studentLevel.Level_Id == 2 || studentLevel.Level_Id == 4)
                                {
                                    feesPaymentReport.SecondYearSchoolFeesInvoiceNumber =
                                        confirmedPayment.Invoice_Number;
                                    if (thisPaymentEtranzact != null)
                                    {
                                        feesPaymentReport.SecondYearFeesTransactionAmount = "N" +
                                                                                            paymentEtranzactLogic
                                                                                                .GetModelBy(
                                                                                                    p =>
                                                                                                        p.Payment_Id ==
                                                                                                        confirmedPayment
                                                                                                            .Payment_Id)
                                                                                                .TransactionAmount;
                                    }
                                    else if (thisRemitaPayment != null)
                                    {
                                        feesPaymentReport.SecondYearFeesTransactionAmount = "N" +
                                                                                            remitaPaymentLogic
                                                                                                .GetModelBy(
                                                                                                    p =>
                                                                                                        p.Payment_Id ==
                                                                                                        confirmedPayment
                                                                                                            .Payment_Id)
                                                                                                .TransactionAmount;
                                    }
                                }
                            }
                            else if (confirmedPayment.Fee_Type_Id == 3 && confirmedPayment.Session_Id != session.Id)
                            {
                                feesPaymentReport.FirstYearSchoolFeesInvoiceNumber = confirmedPayment.Invoice_Number;
                                if (thisPaymentEtranzact != null)
                                {
                                    feesPaymentReport.FirstYearFeesTransactionAmount = "N" +
                                                                                       paymentEtranzactLogic.GetModelBy(
                                                                                           p =>
                                                                                               p.Payment_Id ==
                                                                                               confirmedPayment
                                                                                                   .Payment_Id)
                                                                                           .TransactionAmount;
                                }
                                else if (thisRemitaPayment != null)
                                {
                                    feesPaymentReport.FirstYearFeesTransactionAmount = "N" +
                                                                                       remitaPaymentLogic.GetModelBy(
                                                                                           p =>
                                                                                               p.Payment_Id ==
                                                                                               confirmedPayment
                                                                                                   .Payment_Id)
                                                                                           .TransactionAmount;
                                }
                            }
                        }


                        feesPaymentReport.Department = department.Name;
                        feesPaymentReport.Level = level.Name;
                        feesPaymentReport.MatricNumber = studentLevel.STUDENT.Matric_Number;
                        if (studentLevel.STUDENT.APPLICATION_FORM != null)
                        {
                            feesPaymentReport.ApplicationNumber =
                                studentLevel.STUDENT.APPLICATION_FORM.Application_Form_Number;
                        }
                        feesPaymentReport.Programme = programme.Name;
                        feesPaymentReport.Session = session.Name;
                        feesPaymentReport.Name = studentLevel.STUDENT.PERSON.Last_Name + " " +
                                                 studentLevel.STUDENT.PERSON.First_Name;

                        feesPaymentReportList.Add(feesPaymentReport);
                    }
                }
            }

            catch (Exception)
            {
                throw;
            }

            return feesPaymentReportList.OrderBy(p => p.Name).ToList();
        }

        public List<FeesPaymentReport> GetAllFeesPaymentBy(Session session, Programme programme, Department department,
            Level level)
        {
            var feesPaymentReportList = new List<FeesPaymentReport>();

            try
            {
                if (session != null && programme != null && department != null && level != null)
                {
                    if (level.Id == 1 || level.Id == 3)
                    {
                        List<FeesPaymentReport> paymentReport =
                            (from a in
                                 repository.GetBy<VW_ALL_PAYMENT_REPORT_NEW_STUDENTS>(
                                     a =>
                                         a.Department_Id == department.Id && a.Level_Id == level.Id &&
                                         a.Programme_Id == programme.Id && a.Session_Id == session.Id)
                             select new FeesPaymentReport
                             {
                                 Name = a.Last_Name + " " + a.First_Name + " " + a.Other_Name,
                                 ApplicationNumber = a.Application_Form_Number,
                                 MatricNumber = a.Matric_Number,
                                 AcceptanceFeeInvoiceNumber = a.ACCEPTANCE_FEE_INVOICE,
                                 AcceptanceTransactionAmount = a.ACCEPTANCE_FEE_AMOUNT.ToString(),
                                 FirstYearSchoolFeesInvoiceNumber = a.FIRST_YEAR_FEE_INVOICE,
                                 FirstYearFeesTransactionAmount = a.FIRST_YEAR_SCHOOL_FEE_AMOUNT.ToString(),
                                 SecondYearSchoolFeesInvoiceNumber = a.SECOND_YEAR_FEE_INVOICE,
                                 SecondYearFeesTransactionAmount = a.SECOND_YEAR_SCHOOL_FEE_AMOUNT.ToString(),
                                 Session = a.Session_Name,
                                 Programme = a.Programme_Name,
                                 Department = a.Department_Name,
                                 Level = a.Level_Name,
                             }).ToList();
                        feesPaymentReportList = paymentReport;
                    }
                    else
                    {
                        feesPaymentReportList = GetFeesPaymentBy(session, programme, department, level);
                    }
                }
            }

            catch (Exception)
            {
                throw;
            }
            return feesPaymentReportList.OrderBy(p => p.Name).ToList();
        }

        public Payment Modify(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Payment_Id == payment.Id;
                PAYMENT paymentEntity = GetEntityBy(selector);

                if (paymentEntity == null || paymentEntity.Payment_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }

                PaymentVerificationLogic paymentVerificationLogic = new PaymentVerificationLogic();
                PaymentVerification paymentVerification = paymentVerificationLogic.GetBy(payment.Id);

                if (paymentVerification == null)
                {
                    paymentEntity.Payment_Serial_Number = payment.SerialNumber;
                    paymentEntity.Invoice_Number = payment.InvoiceNumber;
                    paymentEntity.Date_Paid = payment.DatePaid;

                    if (payment.PaymentMode != null)
                    {
                        paymentEntity.Payment_Mode_Id = payment.PaymentMode.Id;
                    }
                    if (payment.Person != null)
                    {
                        paymentEntity.Person_Id = payment.Person.Id;
                    }
                    if (payment.PaymentType != null)
                    {
                        paymentEntity.Payment_Type_Id = payment.PaymentType.Id;
                    }
                    if (payment.PersonType != null)
                    {
                        paymentEntity.Person_Type_Id = payment.PersonType.Id;
                    }
                    if (payment.FeeType != null)
                    {
                        paymentEntity.Fee_Type_Id = payment.FeeType.Id;
                    }
                    if (payment.Session != null)
                    {
                        paymentEntity.Session_Id = payment.Session.Id;
                    }

                    int modifiedRecordCount = Save();

                    payment = GetModelBy(p => p.Payment_Id == payment.Id);
                }
              
                return payment;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Payment> ModifyAsync(Payment payment)
        {
            try
            {
                Expression<Func<PAYMENT, bool>> selector = p => p.Payment_Id == payment.Id;
                PAYMENT paymentEntity = await GetEntityByAsync(selector);

                if (paymentEntity == null || paymentEntity.Payment_Id <= 0)
                {
                    throw new Exception(NoItemFound);
                }
                PaymentVerificationLogic paymentVerificationLogic = new PaymentVerificationLogic();
                PaymentVerification paymentVerification = await paymentVerificationLogic.GetByAsync(payment.Id);

                if (paymentVerification == null)
                {
                    paymentEntity.Payment_Serial_Number = payment.SerialNumber;
                    paymentEntity.Invoice_Number = payment.InvoiceNumber;
                    paymentEntity.Date_Paid = payment.DatePaid;

                    if (payment.PaymentMode != null)
                    {
                        paymentEntity.Payment_Mode_Id = payment.PaymentMode.Id;
                    }
                    if (payment.Person != null)
                    {
                        paymentEntity.Person_Id = payment.Person.Id;
                    }
                    if (payment.PaymentType != null)
                    {
                        paymentEntity.Payment_Type_Id = payment.PaymentType.Id;
                    }
                    if (payment.PersonType != null)
                    {
                        paymentEntity.Person_Type_Id = payment.PersonType.Id;
                    }
                    if (payment.FeeType != null)
                    {
                        paymentEntity.Fee_Type_Id = payment.FeeType.Id;
                    }
                    if (payment.Session != null)
                    {
                        paymentEntity.Session_Id = payment.Session.Id;
                    }

                    int modifiedRecordCount = await SaveAsync();

                    payment = await GetModelByAsync(p => p.Payment_Id == payment.Id);
                }
               
                return payment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Payment GetFirstInstallment(Payment payment)
        {
            try
            {
                return
                    GetModelBy(
                        p =>
                            p.Person_Id == payment.Person.Id && p.Session_Id == payment.Session.Id &&
                            p.Payment_Mode_Id == 2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Payment GetSecondInstallment(Payment payment)
        {
            try
            {
                return
                    GetModelBy(
                        p =>
                            p.Person_Id == payment.Person.Id && p.Session_Id == payment.Session.Id &&
                            p.Payment_Mode_Id == 3);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DateTime ConvertToDate(string date)
        {
            DateTime newDate = new DateTime();
            try
            {
                string[] dateSplit = date.Split('/');
                newDate = new DateTime(Convert.ToInt32(dateSplit[2]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[0]));
            }
            catch (Exception)
            {
                throw;
            }

            return newDate;
        }
        public List<AllPaymentGeneralView> GetPaymentReportBy(FeeType FeeType, Department department, Programme programme, string dateTo, string dateFrom)
        {
            try
            {
                DateTime paymentFrom = ConvertToDate(dateFrom);
                DateTime paymentTo = ConvertToDate(dateTo);
                if (department.Id == 1001)
                {
                    List<AllPaymentGeneralView> payments =
                    (from p in
                         repository.GetBy<VW_ALL_PAYMENT_REPORT_GENERAL>(
                             p =>
                                  p.Programme_Id == programme.Id &&
                                 p.Fee_Type_Id == FeeType.Id && (p.Date_Paid >= paymentFrom && p.Date_Paid <= paymentTo))

                     select new AllPaymentGeneralView
                     {
                         Student_Name = p.Student_Name,
                         Department = p.Department,
                         DepartmentId = department.Id,
                         FeeTypes = p.Payment_Type,
                         FeeTypesId = p.Fee_Type_Id,
                         Programme = p.Programme,
                         ProgrammeId = p.Programme_Id,
                         level = p.Level,
                         Amount = p.Amount,
                         Session = p.Session_Id,
                         Date = p.Date_Paid,
                         StartDate = paymentFrom.ToShortDateString(),
                         EndDate = paymentTo.ToShortDateString(),

                     }).ToList();
                    return payments;
                }
                else if (programme.Id == 1001)
                {
                    List<AllPaymentGeneralView> payments =
                    (from p in
                         repository.GetBy<VW_ALL_PAYMENT_REPORT_GENERAL>(
                             p =>
                                 p.Department_Id == department.Id &&
                                 p.Fee_Type_Id == FeeType.Id && (p.Date_Paid >= paymentFrom && p.Date_Paid <= paymentTo))

                     select new AllPaymentGeneralView
                     {
                         Student_Name = p.Student_Name,
                         Department = p.Department,
                         DepartmentId = p.Department_Id,
                         FeeTypes = p.Payment_Type,
                         FeeTypesId = p.Fee_Type_Id,
                         Programme = p.Programme,
                         ProgrammeId = programme.Id,
                         level = p.Level,
                         Amount = p.Amount,
                         Session = p.Session_Id,
                         Date = p.Date_Paid,
                         StartDate = paymentFrom.ToShortDateString(),
                         EndDate = paymentTo.ToShortDateString(),

                     }).ToList();
                    return payments;
                }
                else if (FeeType.Id == 1001)
                {
                    List<AllPaymentGeneralView> payments =
                    (from p in
                         repository.GetBy<VW_ALL_PAYMENT_REPORT_GENERAL>(
                             p =>
                                 p.Department_Id == department.Id && p.Programme_Id == programme.Id && (p.Date_Paid >= paymentFrom && p.Date_Paid <= paymentTo))

                     select new AllPaymentGeneralView
                     {
                         Student_Name = p.Student_Name,
                         Department = p.Department,
                         DepartmentId = p.Department_Id,
                         FeeTypes = p.Payment_Type,
                         FeeTypesId = FeeType.Id,
                         Programme = p.Programme,
                         ProgrammeId = p.Programme_Id,
                         level = p.Level,
                         Amount = p.Amount,
                         Session = p.Session_Id,
                         Date = p.Date_Paid,
                         StartDate = paymentFrom.ToShortDateString(),
                         EndDate = paymentTo.ToShortDateString(),

                     }).ToList();
                    return payments;
                }
                else if (programme.Id == 1001 && department.Id == 1001)
                {
                    List<AllPaymentGeneralView> payments =
                    (from p in
                         repository.GetBy<VW_ALL_PAYMENT_REPORT_GENERAL>(
                             p =>
                                 p.Fee_Type_Id == FeeType.Id && (p.Date_Paid >= paymentFrom && p.Date_Paid <= paymentTo))

                     select new AllPaymentGeneralView
                     {
                         Student_Name = p.Student_Name,
                         Department = p.Department,
                         DepartmentId = department.Id,
                         FeeTypes = p.Payment_Type,
                         FeeTypesId = p.Fee_Type_Id,
                         Programme = p.Programme,
                         ProgrammeId = programme.Id,
                         level = p.Level,
                         Amount = p.Amount,
                         Session = p.Session_Id,
                         Date = p.Date_Paid,
                         StartDate = paymentFrom.ToShortDateString(),
                         EndDate = paymentTo.ToShortDateString(),

                     }).ToList();
                    return payments;

                }
                else if (FeeType.Id == 1001 && department.Id == 1001)
                {
                    List<AllPaymentGeneralView> payments =
                    (from p in
                         repository.GetBy<VW_ALL_PAYMENT_REPORT_GENERAL>(
                             p =>
                                  p.Programme_Id == programme.Id && (p.Date_Paid >= paymentFrom && p.Date_Paid <= paymentTo))

                     select new AllPaymentGeneralView
                     {
                         Student_Name = p.Student_Name,
                         Department = p.Department,
                         DepartmentId = department.Id,
                         FeeTypes = p.Payment_Type,
                         FeeTypesId = p.Fee_Type_Id,
                         Programme = p.Programme,
                         ProgrammeId = p.Programme_Id,
                         level = p.Level,
                         Amount = p.Amount,
                         Session = p.Session_Id,
                         Date = p.Date_Paid,
                         StartDate = paymentFrom.ToShortDateString(),
                         EndDate = paymentTo.ToShortDateString(),

                     }).ToList();
                    return payments;
                }
                else if (programme.Id == 1001 && FeeType.Id == 1001)
                {
                    List<AllPaymentGeneralView> payments =
                    (from p in
                         repository.GetBy<VW_ALL_PAYMENT_REPORT_GENERAL>(
                             p =>
                                 p.Department_Id == department.Id && (p.Date_Paid >= paymentFrom && p.Date_Paid <= paymentTo))

                     select new AllPaymentGeneralView
                     {
                         Student_Name = p.Student_Name,
                         Department = p.Department,
                         DepartmentId = p.Department_Id,
                         FeeTypes = p.Payment_Type,
                         FeeTypesId = p.Fee_Type_Id,
                         Programme = p.Programme,
                         ProgrammeId = p.Programme_Id,
                         level = p.Level,
                         Amount = p.Amount,
                         Session = p.Session_Id,
                         Date = p.Date_Paid,
                         StartDate = paymentFrom.ToShortDateString(),
                         EndDate = paymentTo.ToShortDateString(),

                     }).ToList();
                    return payments;
                }
                else if (FeeType.Id == 1001 && department.Id == 1001 && programme.Id == 1001)
                {
                    List<AllPaymentGeneralView> payments =
                    (from p in
                         repository.GetBy<VW_ALL_PAYMENT_REPORT_GENERAL>(
                             p => (p.Date_Paid >= paymentFrom && p.Date_Paid <= paymentTo))

                     select new AllPaymentGeneralView
                     {
                         Student_Name = p.Student_Name,
                         Department = p.Department,
                         DepartmentId = department.Id,
                         FeeTypes = p.Payment_Type,
                         FeeTypesId = p.Fee_Type_Id,
                         Programme = p.Programme,
                         ProgrammeId = p.Programme_Id,
                         level = p.Level,
                         Amount = p.Amount,
                         Session = p.Session_Id,
                         Date = p.Date_Paid,
                         StartDate = paymentFrom.ToShortDateString(),
                         EndDate = paymentTo.ToShortDateString(),

                     }).ToList();
                    return payments;
                }
                else
                {
                    List<AllPaymentGeneralView> payments =
                    (from p in
                         repository.GetBy<VW_ALL_PAYMENT_REPORT_GENERAL>(
                             p =>
                                 p.Department_Id == department.Id && p.Programme_Id == programme.Id &&
                                 p.Fee_Type_Id == FeeType.Id && (p.Date_Paid >= paymentFrom && p.Date_Paid <= paymentTo))

                     select new AllPaymentGeneralView
                     {
                         Student_Name = p.Student_Name,
                         Department = p.Department,
                         DepartmentId = p.Department_Id,
                         FeeTypes = p.Payment_Type,
                         FeeTypesId = p.Fee_Type_Id,
                         Programme = p.Programme,
                         ProgrammeId = p.Programme_Id,
                         level = p.Level,
                         Amount = p.Amount,
                         Session = p.Session_Id,
                         Date = p.Date_Paid,
                         StartDate = paymentFrom.ToShortDateString(),
                         EndDate = paymentTo.ToShortDateString(),

                     }).ToList();
                    return payments;
                }

            }

            catch (Exception)
            {
                throw;
            }
        }

        public List<AllPaymentGeneralView> GetschoolFeeBy(Department department, Programme programme, Level level)
        {
            try
            {
                List<AllPaymentGeneralView> payment =
               (from x in repository.GetBy<VW_ALL_PAYMENT_REPORT_GENERAL>
                    (x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && x.Level_Id == level.Id
                    )
                select new AllPaymentGeneralView
                {
                    Student_Name = x.Student_Name,
                    Department = x.Department,
                    DepartmentId = x.Department_Id,
                    FeeTypes = x.Payment_Type,
                    FeeTypesId = x.Fee_Type_Id,
                    Programme = x.Programme,
                    ProgrammeId = x.Programme_Id,
                    level = x.Level,
                    Amount = x.Amount,
                    Session = x.Session_Id,
                    Date = x.Date_Paid,

                }).OrderBy(c => c.level).ToList();

                return payment;
            }
            catch (Exception)
            {

                throw;
            }


        }

        //public List<FeesReportFormat> GetFeesReport(Department department,Programme programme,Level level)
        //{
        //    try
        //    {
        //        List<FeesReportFormat> payments =
        //            (from p in repository.GetBy<VW_SCHOOL_FEE_REPORT>(p => p.Department_Id == department.Id && p.Programme_Id == programme.Id && p.Level_Id == level.Id)
        //             select new FeesReportFormat
        //             {
        //                 Name = p.Last_Name + " " + p.First_Name + " " + p.Other_Name,
        //                 MatricNumber = p.Matric_Number,
        //                 Department = p.Department_Name,
        //                 Programme = p.Programme_Name,
        //                 Level = p.Last_Name,
        //                 Amount = p.Transaction_Amount.ToString()
        //             }).ToList();
        //        return payments;
        //    }
        //    catch(Exception)
        //    {
        //        throw;
        //    }
        //}


        public List<PaymentView> GetPaystackPaymentBy(Person person)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_PAYMENT_PAYSTACK>(p => p.Person_Id == person.Id)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ConfirmationOrderNumber = p.reference,
                                                  BankCode = p.card_type,
                                                  BankName = p.bank,
                                                  BranchCode = p.brand,
                                                  PaymentDate = p.transaction_date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  PaymentTypeName = p.Payment_Type_Name,
                                                  Amount = p.Amount,
                                                  FormatedAmount = String.Format("{0:N}", p.Amount),
                                                  SessionName = p.Session_Name
                                              }).ToList();
                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PaymentView> GetMonnifyPaymentBy(Person person)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_MONNIFY_PAYMENT>(p => p.Person_Id == person.Id)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ConfirmationOrderNumber = p.Account_Name,
                                                 
                                                  BankName = p.Account_Bank_Name,
                                                  AccountNumber=p.Account_Number,
                                                  PaymentDate = p.Completed_Date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  PaymentTypeName = p.Payment_Type_Name,
                                                  Amount = p.Amount,
                                                  FormatedAmount = String.Format("{0:N}", p.Amount),
                                                  SessionName = p.Session_Name
                                              }).ToList();
                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PaymentView> GetManualPaymentBy(Person person)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<MANUAL_PAYMENT>(p => p.Person_Id == person.Id)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = repository.GetSingleBy<PAYMENT>(s => s.Invoice_Number == p.Invoice_Number).Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ConfirmationOrderNumber = p.Invoice_Number,
                                                  BankCode = "0",
                                                  BankName = "MANUAL",
                                                  BranchCode = "MANUAL",
                                                  PaymentDate = p.Date_Approved,
                                                  FeeTypeId = p.FeeType_Id,
                                                  FeeTypeName = p.FEE_TYPE.Fee_Type_Name,
                                                  PaymentTypeId = 1,
                                                  PaymentTypeName = "Manual Payment",
                                                  Amount = p.Amount,
                                                  FormatedAmount = String.Format("{0:N}", p.Amount),
                                                  SessionName = p.SESSION.Session_Name
                                              }).ToList();
                return payments;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<PaymentView> GetInterswitchPaymentBy(Person person)
        {
            try
            {
                List<PaymentView> payments = (from sr in repository.GetBy<VW_PAYMENT_INTERSWITCH>(p => p.Person_Id == person.Id && p.ResponseCode == "00")
                                              select new PaymentView
                                              {
                                                  InvoiceNumber = sr.Invoice_Number,
                                                  PaymentModeId = sr.Payment_Mode_Id,
                                                  PaymentId =sr.Payment_Id,
                                                  FeeTypeId = sr.Fee_Type_Id,
                                                  SessionId = (int)sr.Session_Id,
                                                  PersonId = sr.Person_Id,
                                                  FeeTypeName = sr.Fee_Type_Name,
                                                  FormatedAmount = String.Format("{0:N}", sr.Amount),
                                                  Amount =  sr.Amount,
                                                  SessionName = sr.Session_Name,
                                                  MatricNumber = sr.Matric_NO,
                                                  LevelName = sr.Level_Name,
                                                  DepartmentName = sr.Department_Name,
                                                  ProgrammeName = sr.Programme_Name,
                                                  ConfirmationOrderNumber = sr.MerchantReference,
                                                  TransactionDate = sr.TransactionDate.ToString(),
                                                  PaymentDate = sr.Date_Paid,
                                              }).ToList();

                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<AcceptanceView> GetAcceptanceReport(Session session, Department department, Programme programme, string dateFrom, string dateTo)
        {
            try
            {
                List<AcceptanceView> payments = new List<AcceptanceView>();

                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime applicationFrom = ConvertToDate(dateFrom);
                    DateTime applicationTo = ConvertToDate(dateTo);

                    payments = (from p in repository.GetBy<VW_ACCEPTANCE_REPORT>(p => p.Department_Id == department.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id && (p.Transaction_Date >= applicationFrom && p.Transaction_Date <= applicationTo))
                                select new AcceptanceView
                                {
                                    Person_Id = p.Person_Id,
                                    Application_Exam_Number = p.Application_Exam_Number,
                                    Invoice_Number = p.Invoice_Number,
                                    Application_Form_Number = p.Application_Form_Number,
                                    First_Choice_Department_Name = p.Department_Name,
                                    Name = p.SURNAME + " " + p.FIRSTNAME + " " + p.OTHER_NAMES,
                                    RRR = p.Invoice_Number,
                                    Programme_Name = p.Programme_Name
                                }).OrderBy(b => b.Name).ToList();

                }
                else
                {
                    payments = (from p in repository.GetBy<VW_ACCEPTANCE_REPORT>(p => p.Department_Id == department.Id && p.Programme_Id == programme.Id && p.Session_Id == session.Id)
                                select new AcceptanceView
                                {
                                    Person_Id = p.Person_Id,
                                    Application_Exam_Number = p.Application_Exam_Number,
                                    Invoice_Number = p.Invoice_Number,
                                    Application_Form_Number = p.Application_Form_Number,
                                    First_Choice_Department_Name = p.Department_Name,
                                    Name = p.SURNAME + " " + p.FIRSTNAME + " " + p.OTHER_NAMES,
                                    RRR = p.Invoice_Number,
                                    Programme_Name = p.Programme_Name
                                }).OrderBy(b => b.Name).ToList();

                }

                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AcceptanceView> GetAcceptanceCount(Session session, string dateFrom, string dateTo)
        {
            try
            {
                List<AcceptanceView> payments = new List<AcceptanceView>();

                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    DateTime applicationFrom = ConvertToDate(dateFrom);
                    DateTime applicationTo = ConvertToDate(dateTo);

                    payments = (from p in repository.GetBy<VW_ACCEPTANCE_REPORT>(p => p.Session_Id == session.Id && (p.Transaction_Date >= applicationFrom && p.Transaction_Date <= applicationTo))
                                select new AcceptanceView
                                {
                                    Person_Id = p.Person_Id,
                                    Application_Exam_Number = p.Application_Exam_Number,
                                    Invoice_Number = p.Invoice_Number,
                                    Application_Form_Number = p.Application_Form_Number,
                                    First_Choice_Department_Name = p.Department_Name,
                                    Name = p.SURNAME + " " + p.FIRSTNAME + " " + p.OTHER_NAMES,
                                    RRR = p.Invoice_Number,
                                    Programme_Name = p.Programme_Name,
                                    Count = 1
                                }).OrderBy(b => b.Name).ToList();

                }
                else
                {
                    payments = (from p in repository.GetBy<VW_ACCEPTANCE_REPORT>(p => p.Session_Id == session.Id)
                                select new AcceptanceView
                                {
                                    Person_Id = p.Person_Id,
                                    Application_Exam_Number = p.Application_Exam_Number,
                                    Invoice_Number = p.Invoice_Number,
                                    Application_Form_Number = p.Application_Form_Number,
                                    First_Choice_Department_Name = p.Department_Name,
                                    Name = p.SURNAME + " " + p.FIRSTNAME + " " + p.OTHER_NAMES,
                                    RRR = p.Invoice_Number,
                                    Programme_Name = p.Programme_Name,
                                    Count = 1
                                }).OrderBy(b => b.Name).ToList();

                }

                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ClassAdmitCard> GetClassAdmitCard(Department department, Programme programme, Level level, Session session)
        {
            try
            {
                IEnumerable<ClassAdmitCard> cards =
               (from x in repository.GetBy<VW_SCHOOL_FEES_PAYMENT>
                    (x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && x.Level_Id == level.Id && x.Session_Id == session.Id
                    )
                select new ClassAdmitCard
                {
                    Fullname = x.FULLNAME,
                    Department = x.Department_Name,
                    Programme = x.Programme_Name,
                    Matric_Number = x.Matric_Number,
                    Level = x.Level_Name,
                    Session = x.Session_Name,
                    ConfirmationNumber = x.Confirmation_No,
                    passport_url = x.Image_File_Url,
                    RegisteredCourseId = x.Student_Course_Registration_Id

                }).OrderBy(c => c.Level).ToList();

                CourseRegistrationDetailLogic registrationDetailLogic = new CourseRegistrationDetailLogic();
                List<ClassAdmitCard> admitCard = new List<ClassAdmitCard>();
                foreach (var card in cards)
                {
                    card.RegisteredCourses = registrationDetailLogic.GetRegisteredCourses(card.RegisteredCourseId);
                    if (!String.IsNullOrEmpty(card.RegisteredCourses))
                    {
                        admitCard.Add(card);
                    }
                }
                return admitCard;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public List<ClassAdmitCard> GetClassAdmitCardWithoutCourses(Department department, Programme programme, Level level, Session session)
        {
            try
            {
                IEnumerable<ClassAdmitCard> cards =
               (from x in repository.GetBy<VW_SCHOOL_FEES_PAYMENT>
                    (x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && x.Level_Id == level.Id && x.Session_Id == session.Id
                    )
                select new ClassAdmitCard
                {
                    Fullname = x.FULLNAME,
                    Department = x.Department_Name,
                    Programme = x.Programme_Name,
                    Matric_Number = x.Matric_Number,
                    Level = x.Level_Name,
                    Session = x.Session_Name,
                    ConfirmationNumber = x.Confirmation_No,
                    passport_url = x.Image_File_Url,
                    RegisteredCourseId = x.Student_Course_Registration_Id

                }).OrderBy(c => c.Level).ToList();

               
                return cards.ToList();
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<List<ClassAdmitCard>> GetClassAdmitCardAsync(Department department, Programme programme, Level level, Session session)
        {
            try
            {
                IEnumerable<ClassAdmitCard> cards =
              (from x in await repository.GetByAsync<VW_SCHOOL_FEES_PAYMENT>
                    (x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && x.Level_Id == level.Id && x.Session_Id == session.Id
                    )
                select new ClassAdmitCard
                {
                    Fullname = x.FULLNAME,
                    Department = x.Department_Name,
                    Programme = x.Programme_Name,
                    Matric_Number = x.Matric_Number,
                    Level = x.Level_Name,
                    Session = x.Session_Name,
                    ConfirmationNumber = x.Confirmation_No,
                    passport_url = x.Image_File_Url,
                    RegisteredCourseId = x.Student_Course_Registration_Id

                }).OrderBy(c => c.Level).ToList();

                CourseRegistrationDetailLogic registrationDetailLogic = new CourseRegistrationDetailLogic();
                List<ClassAdmitCard> admitCard = new List<ClassAdmitCard>();
                foreach (var card in cards)
                {
                    card.RegisteredCourses = registrationDetailLogic.GetRegisteredCourses(card.RegisteredCourseId);
                    if (!String.IsNullOrEmpty(card.RegisteredCourses))
                    {
                        admitCard.Add(card);
                    }
                }
                return admitCard;
            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task<List<ClassAdmitCard>> GetClassAdmitCardWithoutCoursesAsync(Department department, Programme programme, Level level, Session session)
        {
            try
            {
                IEnumerable<ClassAdmitCard> cards =
               (from x in await repository.GetByAsync<VW_SCHOOL_FEES_PAYMENT>
                    (x => x.Department_Id == department.Id && x.Programme_Id == programme.Id && x.Level_Id == level.Id && x.Session_Id == session.Id
                    )
                select new ClassAdmitCard
                {
                    Fullname = x.FULLNAME,
                    Department = x.Department_Name,
                    Programme = x.Programme_Name,
                    Matric_Number = x.Matric_Number,
                    Level = x.Level_Name,
                    Session = x.Session_Name,
                    ConfirmationNumber = x.Confirmation_No,
                    passport_url = x.Image_File_Url,
                    RegisteredCourseId = x.Student_Course_Registration_Id

                }).OrderBy(c => c.Level).ToList();


                return cards.ToList();
            }
            catch (Exception)
            {

                throw;
            }


        }

        public List<PaymentSummary> GetPaymentSummary(DateTime dateFrom, DateTime dateTo)
        {
            List<PaymentSummary> payments;
            try
            {
                payments = (from p in repository.GetBy<VW_PAYMENT_SUMMARY>(p => p.Transaction_Date != null && p.Transaction_Date >= dateFrom && p.Transaction_Date <= dateTo)
                            select new PaymentSummary
                            {
                                PersonId = p.Person_Id,
                                Name = p.Name,
                                MatricNumber = p.Matric_Number,
                                SessionId = p.Session_Id,
                                SessionName = p.Session_Name,
                                FeeTypeId = p.Fee_Type_Id,
                                FeeTypeName = p.Fee_Type_Name,
                                LevelId = p.Level_Id,
                                LevelName = p.Level_Name,
                                ProgrammeId = p.Programme_Id,
                                ProgrammeName = p.Programme_Name,
                                DepartmentId = p.Department_Id,
                                DepartmentName = p.Department_Name,
                                FacultyId = p.Faculty_Id,
                                FacultyName = p.Faculty_Name,
                                TransactionDate = p.Transaction_Date,
                                TransactionAmount = p.Transaction_Amount,
                                PaymentEtranzactId = p.Payment_Etranzact_Id,
                                InvoiceNumber = p.Invoice_Number,
                                ConfirmationNumber = p.Confirmation_Number,
                                PaymentModeId = p.Payment_Mode_Id,
                                PaymentModeName = p.Payment_Mode_Name
                            }).ToList();

            }
            catch (Exception ex)
            {
                throw;
            }

            return payments;
        }
        public List<PaymentSummary> GetPaymentSummary(DateTime dateFrom, DateTime dateTo, int departmentId, int programmeId, int paymentModeId, int feeTypeId)
        {
            List<PaymentSummary> payments;
            try
            {
                payments = (from p in repository.GetBy<VW_PAYMENT_SUMMARY>(p => p.Transaction_Date != null && p.Transaction_Date >= dateFrom && p.Transaction_Date <= dateTo)
                            select new PaymentSummary
                            {
                                PersonId = p.Person_Id,
                                Name = p.Name,
                                MatricNumber = p.Matric_Number,
                                SessionId = p.Session_Id,
                                SessionName = p.Session_Name,
                                FeeTypeId = p.Fee_Type_Id,
                                FeeTypeName = p.Fee_Type_Name,
                                LevelId = p.Level_Id,
                                LevelName = p.Level_Name,
                                ProgrammeId = p.Programme_Id,
                                ProgrammeName = p.Programme_Name,
                                DepartmentId = p.Department_Id,
                                DepartmentName = p.Department_Name,
                                FacultyId = p.Faculty_Id,
                                FacultyName = p.Faculty_Name,
                                TransactionDate = p.Transaction_Date,
                                TransactionAmount = p.Transaction_Amount,
                                PaymentEtranzactId = p.Payment_Etranzact_Id,
                                InvoiceNumber = p.Invoice_Number,
                                ConfirmationNumber = p.Confirmation_Number,
                                PaymentModeId = p.Payment_Mode_Id,
                                PaymentModeName = p.Payment_Mode_Name
                            }).ToList();

            }
            catch (Exception ex)
            {
                throw;
            }

            return payments;
        }

        public List<Receipt> GetReceiptsBy(long paymentId)
        {
            List<Receipt> receipts = new List<Receipt>();
            var paymentLogic = new PaymentLogic();
            RemitaPaymentLogic remitaPaymentLogic = new RemitaPaymentLogic();
            RemitaPayment remitaPayment = new RemitaPayment();
            PaystackLogic paystackLogic = new PaystackLogic();
            PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();
            
            StudentLevel studentLevel = new StudentLevel();
            try
            {
                Payment payment = paymentLogic.GetBy(paymentId);
                remitaPayment = remitaPaymentLogic.GetModelBy(o => o.Payment_Id == payment.Id);
                if (remitaPayment != null)
                {
                    var matricNumber = "";
                    var level = "";
                    var department = "";
                    var programme = "";
                    var faculty = "";
                    StudentLogic studentLogic = new StudentLogic();

                    Model.Model.Student student = studentLogic.GetBy(payment.Person.Id);

                    if (student != null)
                    {
                        matricNumber = student.MatricNumber;
                        StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                        StudentPayment studentPayment = studentPaymentLogic.GetModelBy(a => a.Payment_Id == payment.Id);
                        if (studentPayment != null)
                        {
                            level = studentPayment.Level.Name;
                        }
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        studentLevel = studentLevelLogic.GetBy(student.Id);
                        if (studentLevel != null)
                        {
                            department = studentLevel.Department.Name;
                            programme = studentLevel.Programme.Name;
                            faculty = studentLevel.Department.Faculty.Name;
                        }
                        else
                        {
                            AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                            AdmissionList admissionList = admissionListLogic.GetBy(remitaPayment.payment.Person);
                            if (admissionList != null)
                            {
                                department = admissionList.Deprtment.Name;
                                programme = admissionList.Programme.Name;
                                faculty = admissionList.Deprtment.Faculty.Name;
                            }
                        }
                    }
                    else
                    {
                        AdmissionListLogic admissionListLogic = new AdmissionListLogic();
                        AdmissionList admissionList = admissionListLogic.GetBy(remitaPayment.payment.Person);
                        if (admissionList != null)
                        {
                            department = admissionList.Deprtment.Name;
                            programme = admissionList.Programme.Name;
                            faculty = admissionList.Deprtment.Faculty.Name;
                        }
                    }
                    var amount = (decimal)remitaPayment.TransactionAmount;
                   Receipt receipt = BuildReceipt(payment.Person.FullName, payment.InvoiceNumber, remitaPayment, amount, payment.FeeType.Name, matricNumber, "", payment.Session.Name, level, department, programme, faculty);
                   receipts.Add(receipt);
                }

            }
            catch (Exception)
            {
                    
                throw;
            }
            return receipts;
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, RemitaPayment remitaPayment, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();

                ShortFallLogic shortFallLogic = new ShortFallLogic();
                ShortFall shortFall = shortFallLogic.GetModelsBy(s => s.PAYMENT.Invoice_Number == invoiceNumber).LastOrDefault();
                if (shortFall != null && !string.IsNullOrEmpty(shortFall.Description))
                {
                    receipt.Description = shortFall.Description;
                }

                receipt.Number = remitaPayment.Receipt_No;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = remitaPayment.RRR;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = remitaPayment.payment.PaymentMode.Name;
                receipt.Date = (DateTime)remitaPayment.TransactionDate;
                receipt.QRVerification = QRVerificationUrl + remitaPayment.payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = remitaPayment.payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, Paystack paystack, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();
                receipt.Number = paystack.reference;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paystack.reference;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = paystack.Payment.PaymentMode.Name;
                receipt.Date = (DateTime)paystack.transaction_date;
                receipt.QRVerification = QRVerificationUrl + paystack.Payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = paystack.Payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, PaymentInterswitch paymentInterswitch, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {

            try
            {
                var receipt = new Receipt();
                receipt.Number = paymentInterswitch.RetrievalReferenceNumber;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paymentInterswitch.MerchantReference;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = paymentInterswitch.Payment.PaymentMode.Name;
                receipt.Date = (DateTime)paymentInterswitch.TransactionDate;
                receipt.QRVerification = QRVerificationUrl + paymentInterswitch.Payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = paymentInterswitch.Payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                receipt.PaymentId = paymentInterswitch.Payment.Id.ToString();
                //receipt.PaymentId = Utility.Encrypt(paymentInterswitch.Payment.Id.ToString());
                receipt.IsInterswitchPayment = true;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Receipt BuildReceipt(string name, string invoiceNumber, PaymentMonnify paymentMonnify, decimal amount, string purpose, string MatricNumber, string ApplicationFormNumber, string session, string level, string department, string programme, string faculty)
        {
            try
            {
                var receipt = new Receipt();
                receipt.Number = paymentMonnify.TransactionReference;
                receipt.Name = name;
                receipt.ConfirmationOrderNumber = paymentMonnify.Payment.InvoiceNumber;
                receipt.Amount = amount;
                receipt.AmountInWords = NumberToWords((int)amount);
                receipt.Purpose = purpose;
                receipt.PaymentMode = paymentMonnify.Payment.PaymentMode.Name;
                receipt.Date = (DateTime)paymentMonnify.DateCreated;
                receipt.QRVerification = QRVerificationUrl + paymentMonnify.Payment.Id;
                receipt.MatricNumber = MatricNumber;
                receipt.Session = session;
                receipt.Level = level;
                receipt.ReceiptNumber = paymentMonnify.Payment.SerialNumber.ToString();
                receipt.Department = department;
                receipt.Programme = programme;
                receipt.Faculty = faculty;
                return receipt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
        public PaymentCountAmount GetPaymentCount(DateTime start, DateTime end)
        {
            
            try
            {
                PaymentCountAmount paymentCountAmount = new PaymentCountAmount();
                decimal? sum = 0;
                int count = 0;
                PaystackLogic paystackLogic = new PaystackLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                //Paystack payment
                var paystackPayments=paystackLogic.GetModelsBy(x => x.transaction_date >= start && x.transaction_date<=end && x.status == "success").ToList();
                sum+= paystackPayments.Sum(x => x.amount);
                count += paystackPayments.Count;
                //Etranzact payment
                var etranzactPayments=paymentEtranzactLogic.GetModelsBy(x => x.Transaction_Date>= start && x.Transaction_Date<=end).ToList();
                var etranzactAmount = etranzactPayments.Sum(x => x.TransactionAmount);
                sum += etranzactPayments.Sum(x => x.TransactionAmount);
                count += etranzactPayments.Count;
                paymentCountAmount.Amount = sum;
                paymentCountAmount.Count = count;
                return paymentCountAmount;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public List<BankPaymentCount> GetPaymentCountByBank(DateTime start, DateTime end)
        {

            try
            {
                List<BankPaymentCount> banks = new List<BankPaymentCount>();
                PaystackLogic paystackLogic = new PaystackLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                var groupedByBankCodes = paymentEtranzactLogic.GetModelsBy(p => p.Transaction_Date >= start && p.Transaction_Date <= end && p.Confirmation_No != null).GroupBy(x => x.BankCode).ToList();
                if (groupedByBankCodes != null)
                {
                    foreach(var groupedByBankCode in groupedByBankCodes)
                    {
                        var paymentsByBank = (from p in repository.GetBy<VW_PAYMENTETRANZACT_BANK>(p => p.Transaction_Date >= start && p.Transaction_Date<=end && p.Bank_Code== groupedByBankCode.Key)
                                        select new BankPaymentCount
                                        {
                                            BankCode=p.Bank_Code,
                                            BankName=p.Bank_Name,

                                        }).ToList();

                        BankPaymentCount bankPaymentCount = new BankPaymentCount();
                        bankPaymentCount.BankCode = groupedByBankCode.Key;
                        bankPaymentCount.BankName = paymentsByBank.FirstOrDefault().BankName;
                        bankPaymentCount.Count = paymentsByBank.Count();

                        banks.Add(bankPaymentCount);
                    }
                }
                var paystackPayments = paystackLogic.GetModelsBy(x => x.transaction_date>=start && x.transaction_date<=end && x.status == "success").ToList();
                BankPaymentCount bankPaymentCountTwo = new BankPaymentCount();
                bankPaymentCountTwo.BankCode = "00";
                bankPaymentCountTwo.Count = paystackPayments.Count();

                banks.Add(bankPaymentCountTwo);

                return banks;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ChannelPaymentCount> GetPaymentCountByChannel(DateTime start, DateTime end)
        {
            

            try
            {
                List<ChannelPaymentCount> channels = new List<ChannelPaymentCount>();
                PaystackLogic paystackLogic = new PaystackLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

                var paystackPayments = paystackLogic.GetModelsBy(x => x.transaction_date >= start && x.transaction_date<=end && x.status == "success").ToList();
                ChannelPaymentCount payStackChannel = new ChannelPaymentCount();
                payStackChannel.Channel = "PayStack";
                payStackChannel.Count = paystackPayments.Count;
                channels.Add(payStackChannel);
                var etranzactPayments = paymentEtranzactLogic.GetModelsBy(x => x.Transaction_Date >= start && x.Transaction_Date<=end ).ToList();
                ChannelPaymentCount etranzactChannel = new ChannelPaymentCount();
                etranzactChannel.Channel = "Etranzact";
                etranzactChannel.Count = etranzactPayments.Count;
                channels.Add(etranzactChannel);
                return channels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FeeTypePaymentCount> GetPaymentCountByFeeType(DateTime start, DateTime end)
        {
            
            try
            {
                List<FeeTypePaymentCount> feeTypeList = new List<FeeTypePaymentCount>();
                PaystackLogic paystackLogic = new PaystackLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                var groupedByFeeTypes = paymentEtranzactLogic.GetModelsBy(p => p.Transaction_Date >= start && p.Transaction_Date <= end && p.Confirmation_No!=null).GroupBy(x => x.Payment.Payment.FeeType.Id).ToList();
                if (groupedByFeeTypes.Count>0)
                {
                    foreach (var groupedByFeeType in groupedByFeeTypes)
                    {
                        var paymentsByFeeType = (from p in repository.GetBy<VW_ETRANZACT_FEETYPE>(p => p.Transaction_Date >= start && p.Transaction_Date <= end && p.Fee_Type_Id == groupedByFeeType.Key)
                                              select new FeeTypePaymentCount
                                              {
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name

                                              }).ToList();

                        FeeTypePaymentCount feeTypePaymentCount = new FeeTypePaymentCount();
                        feeTypePaymentCount.FeeTypeId = groupedByFeeType.Key;
                        feeTypePaymentCount.FeeTypeName = paymentsByFeeType.FirstOrDefault().FeeTypeName;
                        feeTypePaymentCount.Count = groupedByFeeType.Count();

                        feeTypeList.Add(feeTypePaymentCount);
                    }
                }
                var groupedByPaystackFeeTypes = paystackLogic.GetModelsBy(p => p.transaction_date >= start && p.transaction_date <= end && p.status=="success").GroupBy(x => x.Payment.FeeType.Id).ToList();
                if (groupedByPaystackFeeTypes.Count>0)
                {
                    foreach (var groupedByPaystackFeeType in groupedByPaystackFeeTypes)
                    {
                        var paymentsByFeeType = (from p in repository.GetBy<VW_PAYSTACK_FEETYPE>(p => p.transaction_date >= start && p.transaction_date <= end && p.Fee_Type_Id == groupedByPaystackFeeType.Key)
                                                 select new FeeTypePaymentCount
                                                 {
                                                     FeeTypeId = p.Fee_Type_Id,
                                                     FeeTypeName = p.Fee_Type_Name

                                                 }).ToList();

                        FeeTypePaymentCount feeTypePaymentCount = new FeeTypePaymentCount();
                        feeTypePaymentCount.FeeTypeId = groupedByPaystackFeeType.Key;
                        feeTypePaymentCount.FeeTypeName = paymentsByFeeType.FirstOrDefault().FeeTypeName;
                        feeTypePaymentCount.Count = groupedByPaystackFeeType.Count();

                        feeTypeList.Add(feeTypePaymentCount);
                    }
                    
                }
                return feeTypeList;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public List<PaymentView> GetPaystackBy(Student student, Session session)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_PAYMENT_PAYSTACK>(p => p.Person_Id == student.Id && p.Session_Id==session.Id)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ConfirmationOrderNumber = p.reference,
                                                  BankCode = p.card_type,
                                                  BankName = p.bank,
                                                  BranchCode = p.brand,
                                                  PaymentDate = p.transaction_date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  PaymentTypeName = p.Payment_Type_Name,
                                                  Amount = p.Amount,
                                                  FormatedAmount = String.Format("{0:N}", p.Amount),
                                                  SessionName = p.Session_Name
                                              }).ToList();
                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PaymentView> GetEtranzactBy(Student student, Session session)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_PAYMENT>(p => p.Person_Id == student.Id && p.Session_Id == session.Id)
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ReceiptNumber = p.Receipt_No,
                                                  ConfirmationOrderNumber = p.Confirmation_No,
                                                  BankCode = p.Bank_Code,
                                                  BankName = p.Bank_Name,
                                                  BranchCode = p.Branch_Code,
                                                  BranchName = p.Branch_Name,
                                                  PaymentDate = p.Transaction_Date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  PaymentTypeName = p.Payment_Type_Name,
                                                  Amount = p.Transaction_Amount,
                                                  FormatedAmount = String.Format("{0:N}", p.Transaction_Amount),
                                                  SessionName = p.Session_Name

                                              }).ToList();
                return payments;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<PaymentView> GETRemitaPayment(Person person)
        {
            try
            {
                List<PaymentView> payments = (from p in repository.GetBy<VW_PAYMENT_REMITA>(p => p.Person_Id == person.Id && (p.Status.Contains("01")|| p.Description.Contains("manual")))
                                              select new PaymentView
                                              {
                                                  PersonId = p.Person_Id,
                                                  PaymentId = p.Payment_Id,
                                                  InvoiceNumber = p.Invoice_Number,
                                                  ConfirmationOrderNumber = p.RRR,
                                                  PaymentDate = p.Transaction_Date,
                                                  FeeTypeId = p.Fee_Type_Id,
                                                  FeeTypeName = p.Fee_Type_Name,
                                                  PaymentTypeId = p.Payment_Type_Id,
                                                  Amount = p.Transaction_Amount,
                                                  FormatedAmount = String.Format("{0:N}", p.Transaction_Amount),
                                                  SessionName = p.Session_Name

                                              }).ToList();
                return payments;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}