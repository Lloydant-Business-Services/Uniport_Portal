using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using Abundance_Nk.Web.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class AdmissionProcessingController :BaseController
    {
        private readonly AdmissionProcessingViewModel viewModel;

        public AdmissionProcessingController()
        {
            viewModel = new AdmissionProcessingViewModel();
        }

        public ActionResult Index()
        {
            ViewBag.SessionId = viewModel.SessionSelectList;

            return View(viewModel);
        }

        public ActionResult ViewDetails(int? mid)
        {
            return View();
        }

        public ActionResult ClearApplicant()
        {
            viewModel.GetApplicantByStatus(ApplicantStatus.Status.CompletedStudentInformationForm);
            return View(viewModel);
        }

        public ActionResult Index2()
        {
            var appliedCourseLogic = new AppliedCourseLogic();
            AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == 32);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AcceptOrReject(List<int> ids,int sessionId,bool isRejected)
        {
            try
            {
                if(ids != null && ids.Count > 0)
                {
                    var applications = new List<ApplicationForm>();
                    foreach(int id in ids)
                    {
                        var application = new ApplicationForm { Id = id };
                        applications.Add(application);
                    }

                    var applicationFormLogic = new ApplicationFormLogic();
                    bool accepted = applicationFormLogic.AcceptOrReject(applications,isRejected);
                    if(accepted)
                    {
                        var session = new Session { Id = sessionId };
                        viewModel.GetApplicationsBy(!isRejected,session);
                        SetMessage("Select Applications has be successfully Accepted.",Message.Category.Information);
                    }
                    else
                    {
                        SetMessage("Opeartion failed during selected Application Acceptance! Please try again.",
                            Message.Category.Information);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Operation failed! " + ex.Message,Message.Category.Error);
            }

            return PartialView("_ApplicationFormsGrid",viewModel.ApplicationForms);
        }

        [HttpPost]
        public ActionResult FindBy(int sessionId,bool isRejected)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var session = new Session { Id = sessionId };
                    viewModel.GetApplicationsBy(isRejected,session);
                }
            }
            catch(Exception ex)
            {
                TempData["msg"] = "Operation failed! " + ex.Message;
                SetMessage("Operation failed! " + ex.Message,Message.Category.Error);
            }

            return PartialView("_ApplicationFormsGrid",viewModel.ApplicationForms);
        }

        public ActionResult ApplicationForm(long fid)
        {
            try
            {
                var applicationFormViewModel = new ApplicationFormViewModel();
                var form = new ApplicationForm { Id = fid };

                applicationFormViewModel.GetApplicationFormBy(form);
                if(applicationFormViewModel.Person != null && applicationFormViewModel.Person.Id > 0)
                {
                    applicationFormViewModel.SetApplicantAppliedCourse(applicationFormViewModel.Person);
                }

                return View(applicationFormViewModel);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ActionResult StudentForm(long fid)
        {
            try
            {
                var studentFormViewModel = new StudentFormViewModel();
                var form = new ApplicationForm { Id = fid };

                studentFormViewModel.LoadApplicantionFormBy(fid);

                if(studentFormViewModel.ApplicationForm.Person != null &&
                    studentFormViewModel.ApplicationForm.Person.Id > 0)
                {
                    studentFormViewModel.LoadStudentInformationFormBy(studentFormViewModel.ApplicationForm.Person.Id);
                }

                return View(studentFormViewModel);
            }
            catch(Exception)
            {
                throw;
            }
        }
        public ActionResult ViewApplicantsPerSession(string fileName = null)
        {
            try
            {
                ViewBag.SessionId = Utility.PopulateAllSessionSelectListItem();
                ViewBag.ProgrammeId = Utility.PopulateAllProgrammeSelectListItem();
                if (fileName != null)
                {
                    new System.Media.SoundPlayer(@"C:\Windows\Media\tada.wav").Play();

                    return File(Server.MapPath("~/Content/tempFolder/" + fileName), "application/zip", fileName);
                }
            }
            catch (Exception ex) { throw ex; }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ViewApplicantsPerSession(AdmissionProcessingViewModel viewmodel)
        {
            try
            {
                ViewBag.SessionId = Utility.PopulateAllSessionSelectListItem();
                ViewBag.ProgrammeId = Utility.PopulateAllProgrammeSelectListItem();
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();

                if (viewmodel.Session!=null && viewmodel.Session.Id > 0 && viewmodel.Programme!=null && viewmodel.Programme.Id>0)
                {
                    viewmodel.ApplicationSummaryReport = applicationFormLogic.GetApplicantsPerSession(viewmodel.Session.Id ,viewmodel.Programme, viewmodel.DateFrom, viewmodel.DateTo);
                    TempData["ApplicantDetails"] = viewmodel.ApplicationSummaryReport;
                    TempData["SessionId"] = viewmodel.Session.Id;
                }
            }
            catch (Exception ex) { SetMessage(string.Format("A Error occured! {0}", ex.Message), Message.Category.Error); }

            return View(viewmodel);
        }

        public ActionResult DownloadIdCardPassport()
        {
            string zipFileName = null;
            List<ApplicationSummaryReport> ApplicantsPerSession = (List<ApplicationSummaryReport>)TempData["ApplicantDetails"];
            try
            {
                if (ApplicantsPerSession != null && ApplicantsPerSession.Count > 0)
                {
                    if (Directory.Exists(Server.MapPath("~/Content/tempFolder")))
                    {
                        Directory.Delete(Server.MapPath("~/Content/tempFolder"), true);
                        Directory.CreateDirectory(Server.MapPath("~/Content/tempFolder"));
                    }
                    else
                    {
                        Directory.CreateDirectory(Server.MapPath("~/Content/tempFolder"));
                    }

                    List<ApplicationSummaryReport> sort = new List<ApplicationSummaryReport>();

                    System.Web.UI.WebControls.GridView gv = new GridView();

                    foreach (var Applicant in ApplicantsPerSession)
                    {
                        string imagePath = Applicant.ImageUrl;

                        if (string.IsNullOrEmpty(imagePath)) { continue; }

                        string[] splitUrl = imagePath.Split('/');
                        string imageUrl = splitUrl[3];
                        FileInfo fileInfo = new FileInfo(imageUrl);
                        string fileExtension = fileInfo.Extension;
                        string newFileName = string.Format("{0}{1}", imageUrl.Split('.')[0], fileExtension);

                        if (!System.IO.File.Exists(Server.MapPath("~" + imagePath))) { continue; }

                        System.IO.File.Copy(Server.MapPath(imagePath), Server.MapPath(Path.Combine("~/Content/tempFolder/", newFileName)), true);

                        Applicant.ImageUrl = newFileName;

                        sort.Add(Applicant);
                    }

                    int SessionId = (int)TempData["SessionId"];
                    SessionLogic sessionLogic = new SessionLogic();

                    var SessionName = "";
                    Model.Model.Session session = new Model.Model.Session();
                    if (SessionId > 0)
                    {
                        session=sessionLogic.GetModelBy(f => f.Session_Id == SessionId);
                    }

                    SessionName = session != null ? session.Name.Replace("/", "_") : "Selected Session";
                    gv.DataSource = sort;
                    gv.Caption = string.Format("{0} SESSION", SessionName);
                    gv.DataBind();
                    SaveStudentDetailsToExcel(gv, "Applicants_Data.xls");

                    zipFileName = string.Format("ABSU_APPLICANT_Passports_For_{0}.zip", SessionName);
                    string downloadPath = "~/Content/tempFolder/";

                    using (ZipFile zip = new ZipFile())
                    {
                        string file = Server.MapPath(downloadPath);
                        zip.AddDirectory(file, "");

                        zip.Save(file + zipFileName);
                        string export = downloadPath + zipFileName;

                        Response.ClearContent();
                        Response.ClearHeaders();

                        //Set zip file name
                        Response.AppendHeader("content-disposition", "attachment; filename=" + zipFileName);

                        Response.Redirect(export, false);
                    }
                }
            }
            catch (Exception ex) { SetMessage(string.Format("Error Occured! {0}", ex.Message), Message.Category.Error); }

            new System.Media.SoundPlayer(@"C:\Windows\Media\tada.wav").Play();
            //return File();

            return RedirectToAction("ViewApplicantsPerSession", "AdmissionProcessing", new { fileName = zipFileName });
        }

        public FileResult DownloadFile()
        {
            List<ApplicationSummaryReport> ApplicantsPerSession = (List<ApplicationSummaryReport>)TempData["ApplicantDetails"];

            try
            {
                //Define file Type
                string fileType = "application/octet-stream";

                //Define Output Memory Stream
                var outputStream = new MemoryStream();

                using (ZipFile zipFile = new ZipFile())
                {
                    //Add Root Directory Name "Files" or Any string name
                    zipFile.AddDirectoryByName("Files");

                    foreach (var Applicant in ApplicantsPerSession)
                    {
                        zipFile.AddFile(Applicant.ImageUrl, "Files");
                    }

                    Response.ClearContent();
                    Response.ClearHeaders();

                    int SessionId = (int)TempData["SessionId"];

                    SessionLogic sessionLogic = new SessionLogic();

                    var SessionName = "";
                    Model.Model.Session session = new Model.Model.Session();
                    if (SessionId > 0)
                    {
                        session = sessionLogic.GetModelBy(f => f.Session_Id == SessionId);
                    }

                    SessionName = session != null ? session.Name.Replace("/", "_") : "Selected Session";

                    string ZipFileName = string.Format("ABSU_Applicants_Details_For_{0}.zip", SessionName);
                    ;                    //Set zip file name
                    Response.AppendHeader("content-disposition", "attachment; filename=" + ZipFileName);

                    //Save the zip content in output stream
                    zipFile.Save(outputStream);
                }
                //Set the cursor to start position
                outputStream.Position = 0;

                //Play audio File
                new System.Media.SoundPlayer(@"C:\Windows\Media\tada.wav").Play();

                //Dispance the stream
                return new FileStreamResult(outputStream, fileType);
            }
            catch (Exception ex) { throw ex; }
        }
        public void SaveStudentDetailsToExcel(GridView ExcelGridView, string fileName)
        {
            try
            {
                Response.Clear();

                Response.Charset = "";

                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                Response.ContentType = "application/vnd.ms-excel";

                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                ExcelGridView.RenderControl(htw);

                Response.Write(sw.ToString());
                string renderedGridView = sw.ToString();
                System.IO.File.WriteAllText(Server.MapPath(Path.Combine("~/Content/temp/", fileName)), renderedGridView);
            }
            catch (Exception ex) { throw ex; }
        }

        //private AdmissionProcessingViewModel viewModel;
        //private Abundance_NkEntities db = new Abundance_NkEntities();

        //public AdmissionProcessingController()
        //{
        //    viewModel = new AdmissionProcessingViewModel();
        //}

        //public ActionResult Index()
        //{
        //    ViewBag.SessionId = viewModel.SessionSelectList;

        //    return View(viewModel);
        //}

        //[AllowAnonymous]
        //public ActionResult Index2()
        //{
        //    try
        //    {
        //        AppliedCourseLogic appliedCourseLogic = new AppliedCourseLogic();
        //        PreviousEducationLogic previousEducationLogic = new PreviousEducationLogic();
        //        AppliedCourse appliedCourse = appliedCourseLogic.GetModelBy(a => a.Person_Id == 32);
        //        PreviousEducation previouseducation = previousEducationLogic.GetModelBy(p => p.Person_Id == 32);

        //        AdmissionCriteriaLogic admissionCriteriaLogic = new AdmissionCriteriaLogic();
        //        string rejectReason = admissionCriteriaLogic.EvaluateApplication(appliedCourse, previouseducation);
        //        ViewBag.RejectReason = rejectReason;
        //    }
        //    catch (Exception ex)
        //    {
        //        ViewBag.RejectReason = ex.Message;
        //        SetMessage(ex.Message, Message.Category.Error);
        //    }

        //    return View(viewModel);
        //}

        //[HttpPost]
        //public ActionResult AcceptOrReject(List<int> ids, int sessionId, bool isRejected)
        //{
        //    try
        //    {
        //        if (ids != null && ids.Count > 0)
        //        {
        //            List<ApplicationForm> applications = new List<ApplicationForm>();

        //            foreach (int id in ids)
        //            {
        //                ApplicationForm application = new ApplicationForm() { Id = id };
        //                applications.Add(application);
        //            }

        //            ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();
        //            bool accepted = applicationFormLogic.AcceptOrReject(applications, isRejected);
        //            if (accepted)
        //            {
        //                Session session = new Session() { Id = sessionId };
        //                viewModel.GetApplicationsBy(!isRejected, session);
        //                SetMessage("Select Applications has be successfully Accepted.", Message.Category.Information);
        //            }
        //            else
        //            {
        //                SetMessage("Opeartion failed during selected Application Acceptance! Please try again.", Message.Category.Information);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        SetMessage("Operation failed! " + ex.Message, Message.Category.Error);
        //    }

        //    return PartialView("_ApplicationFormsGrid", viewModel.ApplicationForms);
        //}

        //[HttpPost]
        //public ActionResult FindBy(int sessionId, bool isRejected)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            Session session = new Session() { Id = sessionId };
        //            viewModel.GetApplicationsBy(isRejected, session);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["msg"] = "Operation failed! " + ex.Message;
        //    }

        //    return PartialView("_ApplicationFormsGrid", viewModel.ApplicationForms);
        //}

        ////public ActionResult FindAllAcceptedBy(int sessionId)
        ////{
        ////    try
        ////    {
        ////        if (ModelState.IsValid)
        ////        {
        ////            Session session = new Session() { Id = sessionId };
        ////            viewModel.GetApplicationsBy(false, session);
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////    }

        ////    return PartialView("_ApplicationFormsGrid", viewModel.ApplicationForms);
        ////}

        ////[HttpPost]
        ////public void ApproveOrReject(List<int> ids, string status)
        ////{
        ////    try
        ////    {
        ////        TempData["msg"] = "Operation was successful.";
        ////    }
        ////    catch(Exception ex)
        ////    {
        ////        TempData["msg"] = "Operation failed! " + ex.Message;
        ////    }
        ////}

        ////[HttpPost]
        ////public ActionResult Index(AdmissionProcessingViewModel admissionProcessingViewModel)
        ////{
        ////    //bool rejected = admissionProcessingViewModel.Rejected;
        ////    //admissionProcessingViewModel.GetApplicationsBy(admissionProcessingViewModel.Rejected);

        ////    return View(admissionProcessingViewModel.ApplicationForms);
        ////}

        //// GET: /Admin/AdmissionProcessing/Details/5
        //public ActionResult Details(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    APPLICATION_FORM application_form = db.APPLICATION_FORM.Find(id);
        //    if (application_form == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(application_form);
        //}

        //// GET: /Admin/AdmissionProcessing/Create
        //public ActionResult Create()
        //{
        //    ViewBag.Application_Form_Setting_Id = new SelectList(db.APPLICATION_FORM_SETTING, "Application_Form_Setting_Id", "Exam_Venue");
        //    ViewBag.Application_Programme_Fee_Id = new SelectList(db.APPLICATION_PROGRAMME_FEE, "Application_Programme_Fee_Id", "Application_Programme_Fee_Id");
        //    ViewBag.Payment_Id = new SelectList(db.PAYMENT, "Payment_Id", "Invoice_Number");
        //    ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name");
        //    return View();
        //}

        //// POST: /Admin/AdmissionProcessing/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include="Application_Form_Id,Serial_Number,Application_Form_Number,Application_Form_Setting_Id,Application_Programme_Fee_Id,Payment_Id,Person_Id,Date_Submitted,Release,Rejected,Reject_Reason,Remarks")] APPLICATION_FORM application_form)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.APPLICATION_FORM.Add(application_form);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.Application_Form_Setting_Id = new SelectList(db.APPLICATION_FORM_SETTING, "Application_Form_Setting_Id", "Exam_Venue", application_form.Application_Form_Setting_Id);
        //    ViewBag.Application_Programme_Fee_Id = new SelectList(db.APPLICATION_PROGRAMME_FEE, "Application_Programme_Fee_Id", "Application_Programme_Fee_Id", application_form.Application_Programme_Fee_Id);
        //    ViewBag.Payment_Id = new SelectList(db.PAYMENT, "Payment_Id", "Invoice_Number", application_form.Payment_Id);
        //    ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name", application_form.Person_Id);
        //    return View(application_form);
        //}

        //// GET: /Admin/AdmissionProcessing/Edit/5
        //public ActionResult Edit(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    APPLICATION_FORM application_form = db.APPLICATION_FORM.Find(id);
        //    if (application_form == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.Application_Form_Setting_Id = new SelectList(db.APPLICATION_FORM_SETTING, "Application_Form_Setting_Id", "Exam_Venue", application_form.Application_Form_Setting_Id);
        //    ViewBag.Application_Programme_Fee_Id = new SelectList(db.APPLICATION_PROGRAMME_FEE, "Application_Programme_Fee_Id", "Application_Programme_Fee_Id", application_form.Application_Programme_Fee_Id);
        //    ViewBag.Payment_Id = new SelectList(db.PAYMENT, "Payment_Id", "Invoice_Number", application_form.Payment_Id);
        //    ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name", application_form.Person_Id);
        //    return View(application_form);
        //}

        //// POST: /Admin/AdmissionProcessing/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="Application_Form_Id,Serial_Number,Application_Form_Number,Application_Form_Setting_Id,Application_Programme_Fee_Id,Payment_Id,Person_Id,Date_Submitted,Release,Rejected,Reject_Reason,Remarks")] APPLICATION_FORM application_form)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(application_form).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.Application_Form_Setting_Id = new SelectList(db.APPLICATION_FORM_SETTING, "Application_Form_Setting_Id", "Exam_Venue", application_form.Application_Form_Setting_Id);
        //    ViewBag.Application_Programme_Fee_Id = new SelectList(db.APPLICATION_PROGRAMME_FEE, "Application_Programme_Fee_Id", "Application_Programme_Fee_Id", application_form.Application_Programme_Fee_Id);
        //    ViewBag.Payment_Id = new SelectList(db.PAYMENT, "Payment_Id", "Invoice_Number", application_form.Payment_Id);
        //    ViewBag.Person_Id = new SelectList(db.PERSON, "Person_Id", "First_Name", application_form.Person_Id);
        //    return View(application_form);
        //}

        //// GET: /Admin/AdmissionProcessing/Delete/5
        //public ActionResult Delete(long? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    APPLICATION_FORM application_form = db.APPLICATION_FORM.Find(id);
        //    if (application_form == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(application_form);
        //}

        //// POST: /Admin/AdmissionProcessing/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(long id)
        //{
        //    APPLICATION_FORM application_form = db.APPLICATION_FORM.Find(id);
        //    db.APPLICATION_FORM.Remove(application_form);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}