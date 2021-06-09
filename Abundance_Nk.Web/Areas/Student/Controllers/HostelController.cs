using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Student.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;

namespace Abundance_Nk.Web.Areas.Student.Controllers
{
    [AllowAnonymous]
    public class HostelController : BaseController
    {
        private HostelViewModel viewModel;
        private bool abaSpecial = false;
        private long abaSpecialId;
        public ActionResult CreateHostelRequest()
        {
            viewModel = new HostelViewModel();
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult CreateHostelRequest(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.Student.MatricNumber != null)
                {
                    HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                    SessionLogic sessionLogic = new SessionLogic();
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    StudentLogic studentLogic = new StudentLogic();
                    PersonLogic personLogic = new PersonLogic();
                    AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                    Model.Model.Student student = new Model.Model.Student();
                    Person person = new Person();
                    StudentLevel studentLevel = new StudentLevel();
                    Programme programme = new Programme();
                    Department department = new Department();
                    Level level = new Level();
                    var activeSession=sessionLogic.GetModelsBy(g => g.Active_Hostel_Application).LastOrDefault();
                    Session session = new Session() { Id = activeSession.Id };

                    PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelsBy(p => p.Confirmation_No == viewModel.Student.MatricNumber &&
                                                        (p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees) && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id).LastOrDefault();
                    if (paymentEtranzact == null)
                    {
                        SetMessage("Confirmation order number is not for current session's school fee!", Message.Category.Error);
                        return View(viewModel);
                    }

                    StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                    StudentPayment studentPayment = studentPaymentLogic.GetModelBy(s => s.Session_Id == session.Id && s.Payment_Id == paymentEtranzact.Payment.Payment.Id);

                    person = paymentEtranzact.Payment.Payment.Person;
                    student = studentLogic.GetModelBy(s => s.Person_Id == person.Id);
                    studentLevel = studentLevelLogic.GetModelsBy(sl => sl.STUDENT.Person_Id == person.Id).LastOrDefault();
                    AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id);

                    if (appliedCourse != null)
                    {
                        programme = appliedCourse.Programme;
                        department = appliedCourse.Department;
                        if (studentPayment == null && appliedCourse.ApplicationForm.Setting.Session.Id == session.Id)
                        {
                            level = new Level() { Id = 1 };
                        }
                        else if (studentPayment != null)
                        {
                            level = studentPayment.Level;
                        }
                        else
                        {
                            if (student != null)
                            {
                                if (studentLevel != null)
                                {
                                    programme = studentLevel.Programme;
                                    department = studentLevel.Department;
                                    level = studentLevel.Level;
                                }
                            }
                            else
                            {
                                SetMessage("Student Level not set, contact school ICS!", Message.Category.Error);
                                return View(viewModel);
                            }
                        }
                    }
                   
                    if (student == null && person == null)
                    {
                        SetMessage("Student record not found.", Message.Category.Error);
                        return View(viewModel);
                    }

                    //List<PaymentEtranzact> paymentEtranzacts = paymentEtranzactLogic.GetModelsBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == student.Id && p.ONLINE_PAYMENT.PAYMENT.Session_Id == session.Id && p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees);

                    //if (paymentEtranzacts.Count == 0)
                    //{
                    //    SetMessage("Pay School Fees before making hostel request!", Message.Category.Error);
                    //    return View(viewModel);
                    //}

                    if (student != null)
                    {
                        if (studentLevel != null)
                        {
                            programme = studentLevel.Programme;
                            department = studentLevel.Department;
                            level = studentLevel.Level;
                        }
                    }
                    HostelRequest hostelRequest = hostelRequestLogic.GetModelBy(h => h.Person_Id == person.Id && h.Session_Id == session.Id);
                   
                    if (studentLevel != null && studentLevel.Department.Id == 50 && studentLevel.Level.Id >= 4 && hostelRequest == null)
                    {
                        hostelRequest = new HostelRequest();
                        hostelRequest.Approved = true;
                        hostelRequest.Department = department;
                        hostelRequest.Programme = programme;
                        hostelRequest.RequestDate = DateTime.Now;
                        hostelRequest.Session = session;
                        hostelRequest.Student = student;
                        hostelRequest.Person = person;
                        hostelRequest.Level = level;

                        hostelRequestLogic.Create(hostelRequest);

                        HostelAllocationStatus(viewModel);

                        if (abaSpecial)
                        {
                            return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Utility.Encrypt(abaSpecialId.ToString()), });

                        }

                        return View(viewModel);
                    }
                    else if (hostelRequest == null)
                    {
                        hostelRequest = new HostelRequest();
                        hostelRequest.Approved = false;
                        hostelRequest.Department = department;
                        hostelRequest.Programme = programme;
                        hostelRequest.RequestDate = DateTime.Now;
                        hostelRequest.Session = session;
                        hostelRequest.Student = student;
                        hostelRequest.Person = person;
                        hostelRequest.Level = level;

                        hostelRequestLogic.Create(hostelRequest);

                        SetMessage("Your request has been submitted!", Message.Category.Information);
                        return View(viewModel);
                    }
                    if (hostelRequest != null && hostelRequest.Approved)
                    {
                        SetMessage("Your request has been approved proceed to generate invoice!", Message.Category.Information);
                        return View(viewModel);
                    }
                    if (hostelRequest != null && !hostelRequest.Approved)
                    {
                        SetMessage("Your request has not been approved!", Message.Category.Error);
                        return View(viewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }


        public ActionResult HostelAllocationStatus()
        {
            viewModel = new HostelViewModel();
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

        private void SetFeeTypeDropDown(HostelViewModel viewModel)
        {
            try
            {
                FeeTypeLogic feeTypeLogic = new FeeTypeLogic();
                if (viewModel.FeeTypeSelectListItem != null && viewModel.FeeTypeSelectListItem.Count > 0)
                {
                    viewModel.FeeType = feeTypeLogic.GetModelBy(ft => ft.Fee_Type_Id == (int)FeeTypes.HostelFee);
                    ViewBag.FeeTypes = new SelectList(viewModel.FeeTypeSelectListItem, Utility.VALUE, Utility.TEXT, viewModel.FeeType.Id);
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
        public ActionResult HostelAllocationStatus(HostelViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();
                HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                HostelAllocationCriteriaLogic hostelAllocationCriteriaLogic = new HostelAllocationCriteriaLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                HostelAllocationCountLogic hostelAllocationCountLogic = new HostelAllocationCountLogic();
                HostelRequestLogic hostelRequestLogic = new HostelRequestLogic();
                StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                AdmissionListLogic admissionListLogic = new AdmissionListLogic();

                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                Programme programme = new Programme();
                Department department = new Department();
                Level level = new Level();

                Model.Model.Student student = new Model.Model.Student();
                Person person = new Person();
                Payment payment = new Payment();
                StudentLevel studentLevel = new StudentLevel();
                HostelAllocation hostelAllocation = new HostelAllocation();
                HostelAllocation existingHostelAllocation = new HostelAllocation();
                SessionLogic sessionLogic = new SessionLogic();
                var activeSession = sessionLogic.GetModelsBy(g => g.Active_Hostel_Application).LastOrDefault();
                viewModel.Session = new Session() { Id = activeSession.Id };
                //viewModel.Session = new Session() { Id = 18 };
                List<HostelAllocationCriteria> hostelAllocationCriteriaList = new List<HostelAllocationCriteria>();

                PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelsBy(p => p.Confirmation_No == viewModel.Student.MatricNumber &&
                                                    (p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees) && p.ONLINE_PAYMENT.PAYMENT.Session_Id == viewModel.Session.Id).LastOrDefault();
                if (paymentEtranzact == null)
                {
                    SetMessage("Confirmation order number is not for current session's school fee!", Message.Category.Error);
                    return View(viewModel);
                }

                person = paymentEtranzact.Payment.Payment.Person;

                StudentPayment studentPayment = studentPaymentLogic.GetModelBy(s => s.Person_Id == person.Id && s.Session_Id == viewModel.Session.Id &&
                                                s.Payment_Id == paymentEtranzact.Payment.Payment.Id);

                AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id);
                if (appliedCourse != null)
                {
                    AdmissionList admissionList = admissionListLogic.GetModelBy(s => s.Application_Form_Id == appliedCourse.ApplicationForm.Id);
                    if (studentPayment == null && appliedCourse.ApplicationForm.Setting.Session.Id == viewModel.Session.Id)
                    {
                        level = new Level() { Id = 1 };
                        programme = admissionList.Programme;
                        department = admissionList.Deprtment;
                    }
                    else if (studentPayment != null)
                    {
                        level = studentPayment.Level;
                    }
                    else
                    {
                        SetMessage("Student Level not set, contact school ICS!", Message.Category.Error);
                        SetFeeTypeDropDown(viewModel);
                        return View(viewModel);
                    }
                }

                student = studentLogic.GetModelBy(s => s.Person_Id == person.Id);
                if (student != null)
                {
                    studentLevel = studentLevelLogic.GetModelsBy(sl => sl.STUDENT.Person_Id == student.Id).LastOrDefault();
                    if (studentPayment != null && studentLevel != null)
                    {
                        programme = studentLevel.Programme;
                        department = studentLevel.Department;
                        level = studentPayment.Level;
                    }
                }

                viewModel.Person = person;
                viewModel.StudentLevel = new StudentLevel();
                viewModel.StudentLevel.Programme = programme;
                viewModel.StudentLevel.Department = department;
                viewModel.StudentLevel.Level = level;

                Campus studentCampus = GetStudentCampus(viewModel.StudentLevel) ?? new Campus { Id = (int)Campuses.Uturu };

                HostelRequest hostelRequest = hostelRequestLogic.GetModelBy(h => h.Person_Id == person.Id && h.Session_Id == viewModel.Session.Id);
                if (hostelRequest == null)
                {
                    SetMessage("You have not requested for hostel allocation!", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }
                if (hostelRequest != null && !hostelRequest.Approved)
                {
                    SetMessage("Your request for hostel allocation has not been approved!", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }

                //int[] excludedDepartments = { 50, 42, 66 };

                int[] medSurgLevels = { 4, 5, 6 };
                int[] lawLevels = { 1, 2, 3, 4, 5 };
                int[] agricLevels = { 2, 3, 4, 5 };

                //if (excludedDepartments.Contains(department.Id))
                //{
                //    if (department.Id == 50 && medSurgLevels.Contains(level.Id))
                //    {
                //        SetMessage("Your hostel details has not been set contact school ICS!", Message.Category.Error);
                //        SetFeeTypeDropDown(viewModel);
                //        return View(viewModel);
                //    }
                //    //if (department.Id == 42 && lawLevels.Contains(level.Id))
                //    //{
                //    //    SetMessage("Your hostel details has not been set contact school ICS!", Message.Category.Error);
                //    //    SetFeeTypeDropDown(viewModel);
                //    //    return View(viewModel);
                //    //}
                //    //if (department.Id == 66 && agricLevels.Contains(level.Id))
                //    //{
                //    //    SetMessage("Your hostel details has not been set contact school ICS!", Message.Category.Error);
                //    //    SetFeeTypeDropDown(viewModel);
                //    //    return View(viewModel);
                //    //}
                //}

                existingHostelAllocation = hostelAllocationLogic.GetModelBy(ha => ha.Session_Id == viewModel.Session.Id && ha.Student_Id == person.Id);
                if (existingHostelAllocation != null)
                {
                    if (existingHostelAllocation.Occupied)
                    {
                        return RedirectToAction("HostelReceipt", new { spmid = existingHostelAllocation.Payment.Id });
                    }
                    else
                    {
                        if (level.Id != 1)
                        {
                            abaSpecialId = existingHostelAllocation.Payment.Id;
                            abaSpecial = true;

                            return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Utility.Encrypt(existingHostelAllocation.Payment.Id.ToString()), });

                        }
                        else
                        {
                            return RedirectToAction("HostelReceipt", new { spmid = existingHostelAllocation.Payment.Id });
                        }
                    }
                }

                if (person.Sex == null)
                {
                    SetMessage("Error! Ensure that your student profile(Sex) is completely filled", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }

                HostelAllocationCount hostelAllocationCount = new HostelAllocationCount();
                if (level.Id == 1)
                {
                    hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Sex_Id == person.Sex.Id && h.Level_Id == level.Id && h.Campus_Id == studentCampus.Id);
                }
                else
                {
                    hostelAllocationCount = hostelAllocationCountLogic.GetModelBy(h => h.Sex_Id == person.Sex.Id && h.Level_Id != 1 && h.Campus_Id == studentCampus.Id);
                }

                if (hostelAllocationCount != null && hostelAllocationCount.Free == 0)
                {
                    SetMessage("Error! The set number of hostel allocation for your level has been exausted!", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }

                int[] umahiaHostels = { 13, 14 };
                int[] otherHostels = { 2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 21 };
                int[] abaHostels = { 15, 16, 17, 19, 20 };

                if ((department.Id == 42 && lawLevels.Contains(level.Id)) || (department.Faculty.Id == 10 && agricLevels.Contains(level.Id)))
                {
                    hostelAllocationCriteriaList = hostelAllocationCriteriaLogic.GetModelsBy(hac => hac.Level_Id == level.Id && umahiaHostels.Contains(hac.Hostel_Id) &&
                                                    hac.HOSTEL.HOSTEL_TYPE.Hostel_Type_Name == person.Sex.Name && hac.HOSTEL_ROOM.Reserved == false && hac.HOSTEL_ROOM.Activated &&
                                                    hac.HOSTEL.Activated && hac.HOSTEL_SERIES.Activated && hac.HOSTEL_ROOM_CORNER.Activated);
                }
                else if (department.Id == 50 && medSurgLevels.Contains(level.Id))
                {
                    hostelAllocationCriteriaList = hostelAllocationCriteriaLogic.GetModelsBy(hac => hac.Level_Id == level.Id && abaHostels.Contains(hac.Hostel_Id) &&
                                                    hac.HOSTEL.HOSTEL_TYPE.Hostel_Type_Name == person.Sex.Name && hac.HOSTEL_ROOM.Reserved == false && hac.HOSTEL_ROOM.Activated &&
                                                    hac.HOSTEL.Activated && hac.HOSTEL_SERIES.Activated && hac.HOSTEL_ROOM_CORNER.Activated);
                }
                else
                {
                    hostelAllocationCriteriaList = hostelAllocationCriteriaLogic.GetModelsBy(hac => hac.Level_Id == level.Id && otherHostels.Contains(hac.Hostel_Id) &&
                                                    hac.HOSTEL.HOSTEL_TYPE.Hostel_Type_Name == person.Sex.Name && hac.HOSTEL_ROOM.Reserved == false && hac.HOSTEL_ROOM.Activated &&
                                                    hac.HOSTEL.Activated && hac.HOSTEL_SERIES.Activated && hac.HOSTEL_ROOM_CORNER.Activated);
                }

                if (hostelAllocationCriteriaList.Count == 0)
                {
                    SetMessage("Hostel Allocation Criteria for your level has not been set!", Message.Category.Error);
                    SetFeeTypeDropDown(viewModel);
                    return View(viewModel);
                }

                int count = 0;
                for (int i = 0; i < hostelAllocationCriteriaList.Count; i++)
                {
                    hostelAllocation.Corner = hostelAllocationCriteriaList[i].Corner;
                    hostelAllocation.Hostel = hostelAllocationCriteriaList[i].Hostel;
                    if (level.Id == 1)
                    {
                        hostelAllocation.Occupied = true;
                    }
                    else
                    {
                        hostelAllocation.Occupied = false;
                    }
                    hostelAllocation.Room = hostelAllocationCriteriaList[i].Room;
                    hostelAllocation.Series = hostelAllocationCriteriaList[i].Series;
                    hostelAllocation.Session = viewModel.Session;
                    hostelAllocation.Student = student;
                    hostelAllocation.Person = person;

                    HostelAllocation allocationCheck = hostelAllocationLogic.GetModelsBy(h => h.Corner_Id == hostelAllocation.Corner.Id && h.Hostel_Id == hostelAllocation.Hostel.Id &&
                                                        h.Room_Id == hostelAllocation.Room.Id && h.Series_Id == hostelAllocation.Series.Id && h.Session_Id == hostelAllocation.Session.Id).FirstOrDefault();
                    if (allocationCheck != null)
                    {
                        count += 1;
                        continue;

                    }

                    payment = paymentLogic.GetModelsBy(s => s.Person_Id == person.Id && s.Fee_Type_Id == (int)FeeTypes.HostelFee && s.Session_Id == viewModel.Session.Id).LastOrDefault();

                    if (payment != null)
                    {
                        return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Utility.Encrypt(payment.Id.ToString()) });
                    }
                    using (TransactionScope scope = new TransactionScope())
                    {
                        payment = CreatePayment(viewModel);

                        StudentPayment currentStudentPayment = new StudentPayment();
                        currentStudentPayment.Level = level;
                        currentStudentPayment.Amount = payment.FeeDetails.Sum(f => f.Fee.Amount);
                        if (level.Id == 1)
                        {
                            currentStudentPayment.Status = true;
                        }
                        else
                        {
                            currentStudentPayment.Status = false;
                        }

                        currentStudentPayment.Student = student;
                        currentStudentPayment.Person = person;
                        currentStudentPayment.Session = viewModel.Session;
                        currentStudentPayment.Id = payment.Id;

                        StudentPayment existingStudentPayment = studentPaymentLogic.GetModelBy(p => p.Payment_Id == payment.Id);

                        if (existingStudentPayment == null)
                        {
                            if (level.Id == 1 && student == null)
                            {
                                //do nothing
                            }
                            else
                            {
                                studentPaymentLogic.Create(currentStudentPayment);
                            }
                        }

                        hostelAllocation.Payment = payment;

                        HostelAllocation newHostelAllocation = hostelAllocationLogic.Create(hostelAllocation);

                        hostelAllocationCount.Free -= 1;
                        hostelAllocationCount.TotalCount -= 1;
                        hostelAllocationCount.LastModified = DateTime.Now;
                        hostelAllocationCountLogic.Modify(hostelAllocationCount);

                        scope.Complete();
                    }

                    if (level.Id == 1)
                    {
                        return RedirectToAction("HostelReceipt", new { spmid = payment.Id });
                    }

                    viewModel.Student = student;
                    viewModel.StudentLevel = studentLevel;
                    viewModel.Payment = payment;
                    viewModel.Payment.FeeType = new FeeType() { Id = (int)FeeTypes.HostelFee };
                    TempData["ViewModel"] = viewModel;

                    abaSpecialId = payment.Id;
                    abaSpecial = true;

                    //return RedirectToAction("GenerateInvoice", "Payment", new { sid = Utility.Encrypt(person.Id.ToString()), fid = Utility.Encrypt(Convert.ToString((int)FeeTypes.HostelFee)) });
                    return RedirectToAction("Invoice", "Credential", new { Area = "Common", pmid = Utility.Encrypt(payment.Id.ToString()) });

                }
                if (count == hostelAllocationCriteriaList.Count)
                {
                    SetMessage("Error! The set number of hostel allocation for your level has been exausted!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            SetFeeTypeDropDown(viewModel);
            return View(viewModel);
        }

        private Campus GetStudentCampus(StudentLevel studentLevel)
        {
            Campus campus = null;
            try
            {
                int[] lawLevels = { 1, 2, 3, 4, 5 };

                if (studentLevel != null && studentLevel.Department.Id == 50 && studentLevel.Level.Id >= 4)
                {
                    campus = new Campus { Id = (int)Campuses.Aba };
                }
                else if (studentLevel != null && ((studentLevel.Department.Id == 42 && lawLevels.Contains(studentLevel.Level.Id)) ||
                    (studentLevel.Department.Faculty.Id == 10 && studentLevel.Level.Id >= 2)))
                {
                    campus = new Campus { Id = (int)Campuses.Umuahia };
                }
                else
                {
                    campus = new Campus { Id = (int)Campuses.Uturu };
                }
            }
            catch (Exception)
            {
                throw;
            }

            return campus;
        }

        private Payment CreatePayment(HostelViewModel viewModel)
        {
            try
            {
                PaymentLogic paymentLogic = new PaymentLogic();
                OnlinePaymentLogic onlinePaymentLogic = new OnlinePaymentLogic();

                Payment newPayment = new Payment();

                PaymentMode paymentMode = new PaymentMode() { Id = 1 };
                PaymentType paymentType = new PaymentType() { Id = 2 };
                PersonType personType = viewModel.Person.Type;
                FeeType feeType = new FeeType() { Id = (int)FeeTypes.HostelFee };

                Payment payment = new Payment();
                payment.PaymentMode = paymentMode;
                payment.PaymentType = paymentType;
                payment.PersonType = personType;
                payment.FeeType = feeType;
                payment.DatePaid = DateTime.Now;
                payment.Person = viewModel.Person;
                payment.Session = viewModel.Session;

                Payment checkPayment = paymentLogic.GetModelBy(p => p.Person_Id == viewModel.Person.Id && p.Fee_Type_Id == feeType.Id && p.Session_Id == viewModel.Session.Id);
                if (checkPayment != null)
                {
                    newPayment = checkPayment;
                }
                else
                {
                    newPayment = paymentLogic.Create(payment);
                }

                newPayment.FeeDetails = paymentLogic.SetFeeDetails(newPayment, viewModel.StudentLevel.Programme.Id,
                    viewModel.StudentLevel.Level.Id, paymentMode.Id, viewModel.StudentLevel.Department.Id,
                    viewModel.Session.Id);

                OnlinePayment newOnlinePayment = null;

                if (newPayment != null)
                {
                    OnlinePayment onlinePaymentCheck = onlinePaymentLogic.GetModelBy(op => op.Payment_Id == newPayment.Id);
                    if (onlinePaymentCheck == null)
                    {
                        PaymentChannel channel = new PaymentChannel() { Id = (int)PaymentChannel.Channels.Etranzact };
                        OnlinePayment onlinePayment = new OnlinePayment();
                        onlinePayment.Channel = channel;
                        onlinePayment.Payment = newPayment;
                        newOnlinePayment = onlinePaymentLogic.Create(onlinePayment);
                    }

                }

                //HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                //HostelFee hostelFee = new HostelFee();

                //hostelFee.Hostel = hostel;
                //hostelFee.Payment = newPayment;
                //hostelFee.Amount = GetHostelFee(hostel);

                //hostelFeeLogic.Create(hostelFee);

                //newPayment.Amount = GetHostelFee(hostel).ToString();

                return newPayment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private double GetHostelFee(Hostel hostel)
        {
            double amount = 0;
            try
            {
                string[] firstHostelGroup = { "Hostel A", "Aguyi Ironsi (Hostel F)", "Onyibaja (Hostel G)", "Nnanna Kalu (Hostel E)", "Hostel H", "Hostel C", "ALGON", "Hostel B" };
                string[] secondHostelGroup = { "QUEENS PALACE I", "QUEENS PALACE II", "QUEENS PALACE III" };

                if (firstHostelGroup.Contains(hostel.Name))
                {
                    amount = 15000;
                }
                if (secondHostelGroup.Contains(hostel.Name))
                {
                    amount = 15000;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return amount;
        }
        public ActionResult Invoice()
        {
            viewModel = (HostelViewModel)TempData["ViewModel"];
            try
            {
                //Int64 paymentid = Convert.ToInt64(Abundance_Nk.Web.Models.Utility.Decrypt(pmid));
                PaymentLogic paymentLogic = new PaymentLogic();
                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                Payment payment = paymentLogic.GetModelBy(p => p.Payment_Id == viewModel.Payment.Id);
                if (payment != null && payment.FeeType.Id == (int)FeeTypes.HostelFee)
                {
                    Model.Model.Student student = new Model.Model.Student();
                    StudentLogic studentLogic = new StudentLogic();
                    student = studentLogic.GetBy(payment.Person.Id);
                    StudentLevel studentLevel = studentLevelLogic.GetModelsBy(s => s.Person_Id == student.Id).LastOrDefault();

                    payment.FeeDetails = feeDetailLogic.GetModelsBy(f => f.Fee_Type_Id == (int)FeeTypes.HostelFee && f.Department_Id == studentLevel.Department.Id && f.Programme_Id == studentLevel.Programme.Id && f.Session_Id == 7 && f.Level_Id == studentLevel.Level.Id);
                    Invoice invoice = new Invoice();
                    invoice.Person = payment.Person;
                    invoice.Payment = payment;


                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    PaymentEtranzactType paymentEtranzactType = new PaymentEtranzactType();
                    PaymentEtranzactTypeLogic PaymentEtranzactTypeLogic = new Business.PaymentEtranzactTypeLogic();

                    paymentEtranzactType = PaymentEtranzactTypeLogic.GetModelsBy(p => p.Fee_Type_Id == payment.FeeType.Id && p.Session_Id == payment.Session.Id && p.Programme_Id == studentLevel.Programme.Id && p.Level_Id == studentLevel.Level.Id && p.Payment_Mode_Id == 1).LastOrDefault();

                    if (student != null)
                    {
                        invoice.MatricNumber = student.MatricNumber;
                    }

                    invoice.paymentEtranzactType = paymentEtranzactType;

                    PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetBy(payment);
                    if (paymentEtranzact != null)
                    {
                        invoice.Paid = true;
                    }

                    HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                    HostelFee hostelFee = hostelFeeLogic.GetModelsBy(h => h.Payment_Id == payment.Id).LastOrDefault();

                    invoice.Amount = Convert.ToDecimal(hostelFee.Amount);

                    //invoice.Payment.FeeDetails = null;

                    if (payment.FeeDetails.Count == 0)
                    {
                        payment.FeeDetails.Add(new FeeDetail() { Fee = new Fee() { Amount = invoice.Amount } });
                    }

                    return View(invoice);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

        public ActionResult PayHostelFee()
        {
            try
            {
                viewModel = new HostelViewModel();
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        [HttpPost]
        public ActionResult PayHostelFee(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel.ConfirmationOrder != null)
                {
                    PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                    PaymentLogic paymentLogic = new PaymentLogic();
                    HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                    SessionLogic sessionLogic = new SessionLogic();

                    if (viewModel.ConfirmationOrder.Length > 12)
                    {
                        var activeSession = sessionLogic.GetModelsBy(g => g.Active_Hostel_Application).LastOrDefault();
                        Session session = new Session() { Id = activeSession.Id };
                        //Model.Model.Session session = new Model.Model.Session() { Id = 18 };
                        FeeType feetype = new FeeType() { Id = (int)FeeTypes.HostelFee };

                        Payment payment = paymentLogic.InvalidConfirmationOrderNumber(viewModel.ConfirmationOrder, session, feetype);
                        if (payment == null)
                        {
                            SetMessage("Confirmation Order Number (" + viewModel.ConfirmationOrder + ") entered is not for the payment! Please ensure that you entered the correct Confirmation Order Number.", Message.Category.Error);
                            return View(viewModel);
                        }

                        if (payment != null && payment.Id > 0)
                        {
                            if (payment.FeeType.Id != (int)FeeTypes.HostelFee)
                            {
                                SetMessage("Confirmation Order Number (" + viewModel.ConfirmationOrder + ") entered is not for Hostel Fee payment! Please enter your Hostel Fee Confirmation Order Number.", Message.Category.Error);
                                return View(viewModel);
                            }

                            HostelAllocation hostelAllocation = hostelAllocationLogic.GetModelBy(ha => ha.Student_Id == payment.Person.Id && ha.Session_Id == payment.Session.Id);
                            if (hostelAllocation != null)
                            {
                                hostelAllocation.Occupied = true;
                                hostelAllocationLogic.Modify(hostelAllocation);
                            }

                            return RedirectToAction("HostelReceipt", new { spmid = payment.Id });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult HostelReceipt(long spmid)
        {
            try
            {
                viewModel = new HostelViewModel();
                HostelAllocationLogic hostelAllocationLogic = new HostelAllocationLogic();
                PaymentLogic paymentLogic = new PaymentLogic();
                StudentLogic studentLogic = new StudentLogic();
                StudentPaymentLogic studentPaymentLogic = new StudentPaymentLogic();
                StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
                //HostelFeeLogic hostelFeeLogic = new HostelFeeLogic();
                //HostelFee hostelFee = new HostelFee();
                Payment payment = new Payment();
                Model.Model.Student student = new Model.Model.Student();
                Person person = new Model.Model.Person();
                Programme programme = new Programme();
                Department department = new Department();
                Level level = new Level();

                payment = paymentLogic.GetModelBy(p => p.Payment_Id == spmid);
                person = payment.Person;
                HostelAllocation hostelAllocation = hostelAllocationLogic.GetModelBy(ha => ha.Payment_Id == spmid && ha.Session_Id == payment.Session.Id && ha.Student_Id == payment.Person.Id);

                student = studentLogic.GetModelBy(s => s.Person_Id == payment.Person.Id);
                if (student != null)
                {
                    StudentPayment studentPayment = studentPaymentLogic.GetModelBy(s => s.Person_Id == student.Id && s.Session_Id == payment.Session.Id && s.Payment_Id == payment.Id);
                    StudentLevel studentLevel = studentLevelLogic.GetModelsBy(sl => sl.STUDENT.Person_Id == student.Id).LastOrDefault();
                    if (studentPayment != null && studentLevel != null)
                    {
                        programme = studentLevel.Programme;
                        department = studentLevel.Department;
                        level = studentPayment.Level;
                    }

                    if (studentLevel != null)
                    {
                        programme = studentLevel.Programme;
                        department = studentLevel.Department;
                    }

                    if (studentPayment == null)
                    {
                        PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();

                        PaymentEtranzact paymentEtranzact = paymentEtranzactLogic.GetModelsBy(p => p.ONLINE_PAYMENT.PAYMENT.Person_Id == student.Id && (p.ONLINE_PAYMENT.PAYMENT.Fee_Type_Id == (int)FeeTypes.SchoolFees) && p.ONLINE_PAYMENT.PAYMENT.Session_Id == payment.Session.Id).LastOrDefault();

                        if (paymentEtranzact != null)
                        {
                            StudentPayment studentSchoolFeePayment = studentPaymentLogic.GetModelBy(s => s.Person_Id == student.Id && s.Session_Id == payment.Session.Id && s.Payment_Id == paymentEtranzact.Payment.Payment.Id);

                            if (studentSchoolFeePayment == null)
                            {
                                AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == student.Id && a.APPLICATION_FORM.APPLICATION_FORM_SETTING.Session_Id == payment.Session.Id);

                                if (appliedCourse != null)
                                {
                                    level = new Level() { Id = 1 };
                                    department = appliedCourse.Department;
                                    programme = appliedCourse.Programme;
                                }
                            }
                            else
                            {
                                level = studentSchoolFeePayment.Level;
                            }

                            StudentPayment currentStudentPayment = new StudentPayment();

                            currentStudentPayment.Level = level;
                            currentStudentPayment.Amount = paymentLogic.SetFeeDetails(payment, programme.Id, level.Id, payment.PaymentMode.Id, department.Id, payment.Session.Id).Sum(f => f.Fee.Amount);
                            if (level.Id == 1)
                            {
                                currentStudentPayment.Status = true;
                            }
                            else
                            {
                                currentStudentPayment.Status = false;
                            }

                            currentStudentPayment.Student = student;
                            currentStudentPayment.Person = paymentEtranzact.Payment.Payment.Person;
                            currentStudentPayment.Session = payment.Session;
                            currentStudentPayment.Id = payment.Id;

                            StudentPayment existingStudentPayment = studentPaymentLogic.GetModelBy(p => p.Payment_Id == payment.Id);

                            if (existingStudentPayment == null)
                            {
                                studentPaymentLogic.Create(currentStudentPayment);
                            }
                        }
                    }

                    viewModel.Student = student;
                }
                else
                {
                    AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == person.Id);
                    if (appliedCourse == null)
                    {
                        SetMessage("No Applied course record!", Message.Category.Error);
                        return View(viewModel);
                    }

                    StudentPayment studentPayment = studentPaymentLogic.GetModelBy(s => s.Person_Id == person.Id && s.Session_Id == payment.Session.Id && s.Payment_Id == payment.Id);

                    programme = appliedCourse.Programme;
                    department = appliedCourse.Department;
                    //level = studentPayment.Level;
                    if (studentPayment == null && appliedCourse.ApplicationForm.Setting.Session.Id == payment.Session.Id)
                    {
                        level = new Level() { Id = 1 };
                    }
                    else if (studentPayment != null)
                    {
                        level = studentPayment.Level;
                    }
                    else
                    {
                        SetMessage("Student Level not set, contact school ICS!", Message.Category.Error);
                        return View(viewModel);
                    }
                }

                payment.FeeDetails = paymentLogic.SetFeeDetails(payment, programme.Id, level.Id, payment.PaymentMode.Id, department.Id, payment.Session.Id);

                if (hostelAllocation != null)
                {
                    viewModel.HostelAllocation = hostelAllocation;
                    viewModel.Payment = payment;
                    viewModel.QRVerification = "http://portal.abiastateuniversity.edu.ng/Student/Hostel/HostelReceipt?spmid=" + payment.Id;
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
    }
}