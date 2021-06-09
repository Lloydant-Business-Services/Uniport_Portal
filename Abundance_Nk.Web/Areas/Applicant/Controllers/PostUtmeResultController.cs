using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Applicant.ViewModels;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Applicant.Controllers
{
    [AllowAnonymous]
    public class PostUtmeResultController :BaseController
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: /Student/PostUtmeResult/
        public ActionResult Index()
        {
            var viewModel = new PostUtmeResultViewModel();

            try
            {
                ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PostUtmeResultViewModel viewModel)
        {
            try
            {
                if(viewModel.JambRegistrationNumber != null && viewModel.PinNumber != null &&
                    viewModel.Programme != null)
                {
                    var payment = new ScratchCard();
                    var paymentScratchCardLogic = new ScratchCardLogic();

                    var result = new PutmeResult();
                    var PostUtmeResultLogic = new PutmeResultLogic();

                    var appForm = new ApplicationForm();
                    var appFormLogic = new ApplicationFormLogic();

                    var jambDetail = new ApplicantJambDetail();
                    var jambDetailLogic = new ApplicantJambDetailLogic();

                    appForm =
                        appFormLogic.GetModelsBy(
                            j =>
                                j.Application_Exam_Number == viewModel.JambRegistrationNumber &&
                                j.APPLICATION_PROGRAMME_FEE.PROGRAMME.Programme_Id == viewModel.Programme.Id)
                            .FirstOrDefault();
                    if(appForm != null)
                    {
                        payment =
                            paymentScratchCardLogic.GetModelsBy(p => p.Pin == viewModel.PinNumber).FirstOrDefault();
                        if(payment == null)
                        {
                            ReloadDropdown(viewModel);
                            SetMessage("Pin is invalid! Please check that you have typed in the correct detail",
                                Message.Category.Error);
                            return View(viewModel);
                        }
                        FeeType feetype;
                        if(viewModel.Programme.Id == 1)
                        {
                            feetype = new FeeType { Id = 7 };
                        }
                        else
                        {
                            feetype = new FeeType { Id = 8 };
                        }
                        if(paymentScratchCardLogic.ValidatePin(viewModel.PinNumber,feetype))
                        {
                            bool pinUseStatus = paymentScratchCardLogic.IsPinUsed(viewModel.PinNumber,appForm.Person.Id);
                            if(!pinUseStatus)
                            {
                                var prg = new Programme();
                                var prgLogic = new ProgrammeLogic();
                                prg = prgLogic.GetModelBy(p => p.Programme_Id == viewModel.Programme.Id);
                                result =
                                    PostUtmeResultLogic.GetModelsBy(
                                        m => m.EXAMNO == viewModel.JambRegistrationNumber && m.PROGRAMME == prg.Name)
                                        .FirstOrDefault();
                                if(result == null || result.Id <= 0)
                                {
                                    ReloadDropdown(viewModel);
                                    SetMessage(
                                        "Examination Number was not found! Please check that you have typed in the correct detail",
                                        Message.Category.Error);
                                    return View(viewModel);
                                }
                                //paymentEtranzactLogic.UpdatePin(payment.Payment.Payment, appForm.Person);
                                jambDetail = jambDetailLogic.GetModelBy(jb => jb.Application_Form_Id == appForm.Id);
                                viewModel.Result = result;
                                viewModel.jambDetail = jambDetail;
                                viewModel.ApplicationDetail = appForm;
                                paymentScratchCardLogic.UpdatePin(viewModel.PinNumber,appForm.Person);
                                TempData["PostUtmeResultViewModel"] = viewModel;
                                return RedirectToAction("PostUtmeResultSlip");
                            }
                            ReloadDropdown(viewModel);
                            SetMessage("Pin has been used by another applicant! Please cross check and Try again.",
                                Message.Category.Error);
                            return View(viewModel);
                        }
                        ReloadDropdown(viewModel);
                        SetMessage(
                            "Pin is not valid for the selected Programme/Purpose! Please check that you have typed in the correct detail",
                            Message.Category.Error);
                        return View(viewModel);
                    }

                    ReloadDropdown(viewModel);
                    SetMessage(
                        "The Examination Number was not found in the database. Please use the Examination Number given after the application process",
                        Message.Category.Error);
                    return View(viewModel);
                }
            }
            catch(Exception ex)
            {
                ReloadDropdown(viewModel);
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            ReloadDropdown(viewModel);
            return View(viewModel);
        }

        public void ReloadDropdown(PostUtmeResultViewModel viewModel)
        {
            try
            {
                if(viewModel.Programme == null)
                {
                    ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
                }
                else
                {
                    ViewBag.ProgrammeId = new SelectList(viewModel.ProgrammeSelectListItem,"VALUE","TEXT",
                        viewModel.Programme.Id);
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        //Load PUTME Slip view
        public ActionResult PostUtmeResultSlip(PostUtmeResultViewModel viewModel)
        {
            var existingViewModel = (PostUtmeResultViewModel)TempData["PostUtmeResultViewModel"];
            TempData["viewModel"] = existingViewModel;
            viewModel = existingViewModel;
            if(viewModel != null)
            {
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        public ActionResult MergeResult()
        {
            var viewModel = new PostUtmeResultViewModel();

            try
            {
                ViewBag.ProgrammeId = viewModel.ProgrammeSelectListItem;
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MergeResult(PostUtmeResultViewModel viewModel)
        {
            try
            {
                if(viewModel.Programme.Id != null && viewModel.Result.FullName != null)
                {
                    var programme = new Programme();
                    var prgLogic = new ProgrammeLogic();
                    programme = prgLogic.GetModelBy(p => p.Programme_Id == viewModel.Programme.Id);

                    var results = new List<PutmeResult>();
                    var PostUtmeResultLogic = new PutmeResultLogic();
                    results =
                        PostUtmeResultLogic.GetModelsBy(
                            a =>
                                a.EXAMNO == "" && a.FULLNAME.Contains(viewModel.Result.FullName) &&
                                a.PROGRAMME == programme.Name);
                    if(results != null)
                    {
                        PopulateResultDropdown(viewModel,results);
                        ViewBag.ResultId = viewModel.ResultSelectListItem;
                    }
                    else
                    {
                        SetMessage("No result found for " + viewModel.Result.FullName,Message.Category.Error);
                    }
                }
                ReloadDropdown(viewModel);
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdateResult(PostUtmeResultViewModel viewModel)
        {
            try
            {
                if(viewModel.Result.Id != null && viewModel.Result.ExamNo != null)
                {
                    var result = new PutmeResult();
                    var PostUtmeResultLogic = new PutmeResultLogic();
                    result = PostUtmeResultLogic.GetModelBy(r => r.ID == viewModel.Result.Id);
                    if(result != null)
                    {
                        result.ExamNo = viewModel.Result.ExamNo;
                        PostUtmeResultLogic.Modify(result);
                        SetMessage("Merged Successfully",Message.Category.Error);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }

            return RedirectToAction("MergeResult");
        }

        private static void PopulateResultDropdown(PostUtmeResultViewModel viewModel,List<PutmeResult> results)
        {
            viewModel.ResultSelectListItem = new List<SelectListItem>();
            var list = new SelectListItem();
            list.Value = "";
            list.Text = "SELECT NAME";
            viewModel.ResultSelectListItem.Add(list);

            foreach(PutmeResult result in results)
            {
                var selectList = new SelectListItem();
                selectList.Value = result.Id.ToString();
                selectList.Text = result.FullName;
                viewModel.ResultSelectListItem.Add(selectList);
            }
        }
    }
}