using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Applicant.Controllers
{
    [AllowAnonymous]
    public class TranscriptController :BaseController
    {
        private const string VALUE = "Value";
        private const string TEXT = "Text";

        private TranscriptViewModel viewModel;

        public ActionResult Index()
        {
            viewModel = new TranscriptViewModel();
            ViewBag.StateId = viewModel.StateSelectList;
            ViewBag.CountryId = viewModel.CountrySelectList;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(TranscriptViewModel transcriptViewModel)
        {
            try
            {
                if(transcriptViewModel.transcriptRequest.student.MatricNumber != null)
                {
                    var transcriptRequestLogic = new TranscriptRequestLogic();
                    TranscriptRequest tRequest =
                        transcriptRequestLogic.GetModelsBy(
                            t => t.STUDENT.Matric_Number == transcriptViewModel.transcriptRequest.student.MatricNumber)
                            .LastOrDefault();

                    if(tRequest != null)
                    {
                        var personLogic = new PersonLogic();
                        Person person = personLogic.GetModelBy(p => p.Person_Id == tRequest.student.Id);
                        tRequest.student.FirstName = person.FirstName;
                        tRequest.student.LastName = person.LastName;
                        tRequest.student.OtherName = person.OtherName;
                        transcriptViewModel.transcriptRequest = tRequest;

                        if(tRequest.payment != null && tRequest.payment.Id > 0)
                        {
                            var remitaPaymentLogic = new RemitaPaymentLogic();
                            tRequest.remitaPayment = remitaPaymentLogic.GetBy(tRequest.payment.Id);
                            if(tRequest.remitaPayment != null && tRequest.remitaPayment.Status.Contains("01"))
                            {
                                GetStudentDetails(transcriptViewModel);
                                transcriptViewModel.transcriptRequest.payment = null;
                            }
                        }
                    }
                    else
                    {
                        GetStudentDetails(transcriptViewModel);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(transcriptViewModel);
        }

        public ActionResult IndexAlt(TranscriptViewModel transcriptViewModel)
        {
            try
            {
                if(transcriptViewModel.transcriptRequest.student == null)
                {
                    SetMessage("Enter Your Matriculation Number",Message.Category.Error);
                    return RedirectToAction("Index");
                }
                if(transcriptViewModel.transcriptRequest.student.MatricNumber != null)
                {
                    var transcriptRequestLogic = new TranscriptRequestLogic();
                    List<TranscriptRequest> tRequests =
                        transcriptRequestLogic.GetModelsBy(
                            t => t.STUDENT.Matric_Number == transcriptViewModel.transcriptRequest.student.MatricNumber);

                    if(tRequests.Count > 0)
                    {
                        var personLogic = new PersonLogic();
                        long sid = tRequests.FirstOrDefault().student.Id;
                        Person person = personLogic.GetModelBy(p => p.Person_Id == sid);

                        for(int i = 0;i < tRequests.Count;i++)
                        {
                            tRequests[i].student.FirstName = person.FirstName;
                            tRequests[i].student.LastName = person.LastName;
                            tRequests[i].student.OtherName = person.OtherName;
                            //transcriptViewModel.transcriptRequest = tRequests[i];

                            if(tRequests[i].payment != null && tRequests[i].payment.Id > 0)
                            {
                                var remitaPaymentLogic = new RemitaPaymentLogic();
                                tRequests[i].remitaPayment = remitaPaymentLogic.GetBy(tRequests[i].payment.Id);
                                if(tRequests[i].remitaPayment != null &&
                                    tRequests[i].remitaPayment.Status.Contains("01"))
                                {
                                    GetStudentDetails(transcriptViewModel);
                                    tRequests[i].payment = null;
                                }
                            }
                        }

                        transcriptViewModel.TranscriptRequests = tRequests;

                        return View(transcriptViewModel);
                    }
                    var studentLogic = new StudentLogic();
                    var student = new Model.Model.Student();
                    student = studentLogic.GetBy(transcriptViewModel.transcriptRequest.student.MatricNumber);

                    if(student != null)
                    {
                        return RedirectToAction("Request",new { sid = student.Id });
                    }
                    return RedirectToAction("Request",new { sid = 0 });
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            return View(transcriptViewModel);
        }

        private static void GetStudentDetails(TranscriptViewModel transcriptViewModel)
        {
            var studentLogic = new StudentLogic();
            var student = new Model.Model.Student();
            string MatricNumber = transcriptViewModel.transcriptRequest.student.MatricNumber;
            student = studentLogic.GetBy(MatricNumber);
            if(student != null)
            {
                var personLogic = new PersonLogic();
                Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                student.FirstName = person.FirstName;
                student.LastName = person.LastName;
                student.OtherName = person.OtherName;
                transcriptViewModel.transcriptRequest.student = student;
            }
        }

        public ActionResult Request(long sid)
        {
            try
            {

                viewModel = new TranscriptViewModel();
                var transcriptRequest = new TranscriptRequest();
                var studentLogic = new StudentLogic();
                var studentLevelLogic = new StudentLevelLogic();
                var transcriptRequestLogic = new TranscriptRequestLogic();
                if(sid > 0)
                {
                    // transcriptRequest = transcriptRequestLogic.GetBy(sid);
                    transcriptRequest = transcriptRequestLogic.GetModelsBy(tr => tr.Student_id == sid).FirstOrDefault();
                    if(transcriptRequest != null)
                    {
                        viewModel.transcriptRequest = transcriptRequest;
                    }
                    else
                    {
                        Model.Model.Student student = studentLogic.GetBy(sid);
                        var studentLevel = studentLevelLogic.GetBy(sid);
                        if (student != null && studentLevel != null)
                        {
                            viewModel.transcriptRequest.student = student;
                            viewModel.Programme = studentLevel.Programme;
                            viewModel.Department = studentLevel.Department;
                            viewModel.Level = studentLevel.Level;
                            
                        }
                       
                    }
                }

                //if (viewModel.Programme != null)
                //{
                //    ViewBag.Programme = new SelectList(viewModel.ProgrammeSelectList, VALUE, TEXT, viewModel.Programme.Id);
                //}
                //else
                //{
                //    ViewBag.Programme = viewModel.ProgrammeSelectList;
                //}
                //if (viewModel.Department != null)
                //{
                //    ViewBag.Department = new SelectList(viewModel.DepartmentSelectList, VALUE, TEXT, viewModel.Department.Id);
                //}
                //else
                //{
                //    ViewBag.Department = viewModel.DepartmentSelectList;
                //}
                //if (viewModel.Level != null)
                //{
                //   ViewBag.Level = new SelectList(viewModel.LevelSelectList, VALUE, TEXT, viewModel.Level.Id);
                //}
                //else
                //{
                //    ViewBag.Level = viewModel.LevelSelectList;
                //}

                ViewBag.Level = viewModel.LevelSelectList;
                ViewBag.Department = viewModel.DepartmentSelectList;
                ViewBag.Programme = viewModel.ProgrammeSelectList;
                ViewBag.StateId = viewModel.StateSelectList;
                ViewBag.CountryId = viewModel.CountrySelectList;
                ViewBag.FeeType = viewModel.FeesTypeSelectList;
                ViewBag.DeliveryServices = viewModel.DeliveryServiceSelectList;
                ViewBag.DeliveryServiceZones = new SelectList(new List<DeliveryServiceZone>(), "Id", "Name");


            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            TempData["TranscriptViewModel"] = viewModel;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Request(TranscriptViewModel transcriptViewModel)
        {
            try
            {
                viewModel = new TranscriptViewModel();
                ReloadDropdown(transcriptViewModel);
                int feeTypeId = transcriptViewModel.transcriptRequest.payment.FeeType.Id ;
                DeliveryServiceZoneLogic deliveryServiceZoneLogic = new DeliveryServiceZoneLogic();
                if(transcriptViewModel.transcriptRequest != null && transcriptViewModel.transcriptRequest.Id <= 0 && transcriptViewModel.transcriptRequest.student != null && transcriptViewModel.transcriptRequest.DestinationCountry != null)
                {
                    if(transcriptViewModel.transcriptRequest.student.Id < 1)
                    {
                        var student = new Model.Model.Student();
                        var person = new Person();
                        var studentLevel = new StudentLevel();
                        var stduentLogic = new StudentLogic();
                        var studentLevelLogic = new StudentLevelLogic();
                        var personLogic = new PersonLogic();

                        student = transcriptViewModel.transcriptRequest.student;

                        var role = new Role { Id = 5 };
                        var nationality = new Nationality { Id = 1 };
                        using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                        {
                            person.LastName = student.LastName;
                            person.FirstName = student.FirstName;
                            person.OtherName = student.OtherName;
                            person.State = new State { Id = "OG" };
                            person.Role = role;
                            person.Nationality = nationality;
                            person.DateEntered = DateTime.Now;
                            person.Type = new PersonType { Id = 3 };
                            person.MobilePhone = student.MobilePhone;
                            person.Email = student.Email;
                            person = personLogic.Create(person);
                            if (person != null && person.Id > 0)
                            {
                                string Type = student.MatricNumber.Substring(0, 1);
                                if (Type == "H")
                                {
                                    student.Type = new StudentType { Id = 2 };
                                }
                                else
                                {
                                    student.Type = new StudentType { Id = 1 };
                                }
                                student.Id = person.Id;
                                student.Category = new StudentCategory { Id = 2 };
                                student.Status = new StudentStatus { Id = 1 };
                                student = stduentLogic.Create(student);

                                studentLevel.Programme = transcriptViewModel.Programme;
                                studentLevel.Department = transcriptViewModel.Department;
                                studentLevel.Level = transcriptViewModel.Level;
                                studentLevel.Session = new Model.Model.Session { Id = 13 };
                                studentLevel.Student = student;
                                studentLevelLogic.Create(studentLevel);
                                transaction.Complete();
                            }
                        }
                    }

                    var transcriptRequestLogic = new TranscriptRequestLogic();
                    var transcriptRequest = new TranscriptRequest();
                    transcriptRequest = transcriptViewModel.transcriptRequest;
                    transcriptRequest.DateRequested = DateTime.Now;
                    //transcriptRequest.DestinationCountry = new Country { Id = "NIG" };
                    if (transcriptRequest.DestinationState.Id == null)
                    {
                        transcriptRequest.DestinationState = new State { Id = "OT" };
                    }
                    transcriptRequest.transcriptClearanceStatus = new TranscriptClearanceStatus
                    {
                        TranscriptClearanceStatusId = 4
                    };
                    if (transcriptViewModel.Activated && transcriptViewModel.DeliveryServiceZone != null && transcriptViewModel.DeliveryServiceZone.Id > 0)
                    {
                          DeliveryServiceZone deliveryServiceZone = deliveryServiceZoneLogic.GetModelBy(d => d.Delivery_Service_Zone_Id == transcriptViewModel.DeliveryServiceZone.Id);
                          transcriptRequest.DeliveryServiceZone = deliveryServiceZone;
                         feeTypeId = deliveryServiceZone.FeeType.Id;

                     }
                    transcriptRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = 1 };
                    transcriptRequest.payment = null;
                    transcriptRequest = transcriptRequestLogic.Create(transcriptRequest);
                    return RedirectToAction("TranscriptPayment", new { tid = transcriptRequest.Id, fid = feeTypeId });
                }
                else
                {
                    var transcriptRequestLogic = new TranscriptRequestLogic();
                    var studentLevelLogic = new StudentLevelLogic();
                    var studentLevel = new StudentLevel();
                    studentLevel = studentLevelLogic.GetBy(transcriptViewModel.transcriptRequest.student.Id);
                    if (studentLevel == null)
                    {
                        studentLevel = new StudentLevel();
                        studentLevel.Programme = transcriptViewModel.Programme;
                        studentLevel.Department = transcriptViewModel.Department;
                        studentLevel.Level = transcriptViewModel.Level;
                        studentLevel.Session = new Model.Model.Session { Id = 13 };
                        studentLevel.Student = new Model.Model.Student { Id = transcriptViewModel.transcriptRequest.student.Id };
                        studentLevelLogic.Create(studentLevel);
                    }

                    if (transcriptViewModel.transcriptRequest.payment.Id <= 0)
                    {
                        var transcriptRequest = new TranscriptRequest();
                        transcriptRequest = transcriptViewModel.transcriptRequest;
                        transcriptRequest.DateRequested = DateTime.Now;
                        //transcriptRequest.DestinationCountry = new Country { Id = "NIG" };
                        if(transcriptRequest.DestinationState.Id == null)
                        {
                            transcriptRequest.DestinationState = new State { Id = "OT" };
                        }
                        if (transcriptViewModel.Activated && transcriptViewModel.DeliveryServiceZone != null && transcriptViewModel.DeliveryServiceZone.Id > 0)
                        {
                            DeliveryServiceZone deliveryServiceZone = deliveryServiceZoneLogic.GetModelBy(d => d.Delivery_Service_Zone_Id == transcriptViewModel.DeliveryServiceZone.Id);
                            transcriptRequest.DeliveryServiceZone = deliveryServiceZone;
                            feeTypeId = deliveryServiceZone.FeeType.Id;

                        }
                        transcriptRequest.transcriptClearanceStatus = new TranscriptClearanceStatus
                        {
                            TranscriptClearanceStatusId = 4
                        };
                        transcriptRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = 1 };
                        transcriptRequest.payment = null;
                        transcriptRequest = transcriptRequestLogic.Create(transcriptRequest);
                        return RedirectToAction("TranscriptPayment", new { tid = transcriptRequest.Id, fid = feeTypeId });
                    }
                    transcriptRequestLogic.Modify(transcriptViewModel.transcriptRequest);
                    return RedirectToAction("TranscriptPayment", new { tid = transcriptViewModel.transcriptRequest.Id, fid = feeTypeId });
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            TempData["TranscriptViewModel"] = viewModel;
            return RedirectToAction("Index");
        }

        public ActionResult TranscriptPayment(long tid,int fid=13)
        {
            viewModel = new TranscriptViewModel();
            try
            {
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();
                var transcriptRequestLogic = new TranscriptRequestLogic();
                TranscriptRequest tRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == tid);
                var personLogic = new PersonLogic();
                var person = new Person();
                var remitaPaymentLogic = new RemitaPaymentLogic();

                person = personLogic.GetModelBy(p => p.Person_Id == tRequest.student.Id);
                if(person != null)
                {
                    tRequest.student.ImageFileUrl = person.ImageFileUrl;
                    tRequest.student.FirstName = person.FirstName;
                    tRequest.student.LastName = person.LastName;
                    tRequest.student.OtherName = person.OtherName;

                    if(tRequest.payment == null)
                    {
                       tRequest.payment = new Payment(){FeeType = new FeeType(){Id =  fid}};
                    }
                    viewModel.transcriptRequest = tRequest;
                    if (tRequest.payment != null && tRequest.transcriptStatus.TranscriptStatusId != 5)
                    {
                        var etranzactpayment = paymentEtranzactLogic.GetModelBy(x => x.Payment_Id == tRequest.payment.Id);
                        if (etranzactpayment != null)
                        {
                            viewModel.transcriptRequest.transcriptStatus.TranscriptStatusId=4;
                        }
                        else
                        {
                            var interswitchPayment = paymentInterswitchLogic.GetModelBy(x => x.Payment_Id == tRequest.payment.Id);
                            if (interswitchPayment != null && interswitchPayment.ResponseCode == "00")
                            {
                                viewModel.transcriptRequest.transcriptStatus.TranscriptStatusId = 4;
                            }
                        }
                    }
                    

                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            TempData["TranscriptViewModel"] = viewModel;
            return View(viewModel);
        }

        public ActionResult ProcessPayment(long tid, int fid = 13)
        {
            try
            {
                var viewModel = new TranscriptViewModel();
                var transcriptRequestLogic = new TranscriptRequestLogic();
                TranscriptRequest tRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == tid);
                if(tRequest != null)
                {
                    Model.Model.Student student = tRequest.student;
                    var personLogic = new PersonLogic();
                    Person person = personLogic.GetModelBy(t => t.Person_Id == tRequest.student.Id);
                    var payment = new Payment();
                    using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        payment = CreatePayment(student,fid);

                        payment.Person = person;
                        CreateStudentPaymentLog(payment);
                        transaction.Complete();
                    }
                    tRequest.payment = payment;
                    tRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = 3 };
                    transcriptRequestLogic.Modify(tRequest);

                    return RedirectToAction("Invoice",new { controller = "Credential",area = "Common",pmid = Utility.Encrypt(payment.Id.ToString()) });

                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("Index");
        }


        private StudentPayment CreateStudentPaymentLog(Payment payment)
        {
            try
            {
                StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                var student = new Model.Model.Student();
                var studentLogic = new StudentLogic();
                student = studentLogic.GetBy(payment.Person.Id);

                var studentLevelLogic = new StudentLevelLogic();
                var studentLevel = new StudentLevel();
                studentLevel = studentLevelLogic.GetBy(payment.Person.Id);
                PaymentLogic paymentLogic = new PaymentLogic();
                payment.FeeDetails = paymentLogic.SetFeeDetails(payment,studentLevel.Programme.Id, studentLevel.Level.Id,1, studentLevel.Department.Id, studentLevel.Session.Id);

                var studentPayment = new StudentPayment();
                studentPayment.Id = payment.Id;
                studentPayment.Level = studentLevel.Level;
                studentPayment.Session = new Model.Model.Session() { Id = 13};
                studentPayment.Student = student;
                var amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                if (amount == 0)
                {
                    amount = 30000;
                }
                studentPayment.Amount = amount;
                studentPayment.Status = false;
                studentPayment = studentPaymentLogic.Create(studentPayment);
                return studentPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private string GenerateHash(string apiKey,RemitaPayment remitaPayment)
        {
            try
            {
                string hash = remitaPayment.MerchantCode + remitaPayment.RRR + apiKey;
                var remitaProcessor = new RemitaPayementProcessor(hash);
                string hashConcatenate = remitaProcessor.HashPaymentDetailToSHA512(hash);
                return hashConcatenate;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult MakePayment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MakePayment(TranscriptViewModel viewModel)
        {
            try
            {
                var payment = new Payment();
                var paymentLogic = new PaymentLogic();
                var paymentEtranzact = new PaymentEtranzact();
                var paymentEtranzactLogic = new PaymentEtranzactLogic();

                var session = new Session { Id = 1 };
                var feetype = new FeeType { Id = (int)FeeTypes.Transcript };

                payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.PaymentEtranzact.ConfirmationNo,session,
                    feetype);
                if(payment != null && payment.Id > 0)
                {
                    if(payment.FeeType.Id != (int)FeeTypes.Transcript)
                    {
                        paymentEtranzact =
                            paymentEtranzactLogic.GetModelBy(
                                p => p.Confirmation_No == viewModel.PaymentEtranzact.ConfirmationNo);
                        if(paymentEtranzact != null)
                        {
                            viewModel.PaymentEtranzact = paymentEtranzact;
                            viewModel.Paymentstatus = true;
                            var transcriptRequestLogic = new TranscriptRequestLogic();
                            var transcriptRequest = new TranscriptRequest();
                            payment = paymentLogic.GetModelBy(p => p.Invoice_Number == paymentEtranzact.CustomerID);
                            transcriptRequest = transcriptRequestLogic.GetModelBy(p => p.Payment_Id == payment.Id);
                            return RedirectToAction("TranscriptPayment",new { tid = transcriptRequest.Id });
                        }

                        viewModel.Paymentstatus = false;
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(viewModel);
        }

        private Payment CreatePayment(Model.Model.Student student, int feeTypeId)
        {
            try
            {
                var payment = new Payment();
                var paymentLogic = new PaymentLogic();
                payment.PaymentMode = new PaymentMode { Id = 1 };
                payment.PaymentType = new PaymentType { Id = 2 };
                payment.PersonType = new PersonType { Id = 4 };
                payment.FeeType = new FeeType { Id = feeTypeId };
                payment.DatePaid = DateTime.Now;
                payment.Person = student;
                payment.Session = new Session { Id = 13 };

                OnlinePayment newOnlinePayment = null;
                Payment newPayment = paymentLogic.Create(payment);
                if(newPayment != null)
                {
                    var channel = new PaymentChannel { Id = (int)PaymentChannel.Channels.Etranzact };
                    var onlinePaymentLogic = new OnlinePaymentLogic();
                    var onlinePayment = new OnlinePayment();
                    onlinePayment.Channel = channel;
                    onlinePayment.Payment = newPayment;
                    newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                }

                return newPayment;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public JsonResult GetState(string id)
        {
            try
            {
                if(string.IsNullOrEmpty(id))
                {
                    return null;
                }

                string country = id;
                return Json(country,JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void ReloadDropdown(TranscriptViewModel transcriptViewModel)
        {
            if(transcriptViewModel.transcriptRequest.DestinationState != null &&
                !string.IsNullOrEmpty(transcriptViewModel.transcriptRequest.DestinationState.Id))
            {
                ViewBag.StateId = new SelectList(viewModel.StateSelectList,Utility.VALUE,Utility.TEXT,
                    transcriptViewModel.transcriptRequest.DestinationState.Id);
                ViewBag.CountryId = new SelectList(viewModel.CountrySelectList,Utility.VALUE,Utility.TEXT,
                    transcriptViewModel.transcriptRequest.DestinationCountry.Id);
            }
            else
            {
                ViewBag.StateId = new SelectList(viewModel.StateSelectList,Utility.VALUE,Utility.TEXT);
                ViewBag.CountryId = new SelectList(viewModel.CountrySelectList,Utility.VALUE,Utility.TEXT,
                    transcriptViewModel.transcriptRequest.DestinationCountry.Id);
            }
        }

        public ActionResult VerificationFees()
        {
            try
            {
                viewModel = new TranscriptViewModel();
                ViewBag.FeeTypeId = viewModel.FeesTypeSelectList;
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult VerificationFees(TranscriptViewModel transcriptViewModel)
        {
            try
            {
                if(transcriptViewModel.StudentVerification.Student.MatricNumber != null)
                {
                    var remitaPaymentLogic = new RemitaPaymentLogic();
                    transcriptViewModel.StudentVerification.RemitaPayment =
                        remitaPaymentLogic.GetModelBy(
                            r => r.PAYMENT.Person_Id == transcriptViewModel.StudentVerification.Student.Id);
                    if(transcriptViewModel.StudentVerification.RemitaPayment != null)
                    {
                        var personLogic = new PersonLogic();
                        Person person =
                            personLogic.GetModelBy(
                                p => p.Person_Id == transcriptViewModel.StudentVerification.Student.Id);
                        transcriptViewModel.StudentVerification.Student.FirstName = person.FirstName;
                        transcriptViewModel.StudentVerification.Student.LastName = person.LastName;
                        transcriptViewModel.StudentVerification.Student.OtherName = person.OtherName;
                    }
                    else
                    {
                        var studentLogic = new StudentLogic();
                        var student = new Model.Model.Student();
                        string MatricNumber = transcriptViewModel.StudentVerification.Student.MatricNumber;
                        student = studentLogic.GetBy(MatricNumber);
                        if(student != null)
                        {
                            var personLogic = new PersonLogic();
                            Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                            student.FirstName = person.FirstName;
                            student.LastName = person.LastName;
                            student.OtherName = person.OtherName;
                            transcriptViewModel.StudentVerification.Student = student;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(transcriptViewModel);
        }

        public ActionResult VerificationRequest(long sid)
        {
            try
            {
                viewModel = new TranscriptViewModel();
                var studentLogic = new StudentLogic();
                if(sid > 0)
                {
                    viewModel.StudentVerification.Student = studentLogic.GetBy(sid);
                    if(viewModel.StudentVerification.Student != null)
                    {
                        viewModel.StudentVerification.FeeType = new FeeType();
                    }
                }

                ViewBag.FeeTypeId = viewModel.FeesTypeSelectList;
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            TempData["TranscriptViewModel"] = viewModel;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult VerificationRequest(TranscriptViewModel transcriptViewModel)
        {
            try
            {
                viewModel = new TranscriptViewModel();

                if(transcriptViewModel.StudentVerification != null &&
                    transcriptViewModel.StudentVerification.RemitaPayment == null)
                {
                    if(transcriptViewModel.StudentVerification.Student.Id < 1)
                    {
                        var student = new Model.Model.Student();
                        var person = new Person();
                        var studentLevel = new StudentLevel();
                        var stduentLogic = new StudentLogic();
                        var studentLevelLogic = new StudentLevelLogic();
                        var personLogic = new PersonLogic();

                        student = transcriptViewModel.StudentVerification.Student;

                        var role = new Role { Id = 5 };
                        var nationality = new Nationality { Id = 1 };

                        person.LastName = student.LastName;
                        person.FirstName = student.FirstName;
                        person.OtherName = student.OtherName;
                        person.Email = student.Email;
                        person.MobilePhone = student.MobilePhone;
                        person.State = new State { Id = "OG" };
                        person.Role = role;
                        person.Nationality = nationality;
                        person.DateEntered = DateTime.Now;
                        person.Type = new PersonType { Id = 3 };
                        person = personLogic.Create(person);
                        if(person != null && person.Id > 0)
                        {
                            string StudentType = student.MatricNumber.Substring(0,1);
                            if(StudentType == "H")
                            {
                                student.Type = new StudentType { Id = 2 };
                            }
                            else
                            {
                                student.Type = new StudentType { Id = 1 };
                            }
                            student.Id = person.Id;
                            student.Category = new StudentCategory { Id = 2 };
                            student.Status = new StudentStatus { Id = 1 };
                            student = stduentLogic.Create(student);
                            transcriptViewModel.StudentVerification.Student.Id = student.Id;
                            return RedirectToAction("ProcessVerificationPayment",
                                new { sid = student.Id,feetypeId = transcriptViewModel.StudentVerification.FeeType.Id });
                        }
                    }
                    return RedirectToAction("ProcessVerificationPayment",
                        new
                        {
                            sid = transcriptViewModel.StudentVerification.Student.Id,
                            feetypeId = transcriptViewModel.StudentVerification.FeeType.Id
                        });
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            TempData["TranscriptViewModel"] = viewModel;
            return RedirectToAction("ProcessVerificationPayment",
                new
                {
                    sid = transcriptViewModel.StudentVerification.Student.Id,
                    feetypeId = transcriptViewModel.StudentVerification.FeeType.Id
                });
        }

        public ActionResult ProcessVerificationPayment(long sid,int feetypeId)
        {
            try
            {
                viewModel = new TranscriptViewModel();
                viewModel.StudentVerification = new StudentVerification();
                var studentLogic = new StudentLogic();
                viewModel.StudentVerification.Student = studentLogic.GetBy(sid);
                if(viewModel.StudentVerification.Student != null)
                {
                    var personLogic = new PersonLogic();
                    Person person = personLogic.GetModelBy(t => t.Person_Id == viewModel.StudentVerification.Student.Id);
                    var payment = new Payment();
                    using(
                        var transaction = new TransactionScope(TransactionScopeOption.Required,
                            new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot }))
                    {
                        payment = CreatePayment(viewModel.StudentVerification.Student,feetypeId);
                        if(payment != null)
                        {
                            Fee fee = null;
                            var paymentLogic = new PaymentLogic();

                            //Get Payment Specific Setting
                            var settings = new RemitaSettings();
                            var settingsLogic = new RemitaSettingsLogic();

                            if(feetypeId == 14)
                            {
                                fee = new Fee { Id = 47,Name = "CERTIFICATE COLLECTION" };
                                settings = settingsLogic.GetBy(8);
                            }
                            else if(feetypeId == 15)
                            {
                                fee = new Fee { Id = 49,Name = "STUDENTSHIP VERIFICATION" };
                                settings = settingsLogic.GetBy(7);
                            }
                            else if(feetypeId == 16)
                            {
                                fee = new Fee { Id = 48,Name = "WES VERIFICATION" };
                                settings = settingsLogic.GetBy(6);
                            }
                           // payment.FeeDetails = paymentLogic.SetFeeDetails(fee.Id);
                            viewModel.StudentVerification.Amount = payment.FeeDetails.Sum(a => a.Fee.Amount);
                            if(viewModel.StudentVerification.Student.Email == null)
                            {
                                viewModel.StudentVerification.Student.Email =
                                    viewModel.StudentVerification.Student.FullName + "@gmail.com";
                            }
                            var remitaObj = new Remita
                            {
                                merchantId = settings.MarchantId,
                                serviceTypeId = settings.serviceTypeId,
                                orderId = payment.InvoiceNumber,
                                payerEmail = viewModel.StudentVerification.Student.Email,
                                payerName = viewModel.StudentVerification.Student.FullName,
                                payerPhone = viewModel.StudentVerification.Student.MobilePhone,
                                paymenttype = fee.Name,
                                responseurl = settings.Response_Url,
                                totalAmount = viewModel.StudentVerification.Amount
                            };
                            string toHash = remitaObj.merchantId + remitaObj.serviceTypeId + remitaObj.orderId +
                                            remitaObj.totalAmount + remitaObj.responseurl + settings.Api_key;

                            var remitaPayementProcessor = new RemitaPayementProcessor(settings.Api_key);
                            remitaObj.hash = remitaPayementProcessor.HashPaymentDetailToSHA512(toHash);
                            viewModel.StudentVerification.Remita = remitaObj;
                            var remitaPayment = new RemitaPayment
                            {
                                payment = payment,
                                OrderId = payment.InvoiceNumber,
                                RRR = payment.InvoiceNumber,
                                Status = "025",
                                Description = fee.Name,
                                TransactionDate = DateTime.Now,
                                TransactionAmount = viewModel.StudentVerification.Amount
                            };
                            var remitaPaymentLogic = new RemitaPaymentLogic();
                            remitaPaymentLogic.Create(remitaPayment);
                        }

                        transaction.Complete();
                    }
                    viewModel.StudentVerification.Payment = payment;

                    return View(viewModel);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("VerificationFees");
        }
        public ActionResult PayForTranscript()
        {
            viewModel = new TranscriptViewModel();
            try
            {
                ViewBag.Sessions = viewModel.SessionSelectList;

            }
            catch (Exception ex)
            {
                SetMessage(ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public JsonResult PayTranscriptFee(string con, int sessionId)
        {

            JsonResultModel result = new JsonResultModel();
            try
            {
                if (!string.IsNullOrEmpty(con) && sessionId > 0)
                {
                    Payment payment = InvalidConfirmationOrderNumber(con, sessionId);
                    if (payment != null)
                    {
                        TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                        TranscriptRequest transcriptRequest = transcriptRequestLogic.GetModelBy(s => s.Payment_Id == payment.Id);
                        transcriptRequest.transcriptStatus.TranscriptStatusId = (int)RequestStatus.RequestProcessed;
                        var modifiedTranscriptRequest = transcriptRequestLogic.Modify(transcriptRequest);
                        if (modifiedTranscriptRequest)
                        {
                            result.IsError = false;
                            result.Message = "Success!";
                            result.Operation = payment.Id.ToString();
                        }
                        result.IsError = false;
                        result.Message = "Success!";
                        result.Operation = payment.Id.ToString();
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "Payment could not be verified!";
                    }
                }
                else
                {
                    result.IsError = true;
                    result.Message = "One of the parameters was not set!";
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Payment InvalidConfirmationOrderNumber(string confirmationOrderNumber, int sessionId)
        {
            var payment = new Payment();
            var etranzactLogic = new PaymentEtranzactLogic();
            if (confirmationOrderNumber.Contains("ABSU"))
            {
                return payment;
            }
            PaymentEtranzact etranzactDetails = etranzactLogic.GetModelBy(m => m.Confirmation_No == confirmationOrderNumber);
            if (etranzactDetails == null || etranzactDetails.ReceiptNo == null)
            {
                var session = new Session { Id = sessionId };
                var paymentTerminal = new PaymentTerminal();
                var paymentTerminalLogic = new PaymentTerminalLogic();
                paymentTerminal = paymentTerminalLogic.GetModelBy(p => p.Fee_Type_Id == 13 && p.Session_Id == session.Id);

                etranzactDetails = etranzactLogic.RetrievePinAlternative(confirmationOrderNumber, paymentTerminal);
                if (etranzactDetails != null && etranzactDetails.ReceiptNo != null)
                {
                    var paymentLogic = new PaymentLogic();
                    payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                    if (payment != null && payment.Id > 0)
                    {
                        StudentLevel studentLevel = new StudentLevel();
                        StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                        studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id && s.Session_Id == sessionId).LastOrDefault();
                        if (studentLevel != null)
                        {
                            var feeDetailLogic = new FeeDetailLogic();
                            decimal feeDetail = feeDetailLogic.GetFeeByDepartmentLevel(studentLevel.Department, studentLevel.Level, studentLevel.Programme, payment.FeeType, payment.Session, payment.PaymentMode);
                            if (payment.Session.Id == session.Id)
                            {
                                if (!etranzactLogic.ValidatePin(etranzactDetails, payment, feeDetail))
                                {
                                    SetMessage("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.", Message.Category.Error);
                                    payment = null;
                                    return payment;
                                }
                            }
                        }

                        //PaymentLogic.SendDetailsToBIR(payment, etranzactDetails, "FORM");
                    }
                    else
                    {
                        SetMessage(
                            "The invoice number attached to the pin doesn't belong to you! Please cross check and try again.",
                            Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage(
                        "Confirmation Order Number entered seems not to be valid! Please cross check and try again.",
                        Message.Category.Error);
                }
            }
            else
            {
                var session = new Session { Id = sessionId };
                var paymentLogic = new PaymentLogic();
                payment = paymentLogic.GetModelBy(m => m.Invoice_Number == etranzactDetails.CustomerID);
                if (payment != null && payment.Id > 0)
                {
                    StudentLevel studentLevel = new StudentLevel();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == payment.Person.Id && s.Session_Id == sessionId).LastOrDefault();
                    if (studentLevel != null)
                    {
                        var feeDetailLogic = new FeeDetailLogic();
                        decimal feeDetail = feeDetailLogic.GetFeeByDepartmentLevel(studentLevel.Department, studentLevel.Level, studentLevel.Programme, payment.FeeType, payment.Session, payment.PaymentMode);
                        if (payment.Session.Id == session.Id)
                        {
                            if (!etranzactLogic.ValidatePin(etranzactDetails, payment, feeDetail))
                            {
                                SetMessage("The pin amount tied to the pin is not correct. Please contact support@lloydant.com.", Message.Category.Error);
                                payment = null;
                                return payment;
                            }
                        }
                        //PaymentLogic.SendDetailsToBIR(payment, etranzactDetails, "FORM");
                    }
                }
                else
                {
                    SetMessage("The invoice number attached to the pin doesn't belong to you! Please cross check and try again.", Message.Category.Error);
                }
            }

            return payment;
        }
        public JsonResult GetDeliveryServices(string countryId, string stateId)
        {
            try
            {
                List<DeliveryService> deliveryServicesInStateCountry = new List<DeliveryService>();

                if (!string.IsNullOrEmpty(countryId) && !string.IsNullOrEmpty(stateId))
                {
                    DeliveryServiceZoneLogic deliveryServiceZoneLogic = new DeliveryServiceZoneLogic();
                    DeliveryServiceLogic deliveryServiceLogic = new DeliveryServiceLogic();
                    StateGeoZoneLogic stateGeoZoneLogic = new StateGeoZoneLogic();

                    if (stateId != "OT")
                    {
                        List<StateGeoZone> stateGeoZones = stateGeoZoneLogic.GetModelsBy(s => s.State_Id == stateId && s.Activated);

                        for (int i = 0; i < stateGeoZones.Count; i++)
                        {
                            StateGeoZone stateGeoZone = stateGeoZones[i];
                            List<DeliveryServiceZone> deliveryServiceZones = deliveryServiceZoneLogic.GetModelsBy(s => s.Country_Id == countryId && s.Geo_Zone_Id == stateGeoZone.GeoZone.Id && s.Activated);

                            List<DeliveryService> deliveryServices = deliveryServiceLogic.GetModelsBy(s => s.Activated);


                            for (int j = 0; j < deliveryServices.Count; j++)
                            {
                                DeliveryService deliveryService = deliveryServices[j];
                                if (deliveryServiceZones.Count(s => s.DeliveryService.Id == deliveryService.Id) > 0)
                                {
                                    if (deliveryServicesInStateCountry.Count(s => s.Id == deliveryService.Id) <= 0)
                                    {

                                        deliveryServicesInStateCountry.Add(deliveryService);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        List<DeliveryServiceZone> deliveryServiceZones = deliveryServiceZoneLogic.GetModelsBy(s => s.Country_Id == countryId && s.Activated);
                        List<DeliveryService> deliveryServices = deliveryServiceLogic.GetModelsBy(s => s.Activated);

                        List<int> distinctDeliveryServiceId = deliveryServices.Select(d => d.Id).Distinct().ToList();

                        for (int j = 0; j < distinctDeliveryServiceId.Count; j++)
                        {
                            int currentDeliveryServiceId = distinctDeliveryServiceId[j];
                            DeliveryService deliveryService = deliveryServices.LastOrDefault(d => d.Id == currentDeliveryServiceId);
                            if (deliveryService != null)
                            {
                                if (deliveryServiceZones.Count(s => s.DeliveryService.Id == deliveryService.Id) > 0)
                                {
                                    if (deliveryServicesInStateCountry.Count(s => s.Id == deliveryService.Id) <= 0)
                                    {
                                        deliveryServicesInStateCountry.Add(deliveryService);
                                    }

                                }
                            }

                        }

                    }
                }

                return Json(new SelectList(deliveryServicesInStateCountry, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public JsonResult GetDeliveryServiceZones(string countryId, string stateId, int deliveryServiceId)
        {
            try
            {
                List<DeliveryServiceZone> deliveryServiceZones = new List<DeliveryServiceZone>();

                if (!string.IsNullOrEmpty(countryId) && !string.IsNullOrEmpty(stateId) && deliveryServiceId > 0)
                {
                    DeliveryServiceZoneLogic deliveryServiceZoneLogic = new DeliveryServiceZoneLogic();
                    StateGeoZoneLogic stateGeoZoneLogic = new StateGeoZoneLogic();
                    FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                    List<StateGeoZone> stateGeoZones = stateGeoZoneLogic.GetModelsBy(s => s.State_Id == stateId && s.Activated);
                    if (stateId == "OT")
                    {
                        List<int> feeTypes = new List<int>
                        {
                            (int) FeeTypes.IntlTranscriptRequestZone1,
                            (int) FeeTypes.IntlTranscriptRequestZone2,
                            (int) FeeTypes.IntlTranscriptRequestZone3,
                            (int) FeeTypes.IntlTranscriptRequestZone4,
                            (int) FeeTypes.IntlTranscriptRequestZone5,
                            (int) FeeTypes.IntlTranscriptRequestZone6,
                            (int) FeeTypes.IntlTranscriptRequestZone7,
                            (int) FeeTypes.IntlTranscriptRequestZone8,
                        };
                        List<DeliveryServiceZone> currentDeliveryServiceZones = deliveryServiceZoneLogic.GetModelsBy(s => s.Country_Id == countryId && s.Delivery_Service_Id == deliveryServiceId && s.Activated);
                        List<FeeDetail> feeDetails = feeDetailLogic.GetModelsBy(f => feeTypes.Contains((int)f.Fee_Type_Id));

                        for (int j = 0; j < currentDeliveryServiceZones.Count; j++)
                        {
                            int currentFeeTypeId = currentDeliveryServiceZones[j].FeeType.Id;

                            FeeDetail feeDetail = feeDetails.FirstOrDefault(s => s.FeeType.Id == currentFeeTypeId && !s.Fee.Name.Contains("Transcript"));
                            if (feeDetail != null)
                            {
                                currentDeliveryServiceZones[j].Name = currentDeliveryServiceZones[j].GeoZone.Name + " - " + feeDetail.Fee.Amount;
                            }
                            else
                            {
                                currentDeliveryServiceZones[j].Name = currentDeliveryServiceZones[j].GeoZone.Name;
                            }

                            DeliveryServiceZone serviceZone = currentDeliveryServiceZones[j];

                            if (deliveryServiceZones.Count(s => s.Id == serviceZone.Id) <= 0)
                            {
                                deliveryServiceZones.Add(currentDeliveryServiceZones[j]);
                            }

                        }
                    }
                    else
                    {

                        for (int j = 0; j < stateGeoZones.Count; j++)
                        {
                            StateGeoZone stateGeoZone = stateGeoZones[j];
                            List<DeliveryServiceZone> currentDeliveryServiceZones = deliveryServiceZoneLogic.GetModelsBy(s => s.Country_Id == countryId && s.Geo_Zone_Id == stateGeoZone.GeoZone.Id && s.Delivery_Service_Id == deliveryServiceId && s.Activated);
                            for (int k = 0; k < currentDeliveryServiceZones.Count; k++)
                            {
                                int currentFeeTypeId = currentDeliveryServiceZones[k].FeeType.Id;
                                FeeDetail feeDetail = feeDetailLogic.GetModelsBy(s => s.Fee_Type_Id == currentFeeTypeId && !s.FEE.Fee_Name.Contains("Transcript")).FirstOrDefault();
                                if (feeDetail != null)
                                    currentDeliveryServiceZones[k].Name = stateGeoZone.GeoZone.Name + " - " + feeDetail.Fee.Amount;

                                DeliveryServiceZone serviceZone = currentDeliveryServiceZones[k];

                                if (deliveryServiceZones.Count(s => s.Id == serviceZone.Id) <= 0)
                                {
                                    deliveryServiceZones.Add(currentDeliveryServiceZones[k]);
                                }

                            }

                        }
                    }
                }
                var orderedDeliveryServiceZone = deliveryServiceZones.OrderBy(s => s.GeoZone.Name);
                return Json(new SelectList(orderedDeliveryServiceZone, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public JsonResult PostInterswitchDetails(string txnRef, string amount)
        {
            bool isCreated = false;
            try
            {
               
                if (!string.IsNullOrEmpty(txnRef) && !string.IsNullOrEmpty(amount))
                {
                    PaymentLogic paymentLogic = new PaymentLogic();
                    PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();


                    if (txnRef.Contains("_"))
                    {
                        string invoiceNumber = txnRef.Split('_').FirstOrDefault();
                        Payment payment = paymentLogic.GetBy(invoiceNumber);

                        PaymentInterswitch paymentInterswitch = new PaymentInterswitch
                        {
                            Payment = payment,
                            Amount = Convert.ToInt32(amount) - 25000,
                            TransactionDate = DateTime.Now,
                            ResponseCode = "Z0",
                            ResponseDescription = "Transaction status unconfirmed",
                            MerchantReference = txnRef
                        };
                        var createdPaymentDetails = paymentInterswitchLogic.Create(paymentInterswitch);
                        if (createdPaymentDetails != null)
                        {
                            isCreated = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                isCreated = false;
            }
            return Json(isCreated, JsonRequestBehavior.AllowGet);
        }
    }
    
}