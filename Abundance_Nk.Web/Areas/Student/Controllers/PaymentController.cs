using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System.Threading.Tasks;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class PaymentController : BaseController
    {
        private readonly OnlinePaymentLogic onlinePaymentLogic;
        private readonly PaymentEtranzactLogic paymentEtranzactLogic;
        private readonly PaymentLogic paymentLogic;
        private readonly FeeTypeLogic feeTypeLogic;
        private readonly SessionLogic sessionLogic;
        private readonly PersonLogic personLogic;
        private readonly StudentLevelLogic studentLevelLogic;
        private readonly StudentLogic studentLogic;
        private readonly StudentPaymentLogic studentPaymentLogic;
        private readonly PaymentViewModel viewModel;

        public PaymentController()
        {
            personLogic = new PersonLogic();
            paymentLogic = new PaymentLogic();
            onlinePaymentLogic = new OnlinePaymentLogic();
            studentLevelLogic = new StudentLevelLogic();
            studentLogic = new StudentLogic();
            studentPaymentLogic = new StudentPaymentLogic();
            paymentEtranzactLogic = new PaymentEtranzactLogic();
            feeTypeLogic = new FeeTypeLogic();
            sessionLogic = new SessionLogic();
            viewModel = new PaymentViewModel();
        }

        public ActionResult Index()
        {
            try
            {
                SetFeeTypeDropDown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        private void SetFeeTypeDropDown(PaymentViewModel viewModel)
        {
            try
            {
                if (viewModel.FeeTypeSelectListItem != null && viewModel.FeeTypeSelectListItem.Count > 0)
                {
                    viewModel.FeeType.Id = (int) FeeTypes.SchoolFees;
                    if(viewModel.StudentLevel!=null && viewModel.StudentLevel.Id > 0 && viewModel.StudentLevel.Department.Faculty.Id != 7)
                    {
                        var hostelFeeType= (int)FeeTypes.HostelFee;
                        var hostelFeeTypeId = hostelFeeType.ToString();
                        var hostefFee=viewModel.FeeTypeSelectListItem.Where(f => f.Value == hostelFeeTypeId).FirstOrDefault();
                        viewModel.FeeTypeSelectListItem.Remove(hostefFee);
                    }
                    ViewBag.FeeTypes = new SelectList(viewModel.FeeTypeSelectListItem, Utility.VALUE, Utility.TEXT,
                        viewModel.FeeType.Id);
                }
                else
                {
                    ViewBag.FeeTypes = new SelectList(new List<FeeType>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        [HttpPost]
        public ActionResult Index(PaymentViewModel viewModel)
        {
            try
            {
                //if (InvalidMatricNumber(viewModel.Student.MatricNumber))
                //{
                //    SetFeeTypeDropDown(viewModel);
                //    return View(viewModel);
                //}

                Model.Model.Student student = studentLogic.GetBy(viewModel.Student.MatricNumber);
                if (student != null && student.Id > 0)
                {
                    return RedirectToAction("GenerateInvoice", "Payment",
                        new {sid = Utility.Encrypt(student.Id.ToString())});
                }
                SetMessage("Your details couldn't be found on the portal. Please go to ICS for verification ",
                    Message.Category.Error);
                //return RedirectToAction("GenerateInvoice", "Payment", new { sid = Utility.Encrypt(student.Id.ToString()) });
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            SetFeeTypeDropDown(viewModel);
            return View(viewModel);
        }

        public ActionResult GenerateInvoice(string sid, string fid)
        {
            try
            {
                int StudentId = Convert.ToInt32(Utility.Decrypt(sid));
                int? feeTypeId = null;
                if (fid != null)
                {
                    feeTypeId = Convert.ToInt32(Utility.Decrypt(fid));
                }


                viewModel.FeeType = new FeeType {Id = feeTypeId ?? (int) FeeTypes.SchoolFees};
                viewModel.PaymentType = new PaymentType {Id = 2};

                ViewBag.Sessions = viewModel.SessionSelectListItem;
                ViewBag.States = viewModel.StateSelectListItem;
                ViewBag.Programmes = viewModel.ProgrammeSelectListItem;
                ViewBag.PaymentModes = viewModel.PaymentModeSelectListItem;
                ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);

                if (StudentId > 0)
                {
                    viewModel.StudentAlreadyExist = true;
                    viewModel.Person = personLogic.GetModelBy(p => p.Person_Id == StudentId);
                    viewModel.Student = studentLogic.GetModelBy(s => s.Person_Id == StudentId);
                    viewModel.StudentLevel = studentLevelLogic.GetBy(StudentId);
                    if (viewModel.StudentLevel != null && viewModel.StudentLevel.Programme.Id > 0)
                    {
                        ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE,
                            Utility.TEXT, viewModel.StudentLevel.Programme.Id);
                    }
                    viewModel.LevelSelectListItem = Utility.PopulateLevelSelectListItem();
                    ViewBag.Levels = viewModel.LevelSelectListItem;

                    if (viewModel.Person != null && viewModel.Person.Id > 0)
                    {
                        if (viewModel.Person.State != null && !string.IsNullOrWhiteSpace(viewModel.Person.State.Id))
                        {
                            ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT,
                                viewModel.Person.State.Id);
                        }
                    }

                    if (viewModel.StudentLevel != null && viewModel.StudentLevel.Id > 0)
                    {
                        if (viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
                        {
                            //ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.Level.Id);
                            //Commented because students weren't confirming level before generating invoice
                            ViewBag.Levels = viewModel.LevelSelectListItem;
                        }
                    }

                    SetDepartmentIfExist(viewModel);
                    SetDepartmentOptionIfExist(viewModel);
                   
                }
                else
                {
                    ViewBag.Levels = new SelectList(new List<Level>(), Utility.ID, Utility.NAME);
                    //ViewBag.Sessions = Utility.PopulateSessionSelectListItem();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        private void SetDepartmentIfExist(PaymentViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
                {
                    var departmentLogic = new ProgrammeDepartmentLogic();
                    List<Department> departments = departmentLogic.GetBy(viewModel.StudentLevel.Programme);
                    if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                    {
                        ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME,
                            viewModel.StudentLevel.Department.Id);
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(departments, Utility.ID, Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        private void SetDepartmentOptionIfExist(PaymentViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                {
                    var departmentOptionLogic = new DepartmentOptionLogic();
                    List<DepartmentOption> departmentOptions =
                        departmentOptionLogic.GetModelsBy(l => l.Department_Id == viewModel.StudentLevel.Department.Id);
                    if (viewModel.StudentLevel.DepartmentOption != null &&
                        viewModel.StudentLevel.DepartmentOption.Id > 0)
                    {
                        ViewBag.DepartmentOptions = new SelectList(departmentOptions, Utility.ID, Utility.NAME,
                            viewModel.StudentLevel.DepartmentOption.Id);
                    }
                    else
                    {
                        ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID,
                            Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
        }

        [HttpPost]
        public ActionResult GenerateInvoice(PaymentViewModel model)
        {
            try
            {
                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Person.DateOfBirth");
                ModelState.Remove("Student.MobilePhone");
                ModelState.Remove("Student.SchoolContactAddress");
                ModelState.Remove("FeeType.Name");

                if (ModelState.IsValid)
                {
                    if (InvalidDepartmentSelection(model))
                    {
                        KeepInvoiceGenerationDropDownState(model);
                        return View(model);
                    }

                    //if (InvalidMatricNumber(viewModel.Student.MatricNumber))
                    //{
                    //    KeepInvoiceGenerationDropDownState(viewModel);
                    //    return View(viewModel);
                    //}

                    Payment payment = null;
                    StudentPayment studentPayment = null;
                    if (model.StudentAlreadyExist == false)
                    {
                        using (var transaction = new TransactionScope())
                        {
                            CreatePerson(model);
                            CreateStudent(model);
                            payment = CreatePayment(model);
                            CreateStudentLevel(model);
                            studentPayment = CreateStudentPaymentLog(model, payment);
                            transaction.Complete();
                        }
                    }
                    else
                    {
                        using (var transaction = new TransactionScope())
                        {
                            StudentLevel studentLevel = studentLevelLogic.GetBy(model.Student.MatricNumber);
                            if (studentLevel != null)
                            {
                                model.StudentLevel.Id = studentLevel.Id;
                                studentLevelLogic.Modify(model.StudentLevel);
                            }
                            model.StudentLevel = studentLevelLogic.GetBy(model.Student.MatricNumber);

                            personLogic.Modify(model.Person);
                            var feeType = new FeeType { Id = (int?)model.FeeType.Id ?? (int)FeeTypes.SchoolFees };
                            feeType = feeTypeLogic.GetModelBy(a => a.Fee_Type_Id == feeType.Id);
                            model.Session = sessionLogic.GetModelBy(a => a.Session_Id == model.Session.Id);
                            payment = paymentLogic.GetBy(feeType, model.Person, model.PaymentMode, model.Session);

                            if (payment == null || payment.Id <= 0)
                            {
                                payment = CreatePayment(model);
                                payment.Session = model.Session;
                                payment.Person = model.Person;
                                studentPayment = CreateStudentPaymentLog(model, payment);
                                payment.FeeType = feeType;
                            }
                            else if (payment.PaymentMode != null)
                            {
                                if (payment.PaymentMode.Id != model.PaymentMode.Id)
                                {
                                    if (model.PaymentMode.Id == 3)
                                    {
                                        Payment firstInstallmentPayment = paymentLogic.GetFirstInstallment(payment);
                                        PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(firstInstallmentPayment);

                                        if (firstInstallmentPayment == null || (firstInstallmentPayment.InvoiceNumber == payment.InvoiceNumber &&
                                             paymentEtranzact == null))
                                        {
                                            SetMessage(
                                                "Please generate an invoice for first installment / make payment before generating for the second installment",
                                                Message.Category.Error);
                                            KeepInvoiceGenerationDropDownState(model);
                                            transaction.Dispose();
                                            return View(model);
                                        }
                                       
                                        Payment secondInstallmentPayment = paymentLogic.GetSecondInstallment(payment);
                                        if (secondInstallmentPayment == null)
                                        {
                                            payment = CreatePayment(model);
                                            studentPayment = CreateStudentPaymentLog(model, payment);
                                        }
                                    }
                                    else
                                    {
                                        PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(payment);
                                        if (paymentEtranzact == null)
                                        {
                                            payment.PaymentMode = model.PaymentMode;
                                            payment = paymentLogic.Modify(payment);
                                            payment.FeeDetails = paymentLogic.SetFeeDetails(payment,
                                                model.StudentLevel.Programme.Id, model.StudentLevel.Level.Id,
                                                payment.PaymentMode.Id, model.StudentLevel.Department.Id,
                                                model.Session.Id);

                                            studentPayment =
                                                studentPaymentLogic.GetModelBy(a => a.Payment_Id == payment.Id);
                                            studentPayment.Level.Id = model.StudentLevel.Level.Id;
                                            studentPayment.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                                            studentPaymentLogic.Modify(studentPayment);
                                        }
                                        else
                                        {
                                            SetMessage(
                                              "This payment has been processed for this session kindly cross-check you are processing payment fo rthe correct session",
                                              Message.Category.Error);
                                        }
                                    }
                                }
                                else
                                {
                                     PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(payment);
                                   
                                    if (paymentEtranzact == null)
                                    {
                                        payment.FeeDetails = paymentLogic.SetFeeDetails(payment,
                                       model.StudentLevel.Programme.Id, model.StudentLevel.Level.Id,
                                       payment.PaymentMode.Id, model.StudentLevel.Department.Id,
                                       model.Session.Id);

                                        studentPayment = studentPaymentLogic.GetModelBy(a => a.Payment_Id == payment.Id);
                                        if (studentPayment != null)
                                        {
                                            studentPayment.Level.Id = model.StudentLevel.Level.Id;
                                            studentPayment.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                                            studentPaymentLogic.Modify(studentPayment);
                                        }
                                        else
                                        {
                                            studentPayment = CreateStudentPaymentLog(model, payment);
                                        }
                                    }
                                    else
                                    {
                                        SetMessage(
                                          "This payment has been processed for this session kindly cross-check you are processing payment for the correct session",
                                          Message.Category.Error);
                                    }
                                   
                                   
                                }
                            }

                            string PaystackSecrect = ConfigurationManager.AppSettings["PaystackSecrect"].ToString();
                            string PaystackSubAccount = ConfigurationManager.AppSettings["PaystackSubAccount"].ToString();

                            FeeDetailLogic feeDetailLogic = new FeeDetailLogic();

                            List<FeeDetail> feeDetail =  feeDetailLogic.GetModelsBy(
                                    f =>
                                        f.Programme_Id == studentLevel.Programme.Id &&
                                        f.Department_Id == studentLevel.Department.Id && f.Fee_Type_Id == feeType.Id &&
                                        f.Level_Id == studentLevel.Level.Id &&
                                        f.Payment_Mode_Id == model.PaymentMode.Id &&
                                        f.Session_Id == model.Session.Id);

                            decimal feeAmount = 0;
                            if (feeDetail != null)
                            {
                                feeAmount = feeDetail.Sum(f =>f.Fee.Amount);
                            }
                            if (payment.FeeType.Id == 81)
                            {
                                string MonnifySUBAccount = ConfigurationManager.AppSettings["MonnifySUBAccount"].ToString();
                                string MonnifyURL = ConfigurationManager.AppSettings["MonnifyUrl"].ToString();
                                string MonnifyUser = ConfigurationManager.AppSettings["MonnifyApiKey"].ToString();
                                string MonnifySecrect = ConfigurationManager.AppSettings["MonnifyContractCode"].ToString();
                                string MonnifyCode = ConfigurationManager.AppSettings["MonnifyCode"].ToString();

                                //Split used here is school account. So substract commision from total fees thats what is split amount
                                decimal Total = feeAmount;
                                decimal SchoolCut = Total - 500;
                                Total += 150;
                                payment.InvoiceNumber = payment.InvoiceNumber;
                                payment.FeeType.Name = "Undergraduate Matriculation Gown";
                                payment.Id = payment.Id;
                                MonnifySplit split = new MonnifySplit { splitAmount = SchoolCut.ToString(), subAccountCode = MonnifySUBAccount };
                                PaymentMonnifyLogic paymentMonnifyLogic = new PaymentMonnifyLogic(MonnifyURL, MonnifyUser, MonnifySecrect, MonnifyCode);
                                paymentMonnifyLogic.GenerateInvoice(payment, Total.ToString(), DateTime.Now.AddDays(7), new List<MonnifySplit> { split });


                                //return RedirectToAction("Invoice", "Form",
                                //       new { Area = "Applicant", ivn = payment.InvoiceNumber });
                            }
                            else
                            {
                                int[] paystackPayments = { 19, 33, 34, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 21, 23, 22, 46, 55, 47, 49, 51, 32, 77, 76, 22, 46, 55, 47, 49, 51, 32, 77, 76, 81, 82, 87 };

                                if (!String.IsNullOrEmpty(PaystackSecrect) && feeAmount != 0M && feeAmount <= 10000M && paystackPayments.Contains(payment.FeeType.Id))
                                {
                                    PaystackLogic paystackLogic = new PaystackLogic();
                                    model.Paystack = paystackLogic.GetBy(payment);
                                    if (model.Paystack == null)
                                    {

                                        decimal amount = 0M;
                                        decimal commission = 0M;
                                        decimal addOn = 0M;
                                        amount = feeAmount;
                                        //GetAmountCommission(out amount, out commission, feeAmount, payment.FeeType, payment.Session);
                                        GetAmountCommission(out addOn, out commission, studentLevel.Programme, payment.FeeType, payment.Session);
                                        if (commission != 0)
                                        {

                                            if (addOn == 0)
                                            {
                                                amount = amount + ((decimal)(0.015) * amount);
                                            }
                                            else
                                            {
                                                amount = amount + addOn;
                                            }
                                            if (payment != null && payment.Person == null)
                                            {
                                                payment.Person = model.Person;
                                            }

                                            //SUG Fees
                                            if (payment.FeeType.Id == 87)
                                            {
                                                PaystackSubAccount = ConfigurationManager.AppSettings["PaystackSUGSubAccount"].ToString();

                                            }

                                            PaystackRepsonse paystackRepsonse = paystackLogic.MakePayment(payment, PaystackSecrect, PaystackSubAccount, amount, model.StudentLevel.Department.Name, model.StudentLevel.Level.Name, model.Student.MatricNumber, commission);
                                            if (paystackRepsonse != null && paystackRepsonse.status && !String.IsNullOrEmpty(paystackRepsonse.data.authorization_url))
                                            {
                                                paystackRepsonse.Paystack = new Paystack();
                                                paystackRepsonse.Paystack.Payment = payment;
                                                paystackRepsonse.Paystack.authorization_url = paystackRepsonse.data.authorization_url;
                                                paystackRepsonse.Paystack.access_code = paystackRepsonse.data.access_code;
                                                model.Paystack = paystackLogic.Create(paystackRepsonse.Paystack);
                                            }
                                        }


                                    }

                                }
                            }
                            

                            transaction.Complete();
                        }
                    }

                    TempData["PaymentViewModel"] = model;
                    return RedirectToAction("Invoice", "Credential", new {Area = "Common", pmid = Utility.Encrypt(payment.Id.ToString()),});
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            KeepInvoiceGenerationDropDownState(model);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> GenerateInvoiceAsync(PaymentViewModel viewModel)
        {
            try
            {
                ModelState.Remove("Student.LastName");
                ModelState.Remove("Student.FirstName");
                ModelState.Remove("Person.DateOfBirth");
                ModelState.Remove("Student.MobilePhone");
                ModelState.Remove("Student.SchoolContactAddress");
                ModelState.Remove("FeeType.Name");

                if (ModelState.IsValid)
                {
                    if (InvalidDepartmentSelection(viewModel))
                    {
                        KeepInvoiceGenerationDropDownState(viewModel);
                        return View("GenerateInvoice", viewModel);
                    }
                    

                    Payment payment = null;
                    StudentPayment studentPayment = null;
                    if (viewModel.StudentAlreadyExist == false)
                    {
                        using (var transaction = new TransactionScope())
                        {
                            CreatePerson(viewModel);
                            CreateStudent(viewModel);
                            payment = await CreatePaymentAsync(viewModel);
                            await CreateStudentLevelAsync(viewModel);
                            studentPayment = await CreateStudentPaymentLogAsync(viewModel, payment);
                            transaction.Complete();
                        }
                    }
                    else
                    {
                        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                        {
                            StudentLevel studentLevel = await studentLevelLogic.GetByAsync(viewModel.Student.MatricNumber);
                            if (studentLevel != null)
                            {
                                viewModel.StudentLevel.Id = studentLevel.Id;
                                await studentLevelLogic.ModifyAsync(viewModel.StudentLevel);
                            }
                            viewModel.StudentLevel.Id = studentLevel.Id;
                            studentLevel.Level.Id = viewModel.StudentLevel.Level.Id;
                            personLogic.Modify(viewModel.Person);

                            var feeType = new FeeType { Id = (int?)viewModel.FeeType.Id ?? (int)FeeTypes.SchoolFees };

                            feeType = await feeTypeLogic.GetModelByAsync(a => a.Fee_Type_Id == feeType.Id);
                            viewModel.Session = await sessionLogic.GetModelByAsync(a => a.Session_Id == viewModel.Session.Id);
                            payment = await paymentLogic.GetByAsync(feeType, viewModel.Person, viewModel.PaymentMode, viewModel.Session);

                            if (payment == null || payment.Id <= 0)
                            {
                                payment = await CreatePaymentAsync(viewModel);
                                payment.Session = viewModel.Session;
                                studentPayment = await CreateStudentPaymentLogAsync(viewModel, payment);
                                payment.FeeType = feeType;
                            }
                            else if (payment.PaymentMode != null)
                            {
                                if (payment.PaymentMode.Id != viewModel.PaymentMode.Id)
                                {
                                    if (viewModel.PaymentMode.Id == 3)
                                    {
                                        Payment firstInstallmentPayment = paymentLogic.GetFirstInstallment(payment);
                                        PaymentEtranzact paymentEtranzact = await paymentEtranzactLogic.GetByAsync(firstInstallmentPayment);

                                        if (firstInstallmentPayment == null ||
                                            (firstInstallmentPayment.InvoiceNumber == payment.InvoiceNumber &&
                                             paymentEtranzact == null))
                                        {
                                            SetMessage("Please generate an invoice for first installment / make payment before generating for the second installment",
                                                Message.Category.Error);
                                            KeepInvoiceGenerationDropDownState(viewModel);
                                            transaction.Dispose();
                                            return View("GenerateInvoice", viewModel);
                                        }

                                        Payment secondInstallmentPayment = paymentLogic.GetSecondInstallment(payment);
                                        if (secondInstallmentPayment == null)
                                        {
                                            payment = CreatePayment(viewModel);
                                            studentPayment = await CreateStudentPaymentLogAsync(viewModel, payment);
                                        }
                                    }
                                    else
                                    {
                                        PaymentEtranzact paymentEtranzact = await paymentEtranzactLogic.GetByAsync(payment);
                                        if (paymentEtranzact == null)
                                        {
                                            payment.PaymentMode = viewModel.PaymentMode;
                                            payment = paymentLogic.Modify(payment);
                                            payment.FeeDetails = await paymentLogic.SetFeeDetailsListAsync(payment,
                                                viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id,
                                                payment.PaymentMode.Id, viewModel.StudentLevel.Department.Id,
                                                viewModel.Session.Id);

                                            studentPayment = await studentPaymentLogic.GetModelByAsync(a => a.Payment_Id == payment.Id);
                                            studentPayment.Level.Id = viewModel.StudentLevel.Level.Id;
                                            studentPayment.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                                            studentPaymentLogic.Modify(studentPayment);
                                        }
                                    }
                                }
                                else
                                {
                                    payment.FeeDetails = await paymentLogic.SetFeeDetailsListAsync(payment,
                                        viewModel.StudentLevel.Programme.Id, viewModel.StudentLevel.Level.Id,
                                        payment.PaymentMode.Id, viewModel.StudentLevel.Department.Id,
                                        viewModel.Session.Id);

                                    studentPayment = await studentPaymentLogic.GetModelByAsync(a => a.Payment_Id == payment.Id);
                                    if (studentPayment != null)
                                    {
                                        studentPayment.Level.Id = viewModel.StudentLevel.Level.Id;
                                        studentPayment.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                                        studentPaymentLogic.Modify(studentPayment);
                                    }
                                    else
                                    {
                                        studentPayment = await CreateStudentPaymentLogAsync(viewModel, payment);
                                    }

                                }
                            }

                            string PaystackSecrect = ConfigurationManager.AppSettings["PaystackSecrect"].ToString();
                            string PaystackSubAccount = ConfigurationManager.AppSettings["PaystackSubAccount"].ToString();

                            FeeDetailLogic feeDetailLogic = new FeeDetailLogic();

                            List<FeeDetail> feeDetail =await feeDetailLogic.GetModelsByAsync(
                                    f =>
                                        f.Programme_Id == studentLevel.Programme.Id &&
                                        f.Department_Id == studentLevel.Department.Id && f.Fee_Type_Id == feeType.Id &&
                                        f.Level_Id == studentLevel.Level.Id &&
                                        f.Payment_Mode_Id == viewModel.PaymentMode.Id &&
                                        f.Session_Id == viewModel.Session.Id);

                            decimal feeAmount = 0;
                            if (feeDetail != null)
                            {
                                feeAmount = feeDetail.Sum(f => f.Fee.Amount);
                            }

                            int[] paystackPayments = { 19, 33, 34, 20, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 22, 46, 55, 47, 49, 51, 32, 77, 76 };
                            int[] paystackPaymentsWithBankCommission = { 22, 46, 55, 47, 49, 51, 32, 77, 76 };

                            if (!String.IsNullOrEmpty(PaystackSecrect) && feeAmount != 0M && feeAmount <= 10000M && paystackPayments.Contains(payment.FeeType.Id))
                            {
                                PaystackLogic paystackLogic = new PaystackLogic();
                                viewModel.Paystack = paystackLogic.GetBy(payment);
                                if (viewModel.Paystack == null)
                                {
                                    decimal amount = 0M;
                                    decimal commission = 0M;
                                    decimal addon = 0M;

                                    //GetAmountCommission(out amount, out commission, feeAmount, payment.FeeType, payment.Session);
                                    GetAmountCommission(out addon, out commission, studentLevel.Programme, payment.FeeType, payment.Session);
                                    if (commission != 0)
                                    {
                                        if (paystackPaymentsWithBankCommission.Contains(payment.FeeType.Id))
                                        {
                                            amount += 100;
                                        }
                                        else
                                        {
                                            if (addon == 0)
                                            {
                                                amount = amount + ((decimal)(0.015) * amount);
                                            }
                                            else
                                            {
                                                amount = amount + addon;
                                            }
                                        }
                                        if (payment != null && payment.Person == null)
                                        {
                                            payment.Person = viewModel.Person;
                                        }

                                        PaystackRepsonse paystackRepsonse = paystackLogic.MakePayment(payment, PaystackSecrect, PaystackSubAccount, amount, viewModel.StudentLevel.Department.Name, viewModel.StudentLevel.Level.Name, viewModel.Student.MatricNumber, commission);
                                        if (paystackRepsonse != null && paystackRepsonse.status && !String.IsNullOrEmpty(paystackRepsonse.data.authorization_url))
                                        {
                                            paystackRepsonse.Paystack = new Paystack();
                                            paystackRepsonse.Paystack.Payment = payment;
                                            paystackRepsonse.Paystack.authorization_url = paystackRepsonse.data.authorization_url;
                                            paystackRepsonse.Paystack.access_code = paystackRepsonse.data.access_code;
                                            viewModel.Paystack = paystackLogic.Create(paystackRepsonse.Paystack);
                                        }
                                    }

                                }

                            }

                            transaction.Complete();
                        }
                    }

                    TempData["PaymentViewModel"] = viewModel;
                    return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Utility.Encrypt(payment.Id.ToString()), });
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            KeepInvoiceGenerationDropDownState(viewModel);
            return View("GenerateInvoice", viewModel);
        }

        //private void GetAmountCommission(out decimal amount, out decimal commission, decimal feeAmount,FeeType feeType,Session session)
        //{
        //      PaymentPaystackCommission paymentPaystackCommission = new PaymentPaystackCommission();
        //    try
        //    {

        //        if (feeType != null && session != null && feeAmount > 0)
        //        {
        //            PaymentPayStackCommissionLogic paymentPayStackCommissionLogic = new PaymentPayStackCommissionLogic();
        //            paymentPaystackCommission = paymentPayStackCommissionLogic.GetModelBy(s => s.FeeType_Id == feeType.Id && s.Session_Id == session.Id && s.FEE.Amount == feeAmount);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //    commission = paymentPaystackCommission.Amount;
        //    amount = feeAmount;
        //}
        private void GetAmountCommission(out decimal addon, out decimal commission,Programme programme, FeeType feeType, Session session)
        {
            PaymentPaystackCommission paymentPaystackCommission = new PaymentPaystackCommission();
            try
            {

                if (feeType != null && session != null && programme!=null)
                {
                    PaymentPayStackCommissionLogic paymentPayStackCommissionLogic = new PaymentPayStackCommissionLogic();
                    paymentPaystackCommission = paymentPayStackCommissionLogic.GetModelBy(s => s.FeeType_Id == feeType.Id && s.Session_Id == session.Id && s.Programme_Id==programme.Id && s.Activated== true);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            commission = paymentPaystackCommission != null ? paymentPaystackCommission.Amount : 0;
            addon = paymentPaystackCommission != null ? (decimal)paymentPaystackCommission.AddOnFee : 0;
        }



        private bool DoesMatricNumberExist(string matricNo)
        {
            try
            {
                Model.Model.Student student = studentLogic.GetModelsBy(m => m.Matric_Number == matricNo).LastOrDefault();
                if (student == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private bool InvalidMatricNumber(string matricNo)
        {
            try
            {
                string baseMatricNo = null;
                string[] matricNoArray = matricNo.Split('/');

                if (matricNoArray.Length > 0)
                {
                    var matricNoArrayCopy = new string[matricNoArray.Length - 1];
                    for (int i = 0; i < matricNoArray.Length; i++)
                    {
                        if (i != matricNoArray.Length - 1)
                        {
                            matricNoArrayCopy[i] = matricNoArray[i];
                        }
                    }
                    if (matricNoArrayCopy.Length > 0)
                    {
                        baseMatricNo = string.Join("/", matricNoArrayCopy);
                    }
                }
                else
                {
                    SetMessage("Invalid Matric Number entered!", Message.Category.Error);
                    return true;
                }

                if (!string.IsNullOrWhiteSpace(baseMatricNo))
                {
                    //StudentMatricNumberAssignmentLogic studentMatricNumberAssignmentLogic = new StudentMatricNumberAssignmentLogic();
                    //bool isInvalid = studentMatricNumberAssignmentLogic.IsInvalid(baseMatricNo);
                    //if (isInvalid)
                    //{
                    //    SetMessage("Invalid Matric Number entered!", Message.Category.Error);
                    //    return true;
                    //}
                }
                else
                {
                    SetMessage("Invalid Matric Number entered!", Message.Category.Error);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool InvalidDepartmentSelection(PaymentViewModel viewModel)
        {
            try
            {
                if (viewModel.StudentLevel.Department == null || viewModel.StudentLevel.Department.Id <= 0)
                {
                    SetMessage("Please select Department!", Message.Category.Error);
                    return true;
                }
                //else if ((viewModel.StudentLevel.DepartmentOption == null ) || (viewModel.StudentLevel.DepartmentOption.Id <= 0 && viewModel.StudentLevel.Programme.Id > 2))
                //{
                //    viewModel.DepartmentOptionSelectListItem = Utility.PopulateDepartmentOptionSelectListItem(viewModel.StudentLevel.Department, viewModel.StudentLevel.Programme);
                //    if (viewModel.DepartmentOptionSelectListItem != null && viewModel.DepartmentOptionSelectListItem.Count > 0)
                //    {
                //        SetMessage("Please select Department Option!", Message.Category.Error);
                //        return true;
                //    }
                //}

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void KeepInvoiceGenerationDropDownState(PaymentViewModel viewModel)
        {
            try
            {
                if (viewModel.Session != null && viewModel.Session.Id > 0)
                {
                    ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT,
                        viewModel.Session.Id);
                }
                else
                {
                    ViewBag.Sessions = new SelectList(viewModel.SessionSelectListItem, Utility.VALUE, Utility.TEXT);
                }

                if (viewModel.Person.State != null && !string.IsNullOrEmpty(viewModel.Person.State.Id))
                {
                    ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT,
                        viewModel.Person.State.Id);
                }
                else
                {
                    ViewBag.States = new SelectList(viewModel.StateSelectListItem, Utility.VALUE, Utility.TEXT);
                }

                if (viewModel.StudentLevel.Level != null && viewModel.StudentLevel.Level.Id > 0)
                {
                    ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT,
                        viewModel.StudentLevel.Level.Id);
                }
                else
                {
                    ViewBag.Levels = new SelectList(viewModel.LevelSelectListItem, Utility.VALUE, Utility.TEXT);
                }
                if (viewModel.PaymentMode != null && viewModel.PaymentMode.Id > 0)
                {
                    ViewBag.PaymentModes = new SelectList(viewModel.PaymentModeSelectListItem, Utility.VALUE,
                        Utility.TEXT, viewModel.PaymentMode.Id);
                }
                else
                {
                    ViewBag.PaymentModes = new SelectList(new List<PaymentMode>(), Utility.VALUE, Utility.TEXT);
                }
                if (viewModel.StudentLevel.Programme != null && viewModel.StudentLevel.Programme.Id > 0)
                {
                    viewModel.DepartmentSelectListItem =
                        Utility.PopulateDepartmentSelectListItem(viewModel.StudentLevel.Programme);
                    ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT,
                        viewModel.StudentLevel.Programme.Id);

                    if (viewModel.StudentLevel.Department != null && viewModel.StudentLevel.Department.Id > 0)
                    {
                        viewModel.DepartmentOptionSelectListItem =
                            Utility.PopulateDepartmentOptionSelectListItem(viewModel.StudentLevel.Department,
                                viewModel.StudentLevel.Programme);
                        ViewBag.Departments = new SelectList(viewModel.DepartmentSelectListItem, Utility.VALUE,
                            Utility.TEXT, viewModel.StudentLevel.Department.Id);

                        if (viewModel.StudentLevel.DepartmentOption != null &&
                            viewModel.StudentLevel.DepartmentOption.Id > 0)
                        {
                            ViewBag.DepartmentOptions = new SelectList(viewModel.DepartmentOptionSelectListItem,
                                Utility.VALUE, Utility.TEXT, viewModel.StudentLevel.DepartmentOption.Id);
                        }
                        else
                        {
                            if (viewModel.DepartmentOptionSelectListItem != null &&
                                viewModel.DepartmentOptionSelectListItem.Count > 0)
                            {
                                ViewBag.DepartmentOptions = new SelectList(viewModel.DepartmentOptionSelectListItem,
                                    Utility.VALUE, Utility.TEXT);
                            }
                            else
                            {
                                ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID,
                                    Utility.NAME);
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Departments = new SelectList(viewModel.DepartmentSelectListItem, Utility.VALUE,
                            Utility.TEXT);
                        ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID,
                            Utility.NAME);
                    }
                }
                else
                {
                    ViewBag.Programmes = new SelectList(viewModel.ProgrammeSelectListItem, Utility.VALUE, Utility.TEXT);
                    ViewBag.Departments = new SelectList(new List<Department>(), Utility.ID, Utility.NAME);
                    ViewBag.DepartmentOptions = new SelectList(new List<DepartmentOption>(), Utility.ID, Utility.NAME);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occured " + ex.Message, Message.Category.Error);
            }
        }

        private Person CreatePerson(PaymentViewModel viewModel)
        {
            try
            {
                var role = new Role {Id = 5};
                //PersonType personType = new PersonType() { Id = viewModel.PersonType.Id };
                var nationality = new Nationality {Id = 1};

                viewModel.Person.Role = role;
                viewModel.Person.Nationality = nationality;
                viewModel.Person.DateEntered = DateTime.Now;
                //viewModel.Person.PersonType = personType;

                Person person = personLogic.Create(viewModel.Person);
                if (person != null && person.Id > 0)
                {
                    viewModel.Person.Id = person.Id;
                }

                return person;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Model.Model.Student CreateStudent(PaymentViewModel viewModel)
        {
            try
            {
                viewModel.Student.Number = 4;
                viewModel.Student.Category = new StudentCategory {Id = viewModel.StudentLevel.Level.Id <= 2 ? 1 : 2};
                viewModel.Student.Id = viewModel.Person.Id;

                return studentLogic.Create(viewModel.Student);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Payment CreatePayment(PaymentViewModel viewModel)
        {
            var newPayment = new Payment();
            try
            {
                var payment = new Payment();
                payment.PaymentMode = viewModel.PaymentMode;
                payment.PaymentType = viewModel.PaymentType;
                payment.PersonType = viewModel.Person.Type;
                payment.FeeType = viewModel.FeeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = viewModel.Person;
                payment.Session = viewModel.Session;

                var pyamentMode = new PaymentMode {Id = viewModel.PaymentMode.Id};
                OnlinePayment newOnlinePayment = null;
                newPayment = paymentLogic.Create(payment);
                newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, viewModel.StudentLevel.Programme.Id,
                    viewModel.StudentLevel.Level.Id, pyamentMode.Id, viewModel.StudentLevel.Department.Id,
                    viewModel.Session.Id);
                Decimal Amt = newPayment.FeeDetails.Sum(p => p.Fee.Amount);

                if (newPayment != null)
                {
                    var channel = new PaymentChannel {Id = (int) PaymentChannel.Channels.Etranzact};
                    var onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                }
                payment = newPayment;
                if (payment != null)
                {
                    // transaction.Complete();
                }

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private StudentLevel CreateStudentLevel(PaymentViewModel viewModel)
        {
            try
            {
                //StudentLevel studentLevel = new StudentLevel();
                //studentLevel.Department = viewModel.StudentLevel.Department;
                //studentLevel.DepartmentOption = viewModel.StudentLevel.DepartmentOption;
                //studentLevel.Level = viewModel.StudentLevel.Level;
                //studentLevel.Programme = viewModel.Programme;

                viewModel.StudentLevel.Session = viewModel.Session;
                viewModel.StudentLevel.Student = viewModel.Student;
                return studentLevelLogic.Create(viewModel.StudentLevel);


                //StudentLevel studentLevel = new StudentLevel();
                //studentLevel.Department = viewModel.StudentLevel.Department;
                //studentLevel.DepartmentOption = viewModel.StudentLevel.DepartmentOption;
                //studentLevel.Session = viewModel.Session;
                //studentLevel.Level = viewModel.StudentLevel.Level;
                //studentLevel.Programme = viewModel.Programme;
                //studentLevel.Student = viewModel.Student;

                //return studentLevelLogic.Create(studentLevel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private StudentPayment CreateStudentPaymentLog(PaymentViewModel viewModel, Payment payment)
        {
            try
            {
                var student = new Model.Model.Student();
                var studentLogic = new StudentLogic();
                student = studentLogic.GetBy(viewModel.Person.Id);
                var studentPayment = new StudentPayment();
                studentPayment.Id = payment.Id;
                studentPayment.Level = viewModel.StudentLevel.Level;
                studentPayment.Session = viewModel.Session;
                studentPayment.Student = student;
                studentPayment.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                studentPayment.Status = false;
                studentPayment = studentPaymentLogic.Create(studentPayment);
                return studentPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<Payment> CreatePaymentAsync (PaymentViewModel viewModel)
        {
            var newPayment = new Payment();
            try
            {
                var payment = new Payment();
                payment.PaymentMode = viewModel.PaymentMode;
                payment.PaymentType = viewModel.PaymentType;
                payment.PersonType = viewModel.Person.Type;
                payment.FeeType = viewModel.FeeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = viewModel.Person;
                payment.Session = viewModel.Session;

                var pyamentMode = new PaymentMode { Id = viewModel.PaymentMode.Id };
                OnlinePayment newOnlinePayment = null;
                newPayment = await paymentLogic.CreateAsync(payment);
                newPayment.FeeDetails = await paymentLogic.SetFeeDetailsListAsync(newPayment, viewModel.StudentLevel.Programme.Id,
                    viewModel.StudentLevel.Level.Id, pyamentMode.Id, viewModel.StudentLevel.Department.Id,
                    viewModel.Session.Id);
                Decimal Amt = newPayment.FeeDetails.Sum(p => p.Fee.Amount);

                if (newPayment != null)
                {
                    var channel = new PaymentChannel { Id = (int)PaymentChannel.Channels.Etranzact };
                    var onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                }
                payment = newPayment;
                if (payment != null)
                {
                    // transaction.Complete();
                }

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<StudentLevel> CreateStudentLevelAsync(PaymentViewModel viewModel)
        {
            try
            {

                viewModel.StudentLevel.Session = viewModel.Session;
                viewModel.StudentLevel.Student = viewModel.Student;
                return await studentLevelLogic.CreateAsync(viewModel.StudentLevel);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<StudentPayment> CreateStudentPaymentLogAsync(PaymentViewModel viewModel, Payment payment)
        {
            try
            {
                var student = new Model.Model.Student();
                var studentLogic = new StudentLogic();
                student = await studentLogic.GetByAsync(viewModel.Person.Id);
                var studentPayment = new StudentPayment();
                studentPayment.Id = payment.Id;
                studentPayment.Level = viewModel.StudentLevel.Level;
                studentPayment.Session = viewModel.Session;
                studentPayment.Student = student;
                studentPayment.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                studentPayment.Status = false;
                studentPayment = await studentPaymentLogic.CreateAsync(studentPayment);
                return studentPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public ActionResult GetDepartmentAndLevelByProgrammeId(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                List<Level> levels = null;
                List<Department> departments = null;
                var programme = new Programme {Id = Convert.ToInt32(id)};
                if (programme.Id > 0)
                {
                    var departmentLogic = new DepartmentLogic();
                    departments = departmentLogic.GetBy(programme);

                    var levelLogic = new LevelLogic();
                    if (programme.Id <= 2)
                    {
                        levels = levelLogic.GetONDs();
                    }
                    else if (programme.Id > 2)
                    {
                        levels = levelLogic.GetHNDs();
                    }
                }

                //return Json(new SelectList(departments, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
                //return Json(new { departments = departments, levels = levels }, "text/html", JsonRequestBehavior.AllowGet);
                return Json(new {Departments = departments, Levels = levels}, "json", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetDepartmentOptionByDepartment(string id, string programmeid)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return null;
                }

                var department = new Department {Id = Convert.ToInt32(id)};
                var programme = new Programme {Id = Convert.ToInt32(programmeid)};
                var departmentLogic = new DepartmentOptionLogic();
                List<DepartmentOption> departmentOptions = departmentLogic.GetBy(department, programme);

                return Json(new SelectList(departmentOptions, Utility.ID, Utility.NAME), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult OldFees(string Detail)
        {
            try
            {
                long sid = Convert.ToInt64(Utility.Decrypt(Detail));
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                viewModel.Student = studentLogic.GetBy(sid);
                viewModel.StudentLevel = studentLevelLogic.GetModelsBy(f => f.Person_Id == sid).LastOrDefault();
                SetFeeTypeDropDown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult OldFees(PaymentViewModel viewModel)
        {
            try
            {
                Model.Model.Student student = studentLogic.GetBy(viewModel.Student.MatricNumber);
                if (student != null && student.Id > 0)
                {
                    return RedirectToAction("GenerateInvoice", "Payment",
                        new
                        {
                            sid = Utility.Encrypt(student.Id.ToString()),
                            fid = Utility.Encrypt(viewModel.FeeType.Id.ToString())
                        });
                }
                SetMessage("Your details couldn't be found on the portal. Please go to ICS for assistance ",
                    Message.Category.Error);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            SetFeeTypeDropDown(viewModel);
            return View(viewModel);
        }
        public ActionResult PaymentClearanceReport()
        {
            return View();
        }

    }
}