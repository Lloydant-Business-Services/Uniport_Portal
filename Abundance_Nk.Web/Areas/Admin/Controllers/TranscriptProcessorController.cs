using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class TranscriptProcessorController : BaseController
    {
        private TranscriptProcessorViewModel viewModel;

        // GET: Admin/TranscriptProcessor
        public ActionResult Index()
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                var requestLogic = new TranscriptRequestLogic();
                viewModel.transcriptRequests = requestLogic.GetProcessedTranscripts();
                PopulateDropDown(viewModel);
            }
            catch (Exception)
            {
                return View(viewModel);
            }
            return View(viewModel);
        }

        private void PopulateDropDown(TranscriptProcessorViewModel viewModel)
        {
            int i = 0;
            foreach (TranscriptRequest t in viewModel.transcriptRequests)
            {
                ViewData["status" + i] = new SelectList(viewModel.transcriptSelectList, Utility.VALUE, Utility.TEXT,
                    t.transcriptStatus.TranscriptStatusId);
                ViewData["Dispatchstatus" + i] = new SelectList(viewModel.StatusSelecList, Utility.VALUE, Utility.TEXT,
                    t.transcriptStatus.TranscriptStatusId);
                ViewData["clearanceStatus" + i] = new SelectList(viewModel.transcriptClearanceSelectList, Utility.VALUE,
                    Utility.TEXT, t.transcriptClearanceStatus.TranscriptClearanceStatusId);
                i++;

            }
        }

        public ActionResult UpdateStatus(long tid, long stat)
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                var transcriptRequestLogic = new TranscriptRequestLogic();
                TranscriptRequest tRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == tid);
                tRequest.transcriptStatus = new TranscriptStatus { TranscriptStatusId = (int)stat };
                transcriptRequestLogic.Modify(tRequest);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Clearance()
        {
            try
            {
                viewModel = new TranscriptProcessorViewModel();
                var requestLogic = new TranscriptRequestLogic();
                viewModel.transcriptRequests = requestLogic.GetAll();
                PopulateDropDown(viewModel);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        public ActionResult UpdateClearance(long tid, long stat)
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                var transcriptRequestLogic = new TranscriptRequestLogic();
                TranscriptRequest tRequest = transcriptRequestLogic.GetModelBy(t => t.Transcript_Request_Id == tid);
                tRequest.transcriptClearanceStatus = new TranscriptClearanceStatus
                {
                    TranscriptClearanceStatusId = (int)stat
                };
                transcriptRequestLogic.Modify(tRequest);
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }
            return RedirectToAction("Clearance");
        }

        public ActionResult ViewTranscriptDetails()
        {
            viewModel = new TranscriptProcessorViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewTranscriptDetails(TranscriptProcessorViewModel viewModel)
        {
            try
            {
                if (viewModel.transcriptRequest.student.MatricNumber != null)
                {
                    var personLogic = new PersonLogic();
                    var studentLogic = new StudentLogic();
                    var transcriptRequestLogic = new TranscriptRequestLogic();

                    Model.Model.Student student =
                        studentLogic.GetModelBy(s => s.Matric_Number == viewModel.transcriptRequest.student.MatricNumber);
                    if (student != null)
                    {
                        Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                        List<TranscriptRequest> transcriptRequests =
                            transcriptRequestLogic.GetModelsBy(tr => tr.Student_id == student.Id);
                        if (transcriptRequests == null)
                        {
                            SetMessage("The student has not made a transcript request", Message.Category.Error);
                        }
                        else
                        {
                            viewModel.RequestDateString =
                                transcriptRequests.LastOrDefault().DateRequested.ToShortDateString();
                            viewModel.transcriptRequests = transcriptRequests;
                            viewModel.Person = person;
                        }
                    }
                    else
                    {
                        SetMessage("Matric Number is not valid, or the student has not made a transcript request",
                            Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Enter Matric Number!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult EditTranscriptDetails(long Id)
        {
            try
            {
                viewModel = new TranscriptProcessorViewModel();
                if (Id > 0)
                {
                    var personLogic = new PersonLogic();
                    var studentLogic = new StudentLogic();
                    var transcriptRequestLogic = new TranscriptRequestLogic();
                    TranscriptRequest transcriptRequest =
                        transcriptRequestLogic.GetModelBy(a => a.Transcript_Request_Id == Id);
                    Model.Model.Student student = transcriptRequest.student;
                    if (student != null)
                    {
                        Person person = personLogic.GetModelBy(p => p.Person_Id == student.Id);
                        viewModel.RequestDateString = transcriptRequest.DateRequested.ToShortDateString();
                        viewModel.transcriptRequest = transcriptRequest;
                        viewModel.Person = person;
                    }
                    else
                    {
                        SetMessage("Matric Number is not valid, or the student has not made a transcript request",
                            Message.Category.Error);
                    }
                }
                else
                {
                    SetMessage("Enter Matric Number!", Message.Category.Error);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            RetainDropDownList(viewModel);
            return View(viewModel);
        }

        public ActionResult SaveTranscriptDetails(TranscriptProcessorViewModel viewModel)
        {
            try
            {
                if (viewModel.transcriptRequest != null)
                {
                    var personLogic = new PersonLogic();
                    var studentLogic = new StudentLogic();
                    var transcriptRequestLogic = new TranscriptRequestLogic();

                    var person = new Person();
                    var student = new Model.Model.Student();

                    person.Id = viewModel.transcriptRequest.student.Id;
                    person.LastName = viewModel.transcriptRequest.student.LastName;
                    person.FirstName = viewModel.transcriptRequest.student.FirstName;
                    person.OtherName = viewModel.transcriptRequest.student.OtherName;
                    bool isPersonModified = personLogic.Modify(person);

                    student.Id = viewModel.transcriptRequest.student.Id;
                    student.MatricNumber = viewModel.transcriptRequest.student.MatricNumber;
                    bool isStudentModified = studentLogic.Modify(student);

                    if (viewModel.transcriptRequest.DestinationCountry.Id == "OTH")
                    {
                        viewModel.transcriptRequest.DestinationState.Id = "OT";
                    }
                    bool isTranscriptRequestModified = transcriptRequestLogic.Modify(viewModel.transcriptRequest);

                    if (isTranscriptRequestModified && isStudentModified)
                    {
                        SetMessage("Operation Successful!", Message.Category.Information);
                        return RedirectToAction("ViewTranscriptDetails");
                    }
                    if (isTranscriptRequestModified && !isStudentModified)
                    {
                        SetMessage("Not all fields were modified!", Message.Category.Information);
                        return RedirectToAction("ViewTranscriptDetails");
                    }
                    if (!isTranscriptRequestModified && isStudentModified)
                    {
                        SetMessage("Not all fields were modified!", Message.Category.Information);
                        return RedirectToAction("ViewTranscriptDetails");
                    }
                    if (!isTranscriptRequestModified && !isStudentModified)
                    {
                        SetMessage("No item modified!", Message.Category.Information);
                        return RedirectToAction("ViewTranscriptDetails");
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("EditTranscriptDetails");
        }

        public ActionResult DeleteTranscriptDetails(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    var transcriptRequestLogic = new TranscriptRequestLogic();
                    var onlinePaymentLogic = new OnlinePaymentLogic();
                    var paymentLogic = new PaymentLogic();

                    TranscriptRequest transcriptRequest =
                        transcriptRequestLogic.GetModelBy(tr => tr.Transcript_Request_Id == Id);
                    TranscriptRequest transcriptRequestAlt = transcriptRequest;
                    if (transcriptRequest != null)
                    {
                        using (var scope = new TransactionScope())
                        {
                            transcriptRequestLogic.Delete(tr => tr.Transcript_Request_Id == transcriptRequest.Id);

                            //if (transcriptRequest.payment != null)
                            //{
                            //    OnlinePayment onlinePayment = onlinePaymentLogic.GetModelBy(op => op.Payment_Id == transcriptRequestAlt.payment.Id);
                            //    if (onlinePayment != null)
                            //    {
                            //        onlinePaymentLogic.Delete(op => op.Payment_Id == transcriptRequestAlt.payment.Id);
                            //    }

                            //    paymentLogic.Delete(p => p.Payment_Id == transcriptRequestAlt.payment.Id);
                            //}

                            SetMessage("Operation Successful!", Message.Category.Information);
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return RedirectToAction("ViewTranscriptDetails");
        }

        private void RetainDropDownList(TranscriptProcessorViewModel viewModel)
        {
            try
            {
                if (viewModel.transcriptRequest != null)
                {
                    if (viewModel.transcriptRequest.DestinationCountry != null)
                    {
                        ViewBag.Country = new SelectList(viewModel.CountrySelectList, "Value", "Text",
                            viewModel.transcriptRequest.DestinationCountry.Id);
                    }
                    else
                    {
                        ViewBag.Country = viewModel.CountrySelectList;
                    }

                    if (viewModel.transcriptRequest.DestinationCountry != null)
                    {
                        ViewBag.State = new SelectList(viewModel.StateSelectList, "Value", "Text",
                            viewModel.transcriptRequest.DestinationState.Id);
                    }
                    else
                    {
                        ViewBag.State = viewModel.StateSelectList;
                    }

                    if (viewModel.transcriptRequest.transcriptClearanceStatus != null)
                    {
                        ViewBag.TranscriptClearanceStatus = new SelectList(viewModel.transcriptClearanceSelectList,
                            "Value", "Text",
                            viewModel.transcriptRequest.transcriptClearanceStatus.TranscriptClearanceStatusId);
                    }
                    else
                    {
                        ViewBag.TranscriptClearanceStatus = viewModel.transcriptClearanceSelectList;
                    }

                    if (viewModel.transcriptRequest.transcriptStatus != null)
                    {
                        ViewBag.TranscriptStatus = new SelectList(viewModel.transcriptSelectList, "Value", "Text",
                            viewModel.transcriptRequest.transcriptStatus.TranscriptStatusId);
                    }
                    else
                    {
                        ViewBag.TranscriptStatus = viewModel.transcriptSelectList;
                    }
                }
                else
                {
                    ViewBag.Country = viewModel.CountrySelectList;
                    ViewBag.State = viewModel.StateSelectList;
                    ViewBag.TranscriptClearanceStatus = viewModel.transcriptClearanceSelectList;
                    ViewBag.TranscriptStatus = viewModel.transcriptSelectList;
                }
            }
            catch (Exception)
            {
                throw;
            }
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
                    List<StateGeoZone> stateGeoZones = stateGeoZoneLogic.GetModelsBy(s => s.State_Id == stateId && s.Activated);

                    for (int i = 0; i < stateGeoZones.Count; i++)
                    {
                        StateGeoZone stateGeoZone = stateGeoZones[i];
                        if (stateGeoZones[i].State.Id == "OT")
                        {
                            List<DeliveryServiceZone> currentDeliveryServiceZones = deliveryServiceZoneLogic.GetModelsBy(s => s.Country_Id == countryId && s.Geo_Zone_Id == stateGeoZone.GeoZone.Id && s.Delivery_Service_Id == deliveryServiceId && s.Activated);

                            for (int j = 0; j < currentDeliveryServiceZones.Count; j++)
                            {
                                int currentFeeTypeId = currentDeliveryServiceZones[j].FeeType.Id;
                                FeeDetailLogic feeDetailLogic = new FeeDetailLogic();
                                FeeDetail feeDetail = feeDetailLogic.GetModelsBy(s => s.Fee_Type_Id == currentFeeTypeId && !s.FEE.Fee_Name.Contains("Transcript")).FirstOrDefault();
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
                            List<DeliveryServiceZone> currentDeliveryServiceZones = deliveryServiceZoneLogic.GetModelsBy(s => s.Country_Id == countryId && s.Geo_Zone_Id == stateGeoZone.GeoZone.Id && s.Delivery_Service_Id == deliveryServiceId && s.Activated);

                            for (int j = 0; j < currentDeliveryServiceZones.Count; j++)
                            {

                                currentDeliveryServiceZones[j].Name = stateGeoZone.State.Name + " - " + currentDeliveryServiceZones[j].FeeType;

                                DeliveryServiceZone serviceZone = currentDeliveryServiceZones[j];

                                if (deliveryServiceZones.Count(s => s.Id == serviceZone.Id) <= 0)
                                {
                                    deliveryServiceZones.Add(currentDeliveryServiceZones[j]);
                                }

                            }
                        }

                    }
                }

                return Json(new SelectList(deliveryServiceZones, "Id", "Name"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ViewDeliveryService()
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                DeliveryServiceLogic deliveryServiceLogic = new DeliveryServiceLogic();
                viewModel.DeliveryServices = deliveryServiceLogic.GetAll();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult CreateDeliveryService()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDeliveryService(TranscriptProcessorViewModel model)
        {
            try
            {

                if (model.DeliveryService != null)
                {
                    DeliveryServiceLogic deliveryServiceLogic = new DeliveryServiceLogic();
                    var exsitingDeliveryService = deliveryServiceLogic.GetModelBy(s => s.Name.Contains(model.DeliveryService.Name));
                    if (exsitingDeliveryService == null)
                    {
                        var createdDeliveryService = deliveryServiceLogic.Create(model.DeliveryService);
                        if (createdDeliveryService != null)
                        {
                            SetMessage("Operation Successful", Message.Category.Information);
                            ModelState.Clear();
                        }
                        else
                        {
                            SetMessage("Operation Failed", Message.Category.Error);
                        }
                    }
                    else
                    {
                        SetMessage("Operation Failed! Courier Destination Already Exists ", Message.Category.Error);
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditDeliveryService(int? Id)
        {
            DeliveryService deliveryService;
            try
            {
                if (Id <= 0)
                {
                    throw new ArgumentException("Id was not Found");
                }

                int deliveryServiceId = Convert.ToInt32(Id);
                DeliveryServiceLogic deliveryServiceLogic = new DeliveryServiceLogic();
                deliveryService = deliveryServiceLogic.GetModelBy(s => s.Delivery_Service_Id == deliveryServiceId);

            }
            catch (Exception)
            {

                throw;
            }
            return View(deliveryService);
        }
        [HttpPost]
        public ActionResult EditDeliveryService(DeliveryService model)
        {
            try
            {
                if (model != null && model.Id > 0)
                {
                    DeliveryServiceLogic deliveryServiceLogic = new DeliveryServiceLogic();
                    var modifiedDeliveryService = deliveryServiceLogic.Modify(model);
                    if (modifiedDeliveryService)
                    {
                        SetMessage("Operation Successful", Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("Operation Failed! Kindly Try again", Message.Category.Error);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("ViewDeliveryService", "TranscriptProcessor", new { area = "Admin" });
        }
        public ActionResult ViewDeliveryServiceZone()
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                DeliveryServiceZoneLogic deliveryServiceZoneLogic = new DeliveryServiceZoneLogic();
                viewModel.DeliveryServiceZones = deliveryServiceZoneLogic.GetAll();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult CreateDeliveryServiceZone()
        {
            try
            {
                viewModel = new TranscriptProcessorViewModel();
                ViewBag.GeoZoneId = viewModel.GeoZoneSelectListItems;
                ViewBag.DeliveryServiceId = viewModel.DeliverySerivceSelectListItems;
                ViewBag.FeeId = viewModel.FeeTypeSelectListItems;
                ViewBag.CountryId = viewModel.CountrySelectList;
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateDeliveryServiceZone(TranscriptProcessorViewModel model)
        {
            try
            {

                if (model.DeliveryServiceZone != null)
                {
                    DeliveryServiceZoneLogic deliveryServiceZoneLogic = new DeliveryServiceZoneLogic();
                    var exsitingDeliveryServiceZone = deliveryServiceZoneLogic.GetModelBy(s => s.Delivery_Service_Id == model.DeliveryServiceZone.DeliveryService.Id && s.Fee_Type_Id == model.DeliveryServiceZone.FeeType.Id
                      && s.Geo_Zone_Id == model.DeliveryServiceZone.GeoZone.Id && s.Delivery_Service_Amount == model.DeliveryServiceZone.DeliveryServiceAmount 
                      && s.LLoydant_Amount == model.DeliveryServiceZone.LLoydantAmount && s.School_Amount == model.DeliveryServiceZone.SchoolAmount);
                    if (exsitingDeliveryServiceZone == null)
                    {
                        var deliveryServiceZone = deliveryServiceZoneLogic.Create(model.DeliveryServiceZone);
                        if (deliveryServiceZone != null)
                        {
                            SetMessage("Operation Successful", Message.Category.Information);
                            ModelState.Clear();
                        }
                        else
                        {
                            SetMessage("Operation Failed", Message.Category.Error);
                        }
                    }
                    else
                    {
                        SetMessage("Operation Failed! Courier Destination Already Exists ", Message.Category.Error);
                    }

                }
                PopulateDropDowns();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }
        [HttpGet]
        public ActionResult EditDeliveryServiceZone(int? Id)
        {
            DeliveryServiceZone deliveryServiceZone;
            try
            {
                if (Id <= 0)
                {
                    throw new ArgumentException("Id was not Found");
                }

                int deliveryServiceZoneId = Convert.ToInt32(Id);
                DeliveryServiceZoneLogic deliveryServiceZoneLogic = new DeliveryServiceZoneLogic();
                deliveryServiceZone = deliveryServiceZoneLogic.GetModelBy(s => s.Delivery_Service_Zone_Id == deliveryServiceZoneId);
                viewModel = new TranscriptProcessorViewModel();
                ViewBag.GeoZoneId = new SelectList(viewModel.GeoZoneSelectListItems, Utility.VALUE, Utility.TEXT, deliveryServiceZone.GeoZone.Id);
                ViewBag.DeliveryServiceId = new SelectList(viewModel.DeliverySerivceSelectListItems, Utility.VALUE, Utility.TEXT, deliveryServiceZone.DeliveryService.Id);
                ViewBag.FeeId = new SelectList(viewModel.FeeTypeSelectListItems, Utility.VALUE, Utility.TEXT, deliveryServiceZone.FeeType.Id);
                ViewBag.CountryId = new SelectList(viewModel.CountrySelectList, Utility.VALUE, Utility.TEXT, deliveryServiceZone.Country.Id);

            }
            catch (Exception ex)
            {

                throw;
            }
            return View(deliveryServiceZone);
        }
        [HttpPost]
        public ActionResult EditDeliveryServiceZone(DeliveryServiceZone model)
        {
            try
            {
                if (model != null && model.Id > 0)
                {
                    DeliveryServiceZoneLogic deliveryServiceLogic = new DeliveryServiceZoneLogic();
                    var modifiedDeliveryZone = deliveryServiceLogic.Modify(model);
                    if (modifiedDeliveryZone)
                    {
                        SetMessage("Operation Successful", Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("Operation Failed! Kindly Try again", Message.Category.Error);
                    }
                }
                PopulateDropDowns();
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("ViewDeliveryServiceZone", "TranscriptProcessor", new { area = "Admin" });
        }
        public ActionResult ViewGeoZone()
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                GeoZoneLogic geoZoneLogic = new GeoZoneLogic();
                viewModel.GeoZones = geoZoneLogic.GetAll();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult CreateGeoZone()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateGeoZone(TranscriptProcessorViewModel model)
        {
            try
            {

                if (model.GeoZone != null)
                {
                    GeoZoneLogic geoZoneLogic = new GeoZoneLogic();
                    var exsitingCourierService = geoZoneLogic.GetModelBy(s => s.Name.Contains(model.GeoZone.Name));
                    if (exsitingCourierService == null)
                    {
                        var createdGeoZone = geoZoneLogic.Create(model.GeoZone);
                        if (createdGeoZone != null)
                        {
                            SetMessage("Operation Successful", Message.Category.Information);
                            ModelState.Clear();
                        }
                        else
                        {
                            SetMessage("Operation Failed", Message.Category.Error);
                        }
                    }
                    else
                    {
                        SetMessage("Operation Failed! Courier Destination Already Exists ", Message.Category.Error);
                    }

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }

        public ActionResult EditGeoZone(int? Id)
        {
            GeoZone geoZone;
            try
            {
                if (Id <= 0)
                {
                    throw new ArgumentException("Id was not Found");
                }

                int geoZoneId = Convert.ToInt32(Id);
                GeoZoneLogic geoZoneLogic = new GeoZoneLogic();
                geoZone = geoZoneLogic.GetModelBy(s => s.Geo_Zone_Id == geoZoneId);

            }
            catch (Exception)
            {

                throw;
            }
            return View(geoZone);
        }
        [HttpPost]
        public ActionResult EditGeoZone(GeoZone model)
        {
            try
            {
                if (model != null && model.Id > 0)
                {
                    GeoZoneLogic geoZoneLogic = new GeoZoneLogic();
                    var modifiedGeoZone = geoZoneLogic.Modify(model);
                    if (modifiedGeoZone)
                    {
                        SetMessage("Operation Successful", Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("Operation Failed! Kindly Try again", Message.Category.Error);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("ViewGeoZone", "TranscriptProcessor", new { area = "Admin" });
        }
        public ActionResult ViewStateGeoZone()
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                StateGeoZoneLogic stateGeoZoneLogic = new StateGeoZoneLogic();
                viewModel.StateGeoZones = stateGeoZoneLogic.GetAll();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(viewModel);
        }
        [HttpGet]
        public ActionResult CreateStateGeoZone()
        {
            try
            {
                viewModel = new TranscriptProcessorViewModel();
                ViewBag.GeoZoneId = viewModel.GeoZoneSelectListItems;
                ViewBag.StateId = viewModel.StateSelectList;
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateStateGeoZone(TranscriptProcessorViewModel model)
        {
            try
            {

                if (model.StateGeoZone != null)
                {
                    StateGeoZoneLogic stateGeoZoneLogic = new StateGeoZoneLogic();
                    var exsitingCourierService = stateGeoZoneLogic.GetModelBy(s => s.Geo_Zone_Id == model.StateGeoZone.GeoZone.Id && s.State_Id == model.StateGeoZone.State.Id);
                    if (exsitingCourierService == null)
                    {
                        var createdStateGeoZone = stateGeoZoneLogic.Create(model.StateGeoZone);
                        if (createdStateGeoZone != null)
                        {
                            SetMessage("Operation Successful", Message.Category.Information);
                            ModelState.Clear();
                        }
                        else
                        {
                            SetMessage("Operation Failed", Message.Category.Error);
                        }
                    }
                    else
                    {
                        SetMessage("Operation Failed! Courier Destination Already Exists ", Message.Category.Error);
                    }

                }
                PopulateDropDowns();
            }
            catch (Exception ex)
            {

                throw;
            }
            return View();
        }
        public ActionResult EditStateGeoZone(int? Id)
        {
            StateGeoZone stateGeoZone;
            try
            {
                if (Id <= 0 || Id == null)
                {
                    throw new ArgumentException("Id was not Found");
                }

                int stateGeozoneId = Convert.ToInt32(Id);
                StateGeoZoneLogic stateGeoZoneLogic = new StateGeoZoneLogic();
                stateGeoZone = stateGeoZoneLogic.GetModelBy(s => s.State_Geo_Zone_Id == stateGeozoneId);
                viewModel = new TranscriptProcessorViewModel();
                ViewBag.GeoZoneId = new SelectList(viewModel.GeoZoneSelectListItems, Utility.VALUE, Utility.TEXT, stateGeoZone.GeoZone.Id);
                ViewBag.StateId = new SelectList(viewModel.StateSelectList, Utility.VALUE, Utility.TEXT, stateGeoZone.State.Id);
            }
            catch (Exception)
            {

                throw;
            }
            return View(stateGeoZone);
        }
        [HttpPost]
        public ActionResult EditStateGeoZone(StateGeoZone model)
        {
            try
            {
                if (model != null && model.Id > 0)
                {
                    StateGeoZoneLogic stateGeoZoneLogic = new StateGeoZoneLogic();
                    var modifiedStateGeoZone = stateGeoZoneLogic.Modify(model);
                    if (modifiedStateGeoZone)
                    {
                        SetMessage("Operation Successful", Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("Operation Failed! Kindly Try again", Message.Category.Error);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("ViewStateGeoZone", "TranscriptProcessor", new { area = "Admin" });
        }

        private void PopulateDropDowns()
        {
            try
            {
                ViewBag.GeoZoneId = Utility.PopulateGeoZoneSelectListItem();
                ViewBag.DeliveryServiceId = Utility.PopulateDeliveryServiceSelectListItem();
                ViewBag.FeeId = Utility.PopulateFeeSelectListItem();
                ViewBag.StateId = Utility.PopulateStateSelectListItem();
                ViewBag.CountryId = Utility.PopulateCountrySelectListItem();
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public ActionResult DispatchedTranscript()
        {
            viewModel = new TranscriptProcessorViewModel();
            try
            {
                var requestLogic = new TranscriptRequestLogic();
                viewModel.transcriptRequests = requestLogic.GetDispatchedTranscripts();
                PopulateDropDown(viewModel);
            }
            catch (Exception)
            {
                return View(viewModel);
            }
            return View(viewModel);
        }
        //public ActionResult ViewTranscriptRequest()
        //{
        //    //PopulateDropDowns();
        //    TranscriptRequestViewModel viewModel = new TranscriptRequestViewModel();
        //    viewModel.GroupTranscriptByYears = new List<GroupTranscriptByYear>();
        //    viewModel.GroupTranscriptByMonths = new List<GroupTranscriptByMonth>();
        //    viewModel.TranscriptRequests = new List<TranscriptRequest>();
        //    ViewBag.Transcript = viewModel.TranscriptStatusSelectItem;
        //    return View(viewModel);
        //}
        [Authorize(Roles = "lloydant,Admin,School Admin,Transcript Officer")]
        public ActionResult ViewTranscriptRequest()
        {
            //PopulateDropDowns();
            TranscriptRequestViewModel viewModel = new TranscriptRequestViewModel();
            viewModel.GroupTranscriptByYears = new List<GroupTranscriptByYear>();
            viewModel.GroupTranscriptByMonths = new List<GroupTranscriptByMonth>();
            ViewBag.Transcript = viewModel.TranscriptStatusSelectItem;
            List<TranscriptRequest> transcriptRequestList = new List<TranscriptRequest>();


            try
            {

                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                var allTranscriptsWithPayment = transcriptRequestLogic.GetModelsBy(tr => tr.Payment_Id != null);
                 transcriptRequestList = GetPaidTrancriptRequest(allTranscriptsWithPayment);
                if (transcriptRequestList.Count > 0)
                {
                    var groupedByYearRequests = transcriptRequestList.GroupBy(x => x.DateRequested.Year).ToList();
                    if (groupedByYearRequests.Count > 0)
                    {

                        foreach (var groupedByYearRequest in groupedByYearRequests)
                        {
                            int count = 0;
                            int intYear = 0;
                            var year = groupedByYearRequest.Key;
                            intYear = Convert.ToInt32(year);
                            foreach (var transcriptRequest in transcriptRequestList)
                            {
                                int requestYear = Convert.ToInt32(transcriptRequest.DateRequested.Year);
                                if (intYear == requestYear)
                                {
                                    count += 1;
                                }
                            }
                            GroupTranscriptByYear groupTranscript = new GroupTranscriptByYear();
                            groupTranscript.Year = intYear;
                            groupTranscript.TranscriptCount = count;
                            viewModel.GroupTranscriptByYears.Add(groupTranscript);
                        }

                    }
                }
                viewModel.TranscriptRequests = new List<TranscriptRequest>();
                ViewBag.Department = new SelectList(viewModel.DepartmentSelectListItem, Utility.VALUE, Utility.TEXT);
                TempData["PaidTranscript"] = transcriptRequestList;
                TempData.Keep("PaidTranscript");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View(viewModel);
        }

        public ActionResult TranscriptCountByMonth(int year)
        {
            
            TranscriptRequestViewModel viewModel = new TranscriptRequestViewModel();
            viewModel.GroupTranscriptByMonths = new List<GroupTranscriptByMonth>();
            viewModel.GroupTranscriptByYears = new List<GroupTranscriptByYear>();
            viewModel.TranscriptRequests = new List<TranscriptRequest>();
            List<TranscriptRequest> ByselectedYear = new List<TranscriptRequest>();
            ViewBag.Transcript = viewModel.TranscriptStatusSelectItem;
            try
            {
                TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                var allPaidTranscriptRequests = (List<TranscriptRequest>)TempData["PaidTranscript"];
                TempData.Keep("PaidTranscript");
                //var allUnprocessedTranscripts = transcriptRequestLogic.GetModelsBy(tr => tr.Transcript_Status_Id == 3 && tr.Date_Requested.Year == year);
                if (allPaidTranscriptRequests.Count > 0)
                {
                    for(int i=0; i < allPaidTranscriptRequests.Count; i++)
                    {
                        var seletedYear=Convert.ToInt32(allPaidTranscriptRequests[i].DateRequested.Year);
                        if (seletedYear == year)
                        {
                            ByselectedYear.Add(allPaidTranscriptRequests[i]);
                        }
                    }
                    var groupedByMonthRequests = ByselectedYear.GroupBy(x => x.DateRequested.Month).ToList();
                    if (groupedByMonthRequests.Count > 0)
                    {
                        foreach (var groupedByMonthRequest in groupedByMonthRequests)
                        {
                            int count = 0;
                            var stringMonth = MonthName(groupedByMonthRequest.Key);

                            foreach (var allPaidTranscriptRequest in ByselectedYear)
                            {
                                int requestMonth = Convert.ToInt32(allPaidTranscriptRequest.DateRequested.Month);
                                if (groupedByMonthRequest.Key == requestMonth)
                                {
                                    count += 1;
                                }
                            }
                            GroupTranscriptByMonth groupTranscriptByMonth = new GroupTranscriptByMonth();
                            groupTranscriptByMonth.Month = stringMonth;
                            groupTranscriptByMonth.TranscriptCount = count;
                            groupTranscriptByMonth.Year = year;
                            groupTranscriptByMonth.intMonth = groupedByMonthRequest.Key;
                            viewModel.GroupTranscriptByMonths.Add(groupTranscriptByMonth);
                        }

                    }

                }
                return View("ViewTranscriptRequest", viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult TranscriptRequestByMonth(int month, int year)
        {
            
            TranscriptRequestViewModel viewModel = new TranscriptRequestViewModel();
            viewModel.GroupTranscriptByMonths = new List<GroupTranscriptByMonth>();
            viewModel.GroupTranscriptByYears = new List<GroupTranscriptByYear>();
            viewModel.TranscriptRequests = new List<TranscriptRequest>();
            ViewBag.Transcript = viewModel.TranscriptStatusSelectItem;
            try
            {
                //TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                var allUnprocessedTranscripts = (List<TranscriptRequest>)TempData["PaidTranscript"];
                TempData.Keep("PaidTranscript");
                 var allYearandMonth=allUnprocessedTranscripts.Where(x => x.DateRequested.Year == year && x.DateRequested.Month == month).ToList();
                //var allUnprocessedTranscripts = transcriptRequestLogic.GetModelsBy(tr => tr.Transcript_Status_Id == 3 && tr.Date_Requested.Year == year && tr.Date_Requested.Month == month);
                if (allUnprocessedTranscripts.Count > 0)
                {
                    //viewModel.TranscriptRequests = allUnprocessedTranscripts;
                    viewModel.TranscriptRequests = allYearandMonth;
                }
                return View("ViewTranscriptRequest", viewModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult DispatchTranscript(long trId)
        {
            try
            {
                if (trId > 0)
                {
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                    var request=transcriptRequestLogic.GetModelBy(x => x.Transcript_Request_Id == trId);
                    if (request != null)
                    {
                        request.transcriptStatus = new TranscriptStatus { TranscriptStatusId = 5 };
                        transcriptRequestLogic.Modify(request);
                    }

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("ViewTranscriptRequest");
        }

        public List<TranscriptRequest> GetPaidTrancriptRequest(List<TranscriptRequest> transcriptRequests)
        {
            List<TranscriptRequest> transcriptRequestList = new List<TranscriptRequest>();
            try
            {
                
                PaymentEtranzactLogic paymentEtranzactLogic = new PaymentEtranzactLogic();
                PaymentInterswitchLogic paymentInterswitchLogic = new PaymentInterswitchLogic();
                //Get all paid transcript request
                if (transcriptRequests != null && transcriptRequests.Count > 0)
                {
                    for (int i = 0; i < transcriptRequests.Count; i++)
                    {
                        var paymentId = transcriptRequests[i].payment.Id;
                        var etranzactpayment = paymentEtranzactLogic.GetModelsBy(x => x.Payment_Id == paymentId).FirstOrDefault();
                        if (etranzactpayment != null)
                        {
                            transcriptRequestList.Add(transcriptRequests[i]);
                        }
                        else
                        {
                            var interswitchPayment = paymentInterswitchLogic.GetModelsBy(x => x.Payment_Id == paymentId).FirstOrDefault();
                            if (interswitchPayment != null && interswitchPayment.ResponseCode == "00")
                            {
                                transcriptRequestList.Add(transcriptRequests[i]);
                            }
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return transcriptRequestList;
        }
        public string MonthName(int month)
        {
            string myMonth = null;
            try
            {

                switch (month)
                {
                    case 1:
                        myMonth = "JANUARY";
                        break;
                    case 2:
                        myMonth = "FEBRUARY";
                        break;
                    case 3:
                        myMonth = "MARCH";
                        break;
                    case 4:
                        myMonth = "APRIL";
                        break;
                    case 5:
                        myMonth = "MAY";
                        break;
                    case 6:
                        myMonth = "JUNE";
                        break;
                    case 7:
                        myMonth = "JULY";
                        break;
                    case 8:
                        myMonth = "AUGUST";
                        break;
                    case 9:
                        myMonth = "SEPTEMBER";
                        break;
                    case 10:
                        myMonth = "OCTOBER";
                        break;
                    case 11:
                        myMonth = "NOVEMBER";
                        break;
                    case 12:
                        myMonth = "DECEMBER";
                        break;

                }
                return myMonth;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ActionResult TranscriptIncidentLog()
        {
            try
            {

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View();
        }
        [HttpPost]
        public ActionResult TranscriptIncidentLog(TranscriptRequestViewModel transcriptRequestViewModel)
        {
            try
            {
                if(transcriptRequestViewModel.MatricNo != null)
                {
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                    StudentLevelLogic studentLevelLogic = new StudentLevelLogic();
                    var studentTranscriptRequest = transcriptRequestLogic.GetModelsBy(tr => tr.STUDENT.Matric_Number==transcriptRequestViewModel.MatricNo);
                    if (studentTranscriptRequest.Count > 0)
                    {
                        var studentId = studentTranscriptRequest.FirstOrDefault().student.Id;
                        transcriptRequestViewModel.TranscriptRequests=GetPaidTrancriptRequest(studentTranscriptRequest);
                        transcriptRequestViewModel.StudentLevels=studentLevelLogic.GetModelsBy(f => f.Person_Id == studentId);

                    }
                    else
                    {
                        SetMessage("No Paid Transcript Request For This Matric No." + "  " + transcriptRequestViewModel.MatricNo, Message.Category.Information);
                        return View(transcriptRequestViewModel);
                    }
                }
                else
                {
                    SetMessage("Please, Enter Matric No. To Continue", Message.Category.Information);
                    return View(transcriptRequestViewModel);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(transcriptRequestViewModel);
        }
        public JsonResult LogTranscriptIncidentRequest(string email, string phone, long requestId, string note, int departmentId)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if(email !=null && requestId > 0)
                {
                    TranscriptRequestLogic transcriptRequestLogic = new TranscriptRequestLogic();
                    TranscriptIncidentLog transcriptIncidentLog = new TranscriptIncidentLog();
                    TranscriptIncidentLogLogic transcriptIncidentLogLogic = new TranscriptIncidentLogLogic();
                    UserLogic userLogic = new UserLogic();
                   var user= userLogic.GetModelsBy(f => f.User_Name == User.Identity.Name).FirstOrDefault();
                    var requestExist=transcriptRequestLogic.GetModelBy(f => f.Transcript_Request_Id == requestId);
                    var openedLog=transcriptIncidentLogLogic.GetModelsBy(u => u.Transcript_Request_Id == requestId && u.Status);
                    if (openedLog.Count > 0)
                    {
                        result.IsError = true;
                        result.Message = "There is Already an Open Log For this Request";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    var ticket=transcriptIncidentLogLogic.ValidTicket();
                    if (requestExist != null)
                    {
                        transcriptIncidentLog.Date_Opened = DateTime.Now;
                   
                        transcriptIncidentLog.Email = email;
                        transcriptIncidentLog.LoggedUser = user;
                        transcriptIncidentLog.Phone_No = phone;
                        transcriptIncidentLog.Status = true;
                        transcriptIncidentLog.TranscriptRequest = new TranscriptRequest { Id = requestId };
                        transcriptIncidentLog.TicketId = ticket;
                        transcriptIncidentLog.Department = new Department { Id = departmentId };
                        var createdLog=transcriptIncidentLogLogic.Create(transcriptIncidentLog);
                        if (createdLog != null)
                        {
                            var request=transcriptRequestLogic.GetModelBy(i => i.Transcript_Request_Id == requestId);
                            SendMail(email, ticket,request.student.FullName, transcriptIncidentLog.Status);
                            result.IsError = false;
                            result.Message = "Ticket Opened Successfully";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        result.IsError = true;
                        result.Message = "This Transcript Request Was not Found";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ViewTranscriptIncidentLog()
        {
            TranscriptRequestViewModel transcriptRequestViewModel = new TranscriptRequestViewModel();
            try
            {

            }
            catch(Exception ex)
            {
                throw ex;
            }
            return View(transcriptRequestViewModel);
        }
        [HttpPost]
        public ActionResult ViewTranscriptIncidentLog(TranscriptRequestViewModel transcriptRequestViewModel)
        {
            try
            {
                DateTime toDate = transcriptRequestViewModel.To.AddDays(1).AddSeconds(-1);
                TranscriptIncidentLogLogic transcriptIncidentLogLogic = new TranscriptIncidentLogLogic();
                if (transcriptRequestViewModel.From > toDate)
                {
                    SetMessage("Date Range Selected is invalid.", Message.Category.Information);
                    return View(transcriptRequestViewModel);
                }
                if (transcriptRequestViewModel.Active)
                {

                    transcriptRequestViewModel.TranscriptIncidentLogs = transcriptIncidentLogLogic.GetModelsBy(d => d.Status==false && d.Date_Opened >= transcriptRequestViewModel.From && d.Date_Opened <= toDate);
                    transcriptRequestViewModel.ShowClosedTicket = true;
                }
                else
                {
                    transcriptRequestViewModel.TranscriptIncidentLogs = transcriptIncidentLogLogic.GetModelsBy(d => d.Status && d.Date_Opened >= transcriptRequestViewModel.From && d.Date_Opened <= toDate);
                    
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(transcriptRequestViewModel);
        }
        public JsonResult CloseLog( List<string> logIds)
        {
            JsonResultModel result = new JsonResultModel();
            try
            {
                if (logIds.Count > 0)
                {
                    
                    TranscriptIncidentLogLogic transcriptIncidentLogLogic = new TranscriptIncidentLogLogic();
                    List<TranscriptIncidentLog> transcriptIncidentLoglist = new List<TranscriptIncidentLog>();
                    UserLogic userLogic = new UserLogic();
                    var user = userLogic.GetModelsBy(f => f.User_Name == User.Identity.Name).FirstOrDefault();
                    for(int i=0; i < logIds.Count; i++)
                    {
                        string appNoWithId = logIds[i].Replace("_", "/");
                        string[] mySplit = appNoWithId.Split('*');
                        long logId = Convert.ToInt64(mySplit[1]);
                        var Log = transcriptIncidentLogLogic.GetModelBy(u => u.Id == logId);
                        if (Log != null)
                        {
                            Log.Status = false;
                            Log.Date_Closed = DateTime.Now;
                            Log.ClosedUser = user;
                            transcriptIncidentLoglist.Add(Log);

                        }
                    }
                    var successful=transcriptIncidentLogLogic.ModifyList(transcriptIncidentLoglist);

                    if (successful)
                    {
                        for(int i = 0; i < transcriptIncidentLoglist.Count; i++)
                        {
                            SendMail(transcriptIncidentLoglist[i].Email, transcriptIncidentLoglist[i].TicketId, transcriptIncidentLoglist[i].TranscriptRequest.student.Name, false);
                        }
                        result.IsError = false;
                        result.Message = "Ticket Closed Successfully";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result.IsError = false;
                        result.Message = "Ticket Close Failed";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public void SendMail( string email, string ticket, string name, bool status)
        {
            try
            {
                Receipt receipt = new Receipt();
                //receipt.Description = note;
                receipt.Status =status==true? "OPENED":"CLOSED";
                receipt.TicketId = ticket;
                receipt.Name = name;
                EmailMessage message = new EmailMessage();
                //message.Name = paymentInterSwitch.Payment.Person.FullName; 
                message.Email = email ?? "support@lloydant.com";
                message.Subject = ticket;
               // message.From = "ABSU, Donotreply";
                var IncidentTemplate = Server.MapPath("/Areas/Common/Views/Credential/IncidentTemplate.cshtml");
                EmailSenderLogic<Receipt> receiptEmailSenderLogic = new EmailSenderLogic<Receipt>(IncidentTemplate, receipt);

                receiptEmailSenderLogic.Send(message);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        

    }
}