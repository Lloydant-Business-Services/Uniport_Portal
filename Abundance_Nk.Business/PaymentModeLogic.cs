using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Translator;

namespace Abundance_Nk.Business
{
    public class PaymentModeLogic : BusinessBaseLogic<PaymentMode, PAYMENT_MODE>
    {
        public PaymentModeLogic()
        {
            translator = new PaymentModeTranslator();
        }

        public bool Modify(PaymentMode paymentMode)
        {
            try
            {
                Expression<Func<PAYMENT_MODE, bool>> selector = p => p.Payment_Mode_Id == paymentMode.Id;
                PAYMENT_MODE entity = GetEntityBy(selector);

                if (entity == null)
                {
                    throw new Exception(NoItemFound);
                }

                entity.Payment_Mode_Name = paymentMode.Name;
                entity.Payment_Mode_Description = paymentMode.Description;

                int modifiedRecordCount = Save();
                if (modifiedRecordCount <= 0)
                {
                    throw new Exception(NoItemModified);
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PaymentView> GetDebtorsList(Session session, Level level, Programme programme, Department department, PaymentMode paymentMode)
        {
            List<PaymentView> payments = new List<PaymentView>();
            List<PaymentView> newStudentPayments = new List<PaymentView>();
            List<PaymentView> oldStudentPayments = new List<PaymentView>();

            try
            {
                if (level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || session == null || session.Id <= 0 || paymentMode == null || paymentMode.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                if (level.Id == 1)
                {
                    List<PaymentView> newStudentSessionalPayments = new List<PaymentView>();

                    newStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                select new PaymentView
                                                {
                                                    InvoiceNumber = sr.Invoice_Number,
                                                    PaymentModeId = sr.Payment_Mode_Id,
                                                    FeeTypeId = sr.Fee_Type_Id,
                                                    SessionId = sr.Admitted_Session_Id,
                                                    SessionName = sr.Admitted_Session,
                                                    PersonId = sr.Person_Id,
                                                    Name = sr.Name,
                                                    ImageUrl = sr.Image_File_Url,
                                                    MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                    LevelId = 1,
                                                    LevelName = "100 LEVEL",
                                                    ProgrammeId = sr.Admitted_Programme_Id,
                                                    ProgrammeName = sr.Admitted_Programme,
                                                    DepartmentId = sr.Admitted_Department_Id,
                                                    DepartmentName = sr.Admitted_Department,
                                                    ConfirmationOrderNumber = sr.RRR,
                                                    Amount = sr.Transaction_Amount,
                                                    TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                    PaymentId = 1
                                                }).ToList();

                    if (paymentMode.Id == 3)
                    {
                        List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                        firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2))
                                            select new PaymentView
                                            {
                                                InvoiceNumber = sr.Invoice_Number,
                                                PaymentModeId = sr.Payment_Mode_Id,
                                                FeeTypeId = sr.Fee_Type_Id,
                                                SessionId = sr.Admitted_Session_Id,
                                                SessionName = sr.Admitted_Session,
                                                PersonId = sr.Person_Id,
                                                Name = sr.Name,
                                                ImageUrl = sr.Image_File_Url,
                                                MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                LevelId = 1,
                                                LevelName = "100 LEVEL",
                                                ProgrammeId = sr.Admitted_Programme_Id,
                                                ProgrammeName = sr.Admitted_Programme,
                                                DepartmentId = sr.Admitted_Department_Id,
                                                DepartmentName = sr.Admitted_Department,
                                                ConfirmationOrderNumber = sr.RRR,
                                                Amount = sr.Transaction_Amount,
                                                TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                PaymentId = 1
                                            }).ToList();

                        for (int i = 0; i < firstInstallmentPayments.Count; i++)
                        {
                            PaymentView firstInstallment = firstInstallmentPayments[i];

                            List<PaymentView> secondInstallments = newStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                            if (secondInstallments.Count <= 0)
                            {
                                newStudentPayments.Add(firstInstallment);
                            }
                        }

                        payments.AddRange(newStudentPayments);
                    }
                    else
                    {
                        List<PaymentView> newPayments = new List<PaymentView>();
                        newPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && (x.Payment_Mode_Id == 1 || x.Payment_Mode_Id == 2))
                                              select new PaymentView
                                              {
                                                  InvoiceNumber = sr.Invoice_Number,
                                                  PaymentModeId = sr.Payment_Mode_Id,
                                                  FeeTypeId = sr.Fee_Type_Id,
                                                  SessionId = sr.Admitted_Session_Id,
                                                  SessionName = sr.Admitted_Session,
                                                  PersonId = sr.Person_Id,
                                                  Name = sr.Name,
                                                  ImageUrl = sr.Image_File_Url,
                                                  MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                  LevelId = 1,
                                                  LevelName = "100 LEVEL",
                                                  ProgrammeId = sr.Admitted_Programme_Id,
                                                  ProgrammeName = sr.Admitted_Programme,
                                                  DepartmentId = sr.Admitted_Department_Id,
                                                  DepartmentName = sr.Admitted_Department,
                                                  ConfirmationOrderNumber = sr.RRR,
                                                  Amount = sr.Transaction_Amount,
                                                  TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                  PaymentId = 1
                                              }).ToList();

                        for (int i = 0; i < newStudentSessionalPayments.Count; i++)
                        {
                            PaymentView payment = newStudentSessionalPayments[i];
                            PaymentView personPayment = newPayments.LastOrDefault(p => p.PersonId == payment.PersonId);

                            if (personPayment == null)
                            {
                                PaymentView paymentAlreadyAdded = newStudentPayments.LastOrDefault(p => p.PersonId == payment.PersonId);
                                if (paymentAlreadyAdded == null)
                                {
                                    newStudentPayments.Add(payment);
                                }
                            }
                        }

                        payments.AddRange(newStudentPayments);
                    }
                }
                else
                {
                    List<PaymentView> oldStudentSessionalPayments = new List<PaymentView>();

                    oldStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                   select new PaymentView
                                                   {
                                                       InvoiceNumber = sr.Invoice_Number,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       FeeTypeId = sr.Fee_Type_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       PersonId = sr.Person_Id,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       LevelId = sr.Level_Id,
                                                       LevelName = sr.Level_Name,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       ConfirmationOrderNumber = sr.RRR,
                                                       Amount = sr.Transaction_Amount,
                                                       TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                       PaymentId = 1
                                                   }).ToList();

                    if (paymentMode.Id == 3)
                    {
                        List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                        firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2))
                                                    select new PaymentView
                                                    {
                                                        InvoiceNumber = sr.Invoice_Number,
                                                        PaymentModeId = sr.Payment_Mode_Id,
                                                        FeeTypeId = sr.Fee_Type_Id,
                                                        SessionId = sr.Session_Id,
                                                        SessionName = sr.Session_Name,
                                                        PersonId = sr.Person_Id,
                                                        Name = sr.Name,
                                                        ImageUrl = sr.Image_File_Url,
                                                        MatricNumber = sr.Matric_Number,
                                                        LevelId = sr.Level_Id,
                                                        LevelName = sr.Level_Name,
                                                        ProgrammeId = sr.Programme_Id,
                                                        ProgrammeName = sr.Programme_Name,
                                                        DepartmentId = sr.Department_Id,
                                                        DepartmentName = sr.Department_Name,
                                                        ConfirmationOrderNumber = sr.RRR,
                                                        Amount = sr.Transaction_Amount,
                                                        TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                        PaymentId = 1
                                                    }).ToList();

                        for (int i = 0; i < firstInstallmentPayments.Count; i++)
                        {
                            PaymentView firstInstallment = firstInstallmentPayments[i];

                            List<PaymentView> secondInstallments = oldStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                            if (secondInstallments.Count <= 0)
                            {
                                oldStudentPayments.Add(firstInstallment);
                            }
                        }

                        payments.AddRange(oldStudentPayments);
                    }
                    else
                    {
                        oldStudentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == paymentMode.Id))
                                              select new PaymentView
                                              {
                                                  InvoiceNumber = sr.Invoice_Number,
                                                  PaymentModeId = sr.Payment_Mode_Id,
                                                  FeeTypeId = sr.Fee_Type_Id,
                                                  SessionId = sr.Session_Id,
                                                  SessionName = sr.Session_Name,
                                                  PersonId = sr.Person_Id,
                                                  Name = sr.Name,
                                                  ImageUrl = sr.Image_File_Url,
                                                  MatricNumber = sr.Matric_Number,
                                                  LevelId = sr.Level_Id,
                                                  LevelName = sr.Level_Name,
                                                  ProgrammeId = sr.Programme_Id,
                                                  ProgrammeName = sr.Programme_Name,
                                                  DepartmentId = sr.Department_Id,
                                                  DepartmentName = sr.Department_Name,
                                                  ConfirmationOrderNumber = sr.RRR,
                                                  Amount = sr.Transaction_Amount,
                                                  TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                  PaymentId = 1
                                              }).ToList();

                        payments.AddRange(oldStudentPayments);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return payments.OrderBy(o => o.Name).ToList();
        }
        public List<PaymentView> GetPaymentReport(Session session, Level level, Programme programme, Department department, PaymentMode paymentMode)
        {
            List<PaymentView> payments = new List<PaymentView>();

            try
            {
                if (level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || session == null || session.Id <= 0 || paymentMode == null || paymentMode.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                if (level.Id == 1)
                {
                    payments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == paymentMode.Id) && x.Fee_Type_Id == (int)FeeTypes.SchoolFees)
                                select new PaymentView
                                {
                                    InvoiceNumber = sr.Invoice_Number,
                                    PaymentModeId = sr.Payment_Mode_Id,
                                    FeeTypeId = sr.Fee_Type_Id,
                                    SessionId = sr.Admitted_Session_Id,
                                    SessionName = sr.Admitted_Session,
                                    PersonId = sr.Person_Id,
                                    Name = sr.Name,
                                    ImageUrl = sr.Image_File_Url,
                                    MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                    LevelId = 1,
                                    LevelName = "100 LEVEL",
                                    ProgrammeId = sr.Admitted_Programme_Id,
                                    ProgrammeName = sr.Admitted_Programme,
                                    DepartmentId = sr.Admitted_Department_Id,
                                    DepartmentName = sr.Admitted_Department,
                                    ConfirmationOrderNumber = sr.RRR,
                                    Amount = sr.Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),

                                }).ToList();
                }
                else
                {
                    payments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == paymentMode.Id))
                                select new PaymentView
                                {
                                    InvoiceNumber = sr.Invoice_Number,
                                    PaymentModeId = sr.Payment_Mode_Id,
                                    FeeTypeId = sr.Fee_Type_Id,
                                    SessionId = sr.Session_Id,
                                    SessionName = sr.Session_Name,
                                    PersonId = sr.Person_Id,
                                    Name = sr.Name,
                                    ImageUrl = sr.Image_File_Url,
                                    MatricNumber = sr.Matric_Number,
                                    LevelId = sr.Level_Id,
                                    LevelName = sr.Level_Name,
                                    ProgrammeId = sr.Programme_Id,
                                    ProgrammeName = sr.Programme_Name,
                                    DepartmentId = sr.Department_Id,
                                    DepartmentName = sr.Department_Name,
                                    ConfirmationOrderNumber = sr.RRR,
                                    Amount = sr.Transaction_Amount,
                                    TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),

                                }).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return payments.OrderBy(o => o.Name).ToList();
        }

        public List<PaymentView> GetDebtorsCountFullPayment(Session session, PaymentMode paymentMode, Level level)
        {
            List<PaymentView> newStudentPayments = new List<PaymentView>();
            List<PaymentView> oldStudentPayments = new List<PaymentView>();
            List<PaymentView> allPayments = new List<PaymentView>();

            int newStudentCount = 0;
            int oldStudentCount = 0;

            try
            {
                if (session == null || session.Id <= 0 || paymentMode == null || paymentMode.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                if (level.Id == 1)
                {
                    List<PaymentView> newStudentSessionalPayments = new List<PaymentView>();

                    newStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                select new PaymentView
                                                {
                                                    InvoiceNumber = sr.Invoice_Number,
                                                    PaymentModeId = sr.Payment_Mode_Id,
                                                    FeeTypeId = sr.Fee_Type_Id,
                                                    SessionId = sr.Admitted_Session_Id,
                                                    SessionName = sr.Admitted_Session,
                                                    PersonId = sr.Person_Id,
                                                    Name = sr.Name,
                                                    ImageUrl = sr.Image_File_Url,
                                                    MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                    LevelId = 1,
                                                    LevelName = "100 LEVEL",
                                                    ProgrammeId = sr.Admitted_Programme_Id,
                                                    ProgrammeName = sr.Admitted_Programme,
                                                    DepartmentId = sr.Admitted_Department_Id,
                                                    DepartmentName = sr.Admitted_Department,
                                                    ConfirmationOrderNumber = sr.RRR,
                                                    Amount = sr.Transaction_Amount,
                                                    TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                    PaymentId = 1
                                                }).ToList();

                    if (paymentMode.Id == 3)
                    {
                        List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                        firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2))
                                            select new PaymentView
                                            {
                                                InvoiceNumber = sr.Invoice_Number,
                                                PaymentModeId = sr.Payment_Mode_Id,
                                                FeeTypeId = sr.Fee_Type_Id,
                                                SessionId = sr.Admitted_Session_Id,
                                                SessionName = sr.Admitted_Session,
                                                PersonId = sr.Person_Id,
                                                Name = sr.Name,
                                                ImageUrl = sr.Image_File_Url,
                                                MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                LevelId = 1,
                                                LevelName = "100 LEVEL",
                                                ProgrammeId = sr.Admitted_Programme_Id,
                                                ProgrammeName = sr.Admitted_Programme,
                                                DepartmentId = sr.Admitted_Department_Id,
                                                DepartmentName = sr.Admitted_Department,
                                                ConfirmationOrderNumber = sr.RRR,
                                                Amount = sr.Transaction_Amount,
                                                TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                PaymentId = 1
                                            }).ToList();

                        for (int i = 0; i < firstInstallmentPayments.Count; i++)
                        {
                            PaymentView firstInstallment = firstInstallmentPayments[i];

                            List<PaymentView> secondInstallments = newStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                            if (secondInstallments.Count <= 0)
                            {
                                newStudentPayments.Add(firstInstallment);
                            }
                        }
                    }
                    else
                    {
                        List<PaymentView> newPayments = new List<PaymentView>();
                        newPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && (x.Payment_Mode_Id == 1 || x.Payment_Mode_Id == 2))
                                              select new PaymentView
                                              {
                                                  InvoiceNumber = sr.Invoice_Number,
                                                  PaymentModeId = sr.Payment_Mode_Id,
                                                  FeeTypeId = sr.Fee_Type_Id,
                                                  SessionId = sr.Admitted_Session_Id,
                                                  SessionName = sr.Admitted_Session,
                                                  PersonId = sr.Person_Id,
                                                  Name = sr.Name,
                                                  ImageUrl = sr.Image_File_Url,
                                                  MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                  LevelId = 1,
                                                  LevelName = "100 LEVEL",
                                                  ProgrammeId = sr.Admitted_Programme_Id,
                                                  ProgrammeName = sr.Admitted_Programme,
                                                  DepartmentId = sr.Admitted_Department_Id,
                                                  DepartmentName = sr.Admitted_Department,
                                                  ConfirmationOrderNumber = sr.RRR,
                                                  Amount = sr.Transaction_Amount,
                                                  TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                  PaymentId = 1
                                              }).ToList();

                        for (int i = 0; i < newStudentSessionalPayments.Count; i++)
                        {
                            PaymentView payment = newStudentSessionalPayments[i];
                            PaymentView personPayment = newPayments.LastOrDefault(p => p.PersonId == payment.PersonId);

                            if (personPayment == null)
                            {
                                PaymentView paymentAlreadyAdded = newStudentPayments.LastOrDefault(p => p.PersonId == payment.PersonId);
                                if (paymentAlreadyAdded == null)
                                {
                                    newStudentPayments.Add(payment);
                                }
                            }
                        }
                    }
                }
                else
                {
                    List<PaymentView> oldStudentSessionalPayments = new List<PaymentView>();

                    oldStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Level_Id == level.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                   select new PaymentView
                                                   {
                                                       InvoiceNumber = sr.Invoice_Number,
                                                       PaymentModeId = sr.Payment_Mode_Id,
                                                       FeeTypeId = sr.Fee_Type_Id,
                                                       SessionId = sr.Session_Id,
                                                       SessionName = sr.Session_Name,
                                                       PersonId = sr.Person_Id,
                                                       Name = sr.Name,
                                                       ImageUrl = sr.Image_File_Url,
                                                       MatricNumber = sr.Matric_Number,
                                                       LevelId = sr.Level_Id,
                                                       LevelName = sr.Level_Name,
                                                       ProgrammeId = sr.Programme_Id,
                                                       ProgrammeName = sr.Programme_Name,
                                                       DepartmentId = sr.Department_Id,
                                                       DepartmentName = sr.Department_Name,
                                                       ConfirmationOrderNumber = sr.RRR,
                                                       Amount = sr.Transaction_Amount,
                                                       TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                       PaymentId = 1
                                                   }).ToList();

                    if (paymentMode.Id == 3)
                    {
                        List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                        firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Level_Id == level.Id  && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2))
                                                    select new PaymentView
                                                    {
                                                        InvoiceNumber = sr.Invoice_Number,
                                                        PaymentModeId = sr.Payment_Mode_Id,
                                                        FeeTypeId = sr.Fee_Type_Id,
                                                        SessionId = sr.Session_Id,
                                                        SessionName = sr.Session_Name,
                                                        PersonId = sr.Person_Id,
                                                        Name = sr.Name,
                                                        ImageUrl = sr.Image_File_Url,
                                                        MatricNumber = sr.Matric_Number,
                                                        LevelId = sr.Level_Id,
                                                        LevelName = sr.Level_Name,
                                                        ProgrammeId = sr.Programme_Id,
                                                        ProgrammeName = sr.Programme_Name,
                                                        DepartmentId = sr.Department_Id,
                                                        DepartmentName = sr.Department_Name,
                                                        ConfirmationOrderNumber = sr.RRR,
                                                        Amount = sr.Transaction_Amount,
                                                        TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                        PaymentId = 1
                                                    }).ToList();

                        for (int i = 0; i < firstInstallmentPayments.Count; i++)
                        {
                            PaymentView firstInstallment = firstInstallmentPayments[i];

                            List<PaymentView> secondInstallments = oldStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                            if (secondInstallments.Count <= 0)
                            {
                                oldStudentPayments.Add(firstInstallment);
                            }
                        }
                    }
                    else
                    {
                        oldStudentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == paymentMode.Id))
                                              select new PaymentView
                                              {
                                                  InvoiceNumber = sr.Invoice_Number,
                                                  PaymentModeId = sr.Payment_Mode_Id,
                                                  FeeTypeId = sr.Fee_Type_Id,
                                                  SessionId = sr.Session_Id,
                                                  SessionName = sr.Session_Name,
                                                  PersonId = sr.Person_Id,
                                                  Name = sr.Name,
                                                  ImageUrl = sr.Image_File_Url,
                                                  MatricNumber = sr.Matric_Number,
                                                  LevelId = sr.Level_Id,
                                                  LevelName = sr.Level_Name,
                                                  ProgrammeId = sr.Programme_Id,
                                                  ProgrammeName = sr.Programme_Name,
                                                  DepartmentId = sr.Department_Id,
                                                  DepartmentName = sr.Department_Name,
                                                  ConfirmationOrderNumber = sr.RRR,
                                                  Amount = sr.Transaction_Amount,
                                                  TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                  PaymentId = 1
                                              }).ToList();
                    }
                }

                oldStudentCount = oldStudentPayments.Count;
                newStudentCount = newStudentPayments.Count;
                    
                allPayments.AddRange(newStudentPayments);
                allPayments.AddRange(oldStudentPayments);

                for (int i = 0; i < allPayments.Count; i++)
                {
                    allPayments[i].NewStudentDebtorsCount = newStudentCount;
                    allPayments[i].OldStudentDebtorsCount = oldStudentCount;
                    allPayments[i].TotalDebtorsCount = oldStudentCount + newStudentCount;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return allPayments.OrderBy(o => o.Name).ToList();
        }

        public List<PaymentView> GetDebtorsList(Session session, Level level, Programme programme, Department department, PaymentMode paymentMode, string dateFrom, string dateTo)
        {
            List<PaymentView> payments = new List<PaymentView>();
            List<PaymentView> newStudentPayments = new List<PaymentView>();
            List<PaymentView> oldStudentPayments = new List<PaymentView>();
            DateTime processedDateFrom = new DateTime();
            DateTime processedDateTo = new DateTime();


            try
            {
                if (level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || session == null || session.Id <= 0 || paymentMode == null || paymentMode.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }


                if (level.Id == 1)
                {
                    List<PaymentView> newStudentSessionalPayments = new List<PaymentView>();

                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        processedDateFrom = ConvertToDate(dateFrom);
                        processedDateTo = ConvertToDate(dateTo);
                        TimeSpan ts = new TimeSpan(00, 00, 0);
                        processedDateFrom = processedDateFrom.Date + ts;
                        ts = new TimeSpan(23, 59, 0);
                        processedDateTo = processedDateTo.Date + ts;

                        newStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && (x.Transaction_Date >= processedDateFrom && x.Transaction_Date <= processedDateTo) && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                       select new PaymentView
                                                       {
                                                           InvoiceNumber = sr.Invoice_Number,
                                                           PaymentModeId = sr.Payment_Mode_Id,
                                                           FeeTypeId = sr.Fee_Type_Id,
                                                           SessionId = sr.Admitted_Session_Id,
                                                           SessionName = sr.Admitted_Session,
                                                           PersonId = sr.Person_Id,
                                                           Name = sr.Name,
                                                           ImageUrl = sr.Image_File_Url,
                                                           MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                           LevelId = 1,
                                                           LevelName = "100 LEVEL",
                                                           ProgrammeId = sr.Admitted_Programme_Id,
                                                           ProgrammeName = sr.Admitted_Programme,
                                                           DepartmentId = sr.Admitted_Department_Id,
                                                           DepartmentName = sr.Admitted_Department,
                                                           ConfirmationOrderNumber = sr.RRR,
                                                           Amount = sr.Transaction_Amount,
                                                           TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                           PaymentId = 1
                                                       }).ToList();

                        if (paymentMode.Id == 3)
                        {
                            List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                            firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2) && (x.Transaction_Date >= processedDateFrom && x.Transaction_Date <= processedDateTo))
                                                        select new PaymentView
                                                        {
                                                            InvoiceNumber = sr.Invoice_Number,
                                                            PaymentModeId = sr.Payment_Mode_Id,
                                                            FeeTypeId = sr.Fee_Type_Id,
                                                            SessionId = sr.Admitted_Session_Id,
                                                            SessionName = sr.Admitted_Session,
                                                            PersonId = sr.Person_Id,
                                                            Name = sr.Name,
                                                            ImageUrl = sr.Image_File_Url,
                                                            MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                            LevelId = 1,
                                                            LevelName = "100 LEVEL",
                                                            ProgrammeId = sr.Admitted_Programme_Id,
                                                            ProgrammeName = sr.Admitted_Programme,
                                                            DepartmentId = sr.Admitted_Department_Id,
                                                            DepartmentName = sr.Admitted_Department,
                                                            ConfirmationOrderNumber = sr.RRR,
                                                            Amount = sr.Transaction_Amount,
                                                            TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                            PaymentId = 1
                                                        }).ToList();

                            for (int i = 0; i < firstInstallmentPayments.Count; i++)
                            {
                                PaymentView firstInstallment = firstInstallmentPayments[i];

                                List<PaymentView> secondInstallments = newStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                                if (secondInstallments.Count <= 0)
                                {
                                    newStudentPayments.Add(firstInstallment);
                                }
                            }

                            payments.AddRange(newStudentPayments);
                        }
                        else
                        {
                            List<PaymentView> newPayments = new List<PaymentView>();
                            newPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Confirmation_No != null && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && (x.Payment_Mode_Id == 1 || x.Payment_Mode_Id == 2) && (x.Transaction_Date >= processedDateFrom && x.Transaction_Date <= processedDateTo))
                                           select new PaymentView
                                           {
                                               InvoiceNumber = sr.Invoice_Number,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               FeeTypeId = sr.Fee_Type_Id,
                                               SessionId = sr.Admitted_Session_Id,
                                               SessionName = sr.Admitted_Session,
                                               PersonId = sr.Person_Id,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                               LevelId = 1,
                                               LevelName = "100 LEVEL",
                                               ProgrammeId = sr.Admitted_Programme_Id,
                                               ProgrammeName = sr.Admitted_Programme,
                                               DepartmentId = sr.Admitted_Department_Id,
                                               DepartmentName = sr.Admitted_Department,
                                               ConfirmationOrderNumber = sr.RRR,
                                               Amount = sr.Transaction_Amount,
                                               TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                               PaymentId = 1
                                           }).ToList();

                            for (int i = 0; i < newStudentSessionalPayments.Count; i++)
                            {
                                PaymentView payment = newStudentSessionalPayments[i];
                                PaymentView personPayment = newPayments.LastOrDefault(p => p.PersonId == payment.PersonId);

                                if (personPayment == null)
                                {
                                    PaymentView paymentAlreadyAdded = newStudentPayments.LastOrDefault(p => p.PersonId == payment.PersonId);
                                    if (paymentAlreadyAdded == null)
                                    {
                                        newStudentPayments.Add(payment);
                                    }
                                }
                            }

                            payments.AddRange(newStudentPayments);
                        }
                    }
                    else
                    {
                        newStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                       select new PaymentView
                                                       {
                                                           InvoiceNumber = sr.Invoice_Number,
                                                           PaymentModeId = sr.Payment_Mode_Id,
                                                           FeeTypeId = sr.Fee_Type_Id,
                                                           SessionId = sr.Admitted_Session_Id,
                                                           SessionName = sr.Admitted_Session,
                                                           PersonId = sr.Person_Id,
                                                           Name = sr.Name,
                                                           ImageUrl = sr.Image_File_Url,
                                                           MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                           LevelId = 1,
                                                           LevelName = "100 LEVEL",
                                                           ProgrammeId = sr.Admitted_Programme_Id,
                                                           ProgrammeName = sr.Admitted_Programme,
                                                           DepartmentId = sr.Admitted_Department_Id,
                                                           DepartmentName = sr.Admitted_Department,
                                                           ConfirmationOrderNumber = sr.RRR,
                                                           Amount = sr.Transaction_Amount,
                                                           TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                           PaymentId = 1
                                                       }).ToList();

                        if (paymentMode.Id == 3)
                        {
                            List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                            firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2))
                                                        select new PaymentView
                                                        {
                                                            InvoiceNumber = sr.Invoice_Number,
                                                            PaymentModeId = sr.Payment_Mode_Id,
                                                            FeeTypeId = sr.Fee_Type_Id,
                                                            SessionId = sr.Admitted_Session_Id,
                                                            SessionName = sr.Admitted_Session,
                                                            PersonId = sr.Person_Id,
                                                            Name = sr.Name,
                                                            ImageUrl = sr.Image_File_Url,
                                                            MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                            LevelId = 1,
                                                            LevelName = "100 LEVEL",
                                                            ProgrammeId = sr.Admitted_Programme_Id,
                                                            ProgrammeName = sr.Admitted_Programme,
                                                            DepartmentId = sr.Admitted_Department_Id,
                                                            DepartmentName = sr.Admitted_Department,
                                                            ConfirmationOrderNumber = sr.RRR,
                                                            Amount = sr.Transaction_Amount,
                                                            TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                            PaymentId = 1
                                                        }).ToList();

                            for (int i = 0; i < firstInstallmentPayments.Count; i++)
                            {
                                PaymentView firstInstallment = firstInstallmentPayments[i];

                                List<PaymentView> secondInstallments = newStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                                if (secondInstallments.Count <= 0)
                                {
                                    newStudentPayments.Add(firstInstallment);
                                }
                            }

                            payments.AddRange(newStudentPayments);
                        }
                        else
                        {
                            List<PaymentView> newPayments = new List<PaymentView>();
                            newPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && (x.Payment_Mode_Id == 1 || x.Payment_Mode_Id == 2))
                                           select new PaymentView
                                           {
                                               InvoiceNumber = sr.Invoice_Number,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               FeeTypeId = sr.Fee_Type_Id,
                                               SessionId = sr.Admitted_Session_Id,
                                               SessionName = sr.Admitted_Session,
                                               PersonId = sr.Person_Id,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                               LevelId = 1,
                                               LevelName = "100 LEVEL",
                                               ProgrammeId = sr.Admitted_Programme_Id,
                                               ProgrammeName = sr.Admitted_Programme,
                                               DepartmentId = sr.Admitted_Department_Id,
                                               DepartmentName = sr.Admitted_Department,
                                               ConfirmationOrderNumber = sr.RRR,
                                               Amount = sr.Transaction_Amount,
                                               TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                               PaymentId = 1
                                           }).ToList();

                            for (int i = 0; i < newStudentSessionalPayments.Count; i++)
                            {
                                PaymentView payment = newStudentSessionalPayments[i];
                                PaymentView personPayment = newPayments.LastOrDefault(p => p.PersonId == payment.PersonId);

                                if (personPayment == null)
                                {
                                    PaymentView paymentAlreadyAdded = newStudentPayments.LastOrDefault(p => p.PersonId == payment.PersonId);
                                    if (paymentAlreadyAdded == null)
                                    {
                                        newStudentPayments.Add(payment);
                                    }
                                }
                            }

                            payments.AddRange(newStudentPayments);
                        }
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        List<PaymentView> oldStudentSessionalPayments = new List<PaymentView>();
                        processedDateFrom = ConvertToDate(dateFrom);
                        processedDateTo = ConvertToDate(dateTo);
                        TimeSpan ts = new TimeSpan(00, 00, 0);
                        processedDateFrom = processedDateFrom.Date + ts;
                        ts = new TimeSpan(23, 59, 0);
                        processedDateTo = processedDateTo.Date + ts;

                        oldStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && (x.Transaction_Date >= processedDateFrom && x.Transaction_Date <= processedDateTo) && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                       select new PaymentView
                                                       {
                                                           InvoiceNumber = sr.Invoice_Number,
                                                           PaymentModeId = sr.Payment_Mode_Id,
                                                           FeeTypeId = sr.Fee_Type_Id,
                                                           SessionId = sr.Session_Id,
                                                           SessionName = sr.Session_Name,
                                                           PersonId = sr.Person_Id,
                                                           Name = sr.Name,
                                                           ImageUrl = sr.Image_File_Url,
                                                           MatricNumber = sr.Matric_Number,
                                                           LevelId = sr.Level_Id,
                                                           LevelName = sr.Level_Name,
                                                           ProgrammeId = sr.Programme_Id,
                                                           ProgrammeName = sr.Programme_Name,
                                                           DepartmentId = sr.Department_Id,
                                                           DepartmentName = sr.Department_Name,
                                                           ConfirmationOrderNumber = sr.RRR,
                                                           Amount = sr.Transaction_Amount,
                                                           TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                           PaymentId = 1
                                                       }).ToList();

                        if (paymentMode.Id == 3)
                        {
                            List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                            firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2) && (x.Transaction_Date >= processedDateFrom && x.Transaction_Date <= processedDateTo))
                                                        select new PaymentView
                                                        {
                                                            InvoiceNumber = sr.Invoice_Number,
                                                            PaymentModeId = sr.Payment_Mode_Id,
                                                            FeeTypeId = sr.Fee_Type_Id,
                                                            SessionId = sr.Session_Id,
                                                            SessionName = sr.Session_Name,
                                                            PersonId = sr.Person_Id,
                                                            Name = sr.Name,
                                                            ImageUrl = sr.Image_File_Url,
                                                            MatricNumber = sr.Matric_Number,
                                                            LevelId = sr.Level_Id,
                                                            LevelName = sr.Level_Name,
                                                            ProgrammeId = sr.Programme_Id,
                                                            ProgrammeName = sr.Programme_Name,
                                                            DepartmentId = sr.Department_Id,
                                                            DepartmentName = sr.Department_Name,
                                                            ConfirmationOrderNumber = sr.RRR,
                                                            Amount = sr.Transaction_Amount,
                                                            TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                            PaymentId = 1
                                                        }).ToList();

                            for (int i = 0; i < firstInstallmentPayments.Count; i++)
                            {
                                PaymentView firstInstallment = firstInstallmentPayments[i];

                                List<PaymentView> secondInstallments = oldStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                                if (secondInstallments.Count <= 0)
                                {
                                    oldStudentPayments.Add(firstInstallment);
                                }
                            }

                            payments.AddRange(oldStudentPayments);
                        }
                        else
                        {
                            oldStudentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Confirmation_No == null && (x.Payment_Mode_Id == paymentMode.Id) && (x.Transaction_Date >= processedDateFrom && x.Transaction_Date <= processedDateTo))
                                                  select new PaymentView
                                                  {
                                                      InvoiceNumber = sr.Invoice_Number,
                                                      PaymentModeId = sr.Payment_Mode_Id,
                                                      FeeTypeId = sr.Fee_Type_Id,
                                                      SessionId = sr.Session_Id,
                                                      SessionName = sr.Session_Name,
                                                      PersonId = sr.Person_Id,
                                                      Name = sr.Name,
                                                      ImageUrl = sr.Image_File_Url,
                                                      MatricNumber = sr.Matric_Number,
                                                      LevelId = sr.Level_Id,
                                                      LevelName = sr.Level_Name,
                                                      ProgrammeId = sr.Programme_Id,
                                                      ProgrammeName = sr.Programme_Name,
                                                      DepartmentId = sr.Department_Id,
                                                      DepartmentName = sr.Department_Name,
                                                      ConfirmationOrderNumber = sr.RRR,
                                                      Amount = sr.Transaction_Amount,
                                                      TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                      PaymentId = 1
                                                  }).ToList();

                            payments.AddRange(oldStudentPayments);
                        }
                    }
                    else
                    {
                        List<PaymentView> oldStudentSessionalPayments = new List<PaymentView>();

                        oldStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                       select new PaymentView
                                                       {
                                                           InvoiceNumber = sr.Invoice_Number,
                                                           PaymentModeId = sr.Payment_Mode_Id,
                                                           FeeTypeId = sr.Fee_Type_Id,
                                                           SessionId = sr.Session_Id,
                                                           SessionName = sr.Session_Name,
                                                           PersonId = sr.Person_Id,
                                                           Name = sr.Name,
                                                           ImageUrl = sr.Image_File_Url,
                                                           MatricNumber = sr.Matric_Number,
                                                           LevelId = sr.Level_Id,
                                                           LevelName = sr.Level_Name,
                                                           ProgrammeId = sr.Programme_Id,
                                                           ProgrammeName = sr.Programme_Name,
                                                           DepartmentId = sr.Department_Id,
                                                           DepartmentName = sr.Department_Name,
                                                           ConfirmationOrderNumber = sr.RRR,
                                                           Amount = sr.Transaction_Amount,
                                                           TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                           PaymentId = 1
                                                       }).ToList();

                        if (paymentMode.Id == 3)
                        {
                            List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                            firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2))
                                                        select new PaymentView
                                                        {
                                                            InvoiceNumber = sr.Invoice_Number,
                                                            PaymentModeId = sr.Payment_Mode_Id,
                                                            FeeTypeId = sr.Fee_Type_Id,
                                                            SessionId = sr.Session_Id,
                                                            SessionName = sr.Session_Name,
                                                            PersonId = sr.Person_Id,
                                                            Name = sr.Name,
                                                            ImageUrl = sr.Image_File_Url,
                                                            MatricNumber = sr.Matric_Number,
                                                            LevelId = sr.Level_Id,
                                                            LevelName = sr.Level_Name,
                                                            ProgrammeId = sr.Programme_Id,
                                                            ProgrammeName = sr.Programme_Name,
                                                            DepartmentId = sr.Department_Id,
                                                            DepartmentName = sr.Department_Name,
                                                            ConfirmationOrderNumber = sr.RRR,
                                                            Amount = sr.Transaction_Amount,
                                                            TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                            PaymentId = 1
                                                        }).ToList();

                            for (int i = 0; i < firstInstallmentPayments.Count; i++)
                            {
                                PaymentView firstInstallment = firstInstallmentPayments[i];

                                List<PaymentView> secondInstallments = oldStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                                if (secondInstallments.Count <= 0)
                                {
                                    oldStudentPayments.Add(firstInstallment);
                                }
                            }

                            payments.AddRange(oldStudentPayments);
                        }
                        else
                        {
                            oldStudentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == paymentMode.Id))
                                                  select new PaymentView
                                                  {
                                                      InvoiceNumber = sr.Invoice_Number,
                                                      PaymentModeId = sr.Payment_Mode_Id,
                                                      FeeTypeId = sr.Fee_Type_Id,
                                                      SessionId = sr.Session_Id,
                                                      SessionName = sr.Session_Name,
                                                      PersonId = sr.Person_Id,
                                                      Name = sr.Name,
                                                      ImageUrl = sr.Image_File_Url,
                                                      MatricNumber = sr.Matric_Number,
                                                      LevelId = sr.Level_Id,
                                                      LevelName = sr.Level_Name,
                                                      ProgrammeId = sr.Programme_Id,
                                                      ProgrammeName = sr.Programme_Name,
                                                      DepartmentId = sr.Department_Id,
                                                      DepartmentName = sr.Department_Name,
                                                      ConfirmationOrderNumber = sr.RRR,
                                                      Amount = sr.Transaction_Amount,
                                                      TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                      PaymentId = 1
                                                  }).ToList();

                            payments.AddRange(oldStudentPayments);
                        }
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return payments.OrderBy(o => o.Name).ToList();
        }

        public List<PaymentView> GetDebtorsCountFullPayment(Session session, PaymentMode paymentMode, Level level, string dateFrom, string dateTo)
        {
            List<PaymentView> newStudentPayments = new List<PaymentView>();
            List<PaymentView> oldStudentPayments = new List<PaymentView>();
            List<PaymentView> allPayments = new List<PaymentView>();

            DateTime transactionDateFrom = ConvertToDate(dateFrom);
            DateTime transactionDateTo = ConvertToDate(dateTo);

            int newStudentCount = 0;
            int oldStudentCount = 0;

            try
            {
                if (session == null || session.Id <= 0 || paymentMode == null || paymentMode.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                if (level.Id == 1)
                {

                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        List<PaymentView> newStudentSessionalPayments = new List<PaymentView>();

                        newStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && (x.Transaction_Date >= transactionDateFrom && x.Transaction_Date <= transactionDateTo) && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                       select new PaymentView
                                                       {
                                                           InvoiceNumber = sr.Invoice_Number,
                                                           PaymentModeId = sr.Payment_Mode_Id,
                                                           FeeTypeId = sr.Fee_Type_Id,
                                                           SessionId = sr.Admitted_Session_Id,
                                                           SessionName = sr.Admitted_Session,
                                                           PersonId = sr.Person_Id,
                                                           Name = sr.Name,
                                                           ImageUrl = sr.Image_File_Url,
                                                           MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                           LevelId = 1,
                                                           LevelName = "100 LEVEL",
                                                           ProgrammeId = sr.Admitted_Programme_Id,
                                                           ProgrammeName = sr.Admitted_Programme,
                                                           DepartmentId = sr.Admitted_Department_Id,
                                                           DepartmentName = sr.Admitted_Department,
                                                           ConfirmationOrderNumber = sr.RRR,
                                                           Amount = sr.Transaction_Amount,
                                                           TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                           PaymentId = 1
                                                       }).ToList();

                        if (paymentMode.Id == 3)
                        {
                            List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                            firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2) && (x.Transaction_Date >= transactionDateFrom && x.Transaction_Date <= transactionDateTo))
                                                        select new PaymentView
                                                        {
                                                            InvoiceNumber = sr.Invoice_Number,
                                                            PaymentModeId = sr.Payment_Mode_Id,
                                                            FeeTypeId = sr.Fee_Type_Id,
                                                            SessionId = sr.Admitted_Session_Id,
                                                            SessionName = sr.Admitted_Session,
                                                            PersonId = sr.Person_Id,
                                                            Name = sr.Name,
                                                            ImageUrl = sr.Image_File_Url,
                                                            MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                            LevelId = 1,
                                                            LevelName = "100 LEVEL",
                                                            ProgrammeId = sr.Admitted_Programme_Id,
                                                            ProgrammeName = sr.Admitted_Programme,
                                                            DepartmentId = sr.Admitted_Department_Id,
                                                            DepartmentName = sr.Admitted_Department,
                                                            ConfirmationOrderNumber = sr.RRR,
                                                            Amount = sr.Transaction_Amount,
                                                            TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                            PaymentId = 1
                                                        }).ToList();

                            for (int i = 0; i < firstInstallmentPayments.Count; i++)
                            {
                                PaymentView firstInstallment = firstInstallmentPayments[i];

                                List<PaymentView> secondInstallments = newStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                                if (secondInstallments.Count <= 0)
                                {
                                    newStudentPayments.Add(firstInstallment);
                                }
                            }
                        }
                        else
                        {
                            List<PaymentView> newPayments = new List<PaymentView>();
                            newPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && (x.Payment_Mode_Id == 1 || x.Payment_Mode_Id == 2) && (x.Transaction_Date >= transactionDateFrom && x.Transaction_Date <= transactionDateTo))
                                           select new PaymentView
                                           {
                                               InvoiceNumber = sr.Invoice_Number,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               FeeTypeId = sr.Fee_Type_Id,
                                               SessionId = sr.Admitted_Session_Id,
                                               SessionName = sr.Admitted_Session,
                                               PersonId = sr.Person_Id,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                               LevelId = 1,
                                               LevelName = "100 LEVEL",
                                               ProgrammeId = sr.Admitted_Programme_Id,
                                               ProgrammeName = sr.Admitted_Programme,
                                               DepartmentId = sr.Admitted_Department_Id,
                                               DepartmentName = sr.Admitted_Department,
                                               ConfirmationOrderNumber = sr.RRR,
                                               Amount = sr.Transaction_Amount,
                                               TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                               PaymentId = 1
                                           }).ToList();

                            for (int i = 0; i < newStudentSessionalPayments.Count; i++)
                            {
                                PaymentView payment = newStudentSessionalPayments[i];
                                PaymentView personPayment = newPayments.LastOrDefault(p => p.PersonId == payment.PersonId);

                                if (personPayment == null)
                                {
                                    PaymentView paymentAlreadyAdded = newStudentPayments.LastOrDefault(p => p.PersonId == payment.PersonId);
                                    if (paymentAlreadyAdded == null)
                                    {
                                        newStudentPayments.Add(payment);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        List<PaymentView> newStudentSessionalPayments = new List<PaymentView>();

                        newStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                       select new PaymentView
                                                       {
                                                           InvoiceNumber = sr.Invoice_Number,
                                                           PaymentModeId = sr.Payment_Mode_Id,
                                                           FeeTypeId = sr.Fee_Type_Id,
                                                           SessionId = sr.Admitted_Session_Id,
                                                           SessionName = sr.Admitted_Session,
                                                           PersonId = sr.Person_Id,
                                                           Name = sr.Name,
                                                           ImageUrl = sr.Image_File_Url,
                                                           MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                           LevelId = 1,
                                                           LevelName = "100 LEVEL",
                                                           ProgrammeId = sr.Admitted_Programme_Id,
                                                           ProgrammeName = sr.Admitted_Programme,
                                                           DepartmentId = sr.Admitted_Department_Id,
                                                           DepartmentName = sr.Admitted_Department,
                                                           ConfirmationOrderNumber = sr.RRR,
                                                           Amount = sr.Transaction_Amount,
                                                           TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                           PaymentId = 1
                                                       }).ToList();

                        if (paymentMode.Id == 3)
                        {
                            List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                            firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2))
                                                        select new PaymentView
                                                        {
                                                            InvoiceNumber = sr.Invoice_Number,
                                                            PaymentModeId = sr.Payment_Mode_Id,
                                                            FeeTypeId = sr.Fee_Type_Id,
                                                            SessionId = sr.Admitted_Session_Id,
                                                            SessionName = sr.Admitted_Session,
                                                            PersonId = sr.Person_Id,
                                                            Name = sr.Name,
                                                            ImageUrl = sr.Image_File_Url,
                                                            MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                                            LevelId = 1,
                                                            LevelName = "100 LEVEL",
                                                            ProgrammeId = sr.Admitted_Programme_Id,
                                                            ProgrammeName = sr.Admitted_Programme,
                                                            DepartmentId = sr.Admitted_Department_Id,
                                                            DepartmentName = sr.Admitted_Department,
                                                            ConfirmationOrderNumber = sr.RRR,
                                                            Amount = sr.Transaction_Amount,
                                                            TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                            PaymentId = 1
                                                        }).ToList();

                            for (int i = 0; i < firstInstallmentPayments.Count; i++)
                            {
                                PaymentView firstInstallment = firstInstallmentPayments[i];

                                List<PaymentView> secondInstallments = newStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                                if (secondInstallments.Count <= 0)
                                {
                                    newStudentPayments.Add(firstInstallment);
                                }
                            }
                        }
                        else
                        {
                            List<PaymentView> newPayments = new List<PaymentView>();
                            newPayments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && (x.Payment_Mode_Id == 1 || x.Payment_Mode_Id == 2))
                                           select new PaymentView
                                           {
                                               InvoiceNumber = sr.Invoice_Number,
                                               PaymentModeId = sr.Payment_Mode_Id,
                                               FeeTypeId = sr.Fee_Type_Id,
                                               SessionId = sr.Admitted_Session_Id,
                                               SessionName = sr.Admitted_Session,
                                               PersonId = sr.Person_Id,
                                               Name = sr.Name,
                                               ImageUrl = sr.Image_File_Url,
                                               MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                               LevelId = 1,
                                               LevelName = "100 LEVEL",
                                               ProgrammeId = sr.Admitted_Programme_Id,
                                               ProgrammeName = sr.Admitted_Programme,
                                               DepartmentId = sr.Admitted_Department_Id,
                                               DepartmentName = sr.Admitted_Department,
                                               ConfirmationOrderNumber = sr.RRR,
                                               Amount = sr.Transaction_Amount,
                                               TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                               PaymentId = 1
                                           }).ToList();

                            for (int i = 0; i < newStudentSessionalPayments.Count; i++)
                            {
                                PaymentView payment = newStudentSessionalPayments[i];
                                PaymentView personPayment = newPayments.LastOrDefault(p => p.PersonId == payment.PersonId);

                                if (personPayment == null)
                                {
                                    PaymentView paymentAlreadyAdded = newStudentPayments.LastOrDefault(p => p.PersonId == payment.PersonId);
                                    if (paymentAlreadyAdded == null)
                                    {
                                        newStudentPayments.Add(payment);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        List<PaymentView> oldStudentSessionalPayments = new List<PaymentView>();

                        oldStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Level_Id == level.Id && (x.Payment_Mode_Id == 1 || x.Payment_Mode_Id == 2) && (x.Transaction_Date >= transactionDateFrom && x.Transaction_Date <= transactionDateTo) && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                       select new PaymentView
                                                       {
                                                           InvoiceNumber = sr.Invoice_Number,
                                                           PaymentModeId = sr.Payment_Mode_Id,
                                                           FeeTypeId = sr.Fee_Type_Id,
                                                           SessionId = sr.Session_Id,
                                                           SessionName = sr.Session_Name,
                                                           PersonId = sr.Person_Id,
                                                           Name = sr.Name,
                                                           ImageUrl = sr.Image_File_Url,
                                                           MatricNumber = sr.Matric_Number,
                                                           LevelId = sr.Level_Id,
                                                           LevelName = sr.Level_Name,
                                                           ProgrammeId = sr.Programme_Id,
                                                           ProgrammeName = sr.Programme_Name,
                                                           DepartmentId = sr.Department_Id,
                                                           DepartmentName = sr.Department_Name,
                                                           ConfirmationOrderNumber = sr.RRR,
                                                           Amount = sr.Transaction_Amount,
                                                           TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                           PaymentId = 1
                                                       }).ToList();

                        if (paymentMode.Id == 3)
                        {
                            List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                            firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Level_Id == level.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2) && (x.Payment_Mode_Id == 1 || x.Payment_Mode_Id == 2) && (x.Transaction_Date >= transactionDateFrom && x.Transaction_Date <= transactionDateTo))
                                                        select new PaymentView
                                                        {
                                                            InvoiceNumber = sr.Invoice_Number,
                                                            PaymentModeId = sr.Payment_Mode_Id,
                                                            FeeTypeId = sr.Fee_Type_Id,
                                                            SessionId = sr.Session_Id,
                                                            SessionName = sr.Session_Name,
                                                            PersonId = sr.Person_Id,
                                                            Name = sr.Name,
                                                            ImageUrl = sr.Image_File_Url,
                                                            MatricNumber = sr.Matric_Number,
                                                            LevelId = sr.Level_Id,
                                                            LevelName = sr.Level_Name,
                                                            ProgrammeId = sr.Programme_Id,
                                                            ProgrammeName = sr.Programme_Name,
                                                            DepartmentId = sr.Department_Id,
                                                            DepartmentName = sr.Department_Name,
                                                            ConfirmationOrderNumber = sr.RRR,
                                                            Amount = sr.Transaction_Amount,
                                                            TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                            PaymentId = 1
                                                        }).ToList();

                            for (int i = 0; i < firstInstallmentPayments.Count; i++)
                            {
                                PaymentView firstInstallment = firstInstallmentPayments[i];

                                List<PaymentView> secondInstallments = oldStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                                if (secondInstallments.Count <= 0)
                                {
                                    oldStudentPayments.Add(firstInstallment);
                                }
                            }
                        }
                        else
                        {
                            oldStudentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Confirmation_No == null && (x.Payment_Mode_Id == paymentMode.Id) && (x.Payment_Mode_Id == 1 || x.Payment_Mode_Id == 2) && (x.Invoice_Date >= transactionDateFrom && x.Invoice_Date <= transactionDateTo))
                                                  select new PaymentView
                                                  {
                                                      InvoiceNumber = sr.Invoice_Number,
                                                      PaymentModeId = sr.Payment_Mode_Id,
                                                      FeeTypeId = sr.Fee_Type_Id,
                                                      SessionId = sr.Session_Id,
                                                      SessionName = sr.Session_Name,
                                                      PersonId = sr.Person_Id,
                                                      Name = sr.Name,
                                                      ImageUrl = sr.Image_File_Url,
                                                      MatricNumber = sr.Matric_Number,
                                                      LevelId = sr.Level_Id,
                                                      LevelName = sr.Level_Name,
                                                      ProgrammeId = sr.Programme_Id,
                                                      ProgrammeName = sr.Programme_Name,
                                                      DepartmentId = sr.Department_Id,
                                                      DepartmentName = sr.Department_Name,
                                                      ConfirmationOrderNumber = sr.Confirmation_No,
                                                      Amount = sr.Transaction_Amount,
                                                      InvoiceGenerationDate = sr.Invoice_Date,
                                                      TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                      PaymentId = 1
                                                  }).ToList();
                        }
                    }
                    else
                    {
                        List<PaymentView> oldStudentSessionalPayments = new List<PaymentView>();

                        oldStudentSessionalPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Level_Id == level.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")))
                                                       select new PaymentView
                                                       {
                                                           InvoiceNumber = sr.Invoice_Number,
                                                           PaymentModeId = sr.Payment_Mode_Id,
                                                           FeeTypeId = sr.Fee_Type_Id,
                                                           SessionId = sr.Session_Id,
                                                           SessionName = sr.Session_Name,
                                                           PersonId = sr.Person_Id,
                                                           Name = sr.Name,
                                                           ImageUrl = sr.Image_File_Url,
                                                           MatricNumber = sr.Matric_Number,
                                                           LevelId = sr.Level_Id,
                                                           LevelName = sr.Level_Name,
                                                           ProgrammeId = sr.Programme_Id,
                                                           ProgrammeName = sr.Programme_Name,
                                                           DepartmentId = sr.Department_Id,
                                                           DepartmentName = sr.Department_Name,
                                                           ConfirmationOrderNumber = sr.RRR,
                                                           Amount = sr.Transaction_Amount,
                                                           TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                           PaymentId = 1
                                                       }).ToList();

                        if (paymentMode.Id == 3)
                        {
                            List<PaymentView> firstInstallmentPayments = new List<PaymentView>();

                            firstInstallmentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Level_Id == level.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == 2))
                                                        select new PaymentView
                                                        {
                                                            InvoiceNumber = sr.Invoice_Number,
                                                            PaymentModeId = sr.Payment_Mode_Id,
                                                            FeeTypeId = sr.Fee_Type_Id,
                                                            SessionId = sr.Session_Id,
                                                            SessionName = sr.Session_Name,
                                                            PersonId = sr.Person_Id,
                                                            Name = sr.Name,
                                                            ImageUrl = sr.Image_File_Url,
                                                            MatricNumber = sr.Matric_Number,
                                                            LevelId = sr.Level_Id,
                                                            LevelName = sr.Level_Name,
                                                            ProgrammeId = sr.Programme_Id,
                                                            ProgrammeName = sr.Programme_Name,
                                                            DepartmentId = sr.Department_Id,
                                                            DepartmentName = sr.Department_Name,
                                                            ConfirmationOrderNumber = sr.RRR,
                                                            Amount = sr.Transaction_Amount,
                                                            TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                            PaymentId = 1
                                                        }).ToList();

                            for (int i = 0; i < firstInstallmentPayments.Count; i++)
                            {
                                PaymentView firstInstallment = firstInstallmentPayments[i];

                                List<PaymentView> secondInstallments = oldStudentSessionalPayments.Where(p => p.PersonId == firstInstallment.PersonId && p.ConfirmationOrderNumber != null && p.PaymentModeId == 3).ToList();

                                if (secondInstallments.Count <= 0)
                                {
                                    oldStudentPayments.Add(firstInstallment);
                                }
                            }
                        }
                        else
                        {
                            oldStudentPayments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Session_Id == session.Id && x.Confirmation_No == null && (x.Payment_Mode_Id == paymentMode.Id))
                                                  select new PaymentView
                                                  {
                                                      InvoiceNumber = sr.Invoice_Number,
                                                      PaymentModeId = sr.Payment_Mode_Id,
                                                      FeeTypeId = sr.Fee_Type_Id,
                                                      SessionId = sr.Session_Id,
                                                      SessionName = sr.Session_Name,
                                                      PersonId = sr.Person_Id,
                                                      Name = sr.Name,
                                                      ImageUrl = sr.Image_File_Url,
                                                      MatricNumber = sr.Matric_Number,
                                                      LevelId = sr.Level_Id,
                                                      LevelName = sr.Level_Name,
                                                      ProgrammeId = sr.Programme_Id,
                                                      ProgrammeName = sr.Programme_Name,
                                                      DepartmentId = sr.Department_Id,
                                                      DepartmentName = sr.Department_Name,
                                                      ConfirmationOrderNumber = sr.RRR,
                                                      Amount = sr.Transaction_Amount,
                                                      TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),
                                                      PaymentId = 1
                                                  }).ToList();
                        }
                    }
                }

                oldStudentCount = oldStudentPayments.Count;
                newStudentCount = newStudentPayments.Count;

                allPayments.AddRange(newStudentPayments);
                allPayments.AddRange(oldStudentPayments);

                for (int i = 0; i < allPayments.Count; i++)
                {
                    allPayments[i].NewStudentDebtorsCount = newStudentCount;
                    allPayments[i].OldStudentDebtorsCount = oldStudentCount;
                    allPayments[i].TotalDebtorsCount = oldStudentCount + newStudentCount;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return allPayments.OrderBy(o => o.Name).ToList();
        }

        public List<PaymentView> GetPaymentReport(Session session, Level level, Programme programme, Department department, PaymentMode paymentMode, string dateFrom, string dateTo)
        {
            List<PaymentView> payments = new List<PaymentView>();

            try
            {
                DateTime transactionDateFrom = ConvertToDate(dateFrom);
                DateTime transactionDateTo = ConvertToDate(dateTo);
                transactionDateFrom = ConvertToDate(dateFrom);
                transactionDateTo = ConvertToDate(dateTo);
                TimeSpan ts = new TimeSpan(00, 00, 0);
                transactionDateFrom = transactionDateFrom.Date + ts;
                ts = new TimeSpan(23, 59, 0);
                transactionDateTo = transactionDateTo.Date + ts;


                if (level == null || level.Id <= 0 || programme == null || programme.Id <= 0 || department == null || department.Id <= 0 || session == null || session.Id <= 0 || paymentMode == null || paymentMode.Id <= 0)
                {
                    throw new Exception("One or more criteria to get payments not set! Please check your input criteria selection and try again.");
                }

                if (level.Id == 1)
                {
                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        payments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == paymentMode.Id) && x.Fee_Type_Id == (int)FeeTypes.SchoolFees && (x.Transaction_Date >= transactionDateFrom && x.Transaction_Date <= transactionDateTo))
                                    select new PaymentView
                                    {
                                        InvoiceNumber = sr.Invoice_Number,
                                        PaymentModeId = sr.Payment_Mode_Id,
                                        FeeTypeId = sr.Fee_Type_Id,
                                        SessionId = sr.Admitted_Session_Id,
                                        SessionName = sr.Admitted_Session,
                                        PersonId = sr.Person_Id,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                        LevelId = 1,
                                        LevelName = "100 LEVEL",
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        ProgrammeName = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        DepartmentName = sr.Admitted_Department,
                                        ConfirmationOrderNumber = sr.RRR,
                                        Amount = sr.Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),

                                    }).ToList();
                    }
                    else
                    {
                        payments = (from sr in repository.GetBy<VW_PAYMENT_NEW_STUDENT>(x => x.Admitted_Programme_Id == programme.Id && x.Admitted_Department_Id == department.Id && x.Admitted_Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == paymentMode.Id) && x.Fee_Type_Id == (int)FeeTypes.SchoolFees)
                                    select new PaymentView
                                    {
                                        InvoiceNumber = sr.Invoice_Number,
                                        PaymentModeId = sr.Payment_Mode_Id,
                                        FeeTypeId = sr.Fee_Type_Id,
                                        SessionId = sr.Admitted_Session_Id,
                                        SessionName = sr.Admitted_Session,
                                        PersonId = sr.Person_Id,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number ?? sr.Application_Form_Number,
                                        LevelId = 1,
                                        LevelName = "100 LEVEL",
                                        ProgrammeId = sr.Admitted_Programme_Id,
                                        ProgrammeName = sr.Admitted_Programme,
                                        DepartmentId = sr.Admitted_Department_Id,
                                        DepartmentName = sr.Admitted_Department,
                                        ConfirmationOrderNumber = sr.RRR,
                                        Amount = sr.Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),

                                    }).ToList();
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        payments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == paymentMode.Id) && (x.Transaction_Date >= transactionDateFrom && x.Transaction_Date <= transactionDateTo))
                                    select new PaymentView
                                    {
                                        InvoiceNumber = sr.Invoice_Number,
                                        PaymentModeId = sr.Payment_Mode_Id,
                                        FeeTypeId = sr.Fee_Type_Id,
                                        SessionId = sr.Session_Id,
                                        SessionName = sr.Session_Name,
                                        PersonId = sr.Person_Id,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        LevelId = sr.Level_Id,
                                        LevelName = sr.Level_Name,
                                        ProgrammeId = sr.Programme_Id,
                                        ProgrammeName = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        DepartmentName = sr.Department_Name,
                                        ConfirmationOrderNumber = sr.RRR,
                                        Amount = sr.Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),

                                    }).ToList();
                    }
                    else
                    {
                        payments = (from sr in repository.GetBy<VW_PAYMENT_OLD_STUDENT>(x => x.Programme_Id == programme.Id && x.Department_Id == department.Id && x.Level_Id == level.Id && x.Session_Id == session.Id && x.Status != null && (x.Status.Contains("01") || x.Description.Contains("manual")) && (x.Payment_Mode_Id == paymentMode.Id))
                                    select new PaymentView
                                    {
                                        InvoiceNumber = sr.Invoice_Number,
                                        PaymentModeId = sr.Payment_Mode_Id,
                                        FeeTypeId = sr.Fee_Type_Id,
                                        SessionId = sr.Session_Id,
                                        SessionName = sr.Session_Name,
                                        PersonId = sr.Person_Id,
                                        Name = sr.Name,
                                        ImageUrl = sr.Image_File_Url,
                                        MatricNumber = sr.Matric_Number,
                                        LevelId = sr.Level_Id,
                                        LevelName = sr.Level_Name,
                                        ProgrammeId = sr.Programme_Id,
                                        ProgrammeName = sr.Programme_Name,
                                        DepartmentId = sr.Department_Id,
                                        DepartmentName = sr.Department_Name,
                                        ConfirmationOrderNumber = sr.RRR,
                                        Amount = sr.Transaction_Amount,
                                        TransactionDate = Convert.ToDateTime(sr.Transaction_Date).ToShortDateString(),

                                    }).ToList();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return payments.OrderBy(o => o.Name).ToList();
        }


        private DateTime ConvertToDate(string date)
        {
            DateTime newDate = new DateTime();
            try
            {
                //newDate = DateTime.Parse(date);
                string[] dateSplit = date.Split('/');
                newDate = new DateTime(Convert.ToInt32(dateSplit[2]), Convert.ToInt32(dateSplit[1]), Convert.ToInt32(dateSplit[0]));
            }
            catch (Exception)
            {
                throw;
            }

            return newDate;
        }
    }
}