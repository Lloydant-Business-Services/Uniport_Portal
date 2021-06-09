using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    [RoleBasedAttribute]
    public class PostJambController :BaseController
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        //
        // GET: /Admin/PostJamb/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(PostjambResultSupportViewModel vModel,FormCollection f)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    if(vModel.JambNumber != null)
                    {
                        var result = new PutmeResult();
                        var PostUtmeResultLogic = new PutmeResultLogic();
                        result =
                            PostUtmeResultLogic.GetModelsBy(
                                m => m.REGNO == vModel.JambNumber || m.EXAMNO == vModel.JambNumber).FirstOrDefault();
                        if(result == null || result.Id <= 0)
                        {
                            SetMessage(
                                "Registration Number / Jamb No was not found! Please check that you have typed in the correct detail",
                                Message.Category.Error);
                            return View(vModel);
                        }
                        vModel.putmeResult = result;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            TempData["ResultViewModel"] = vModel;
            return View(vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateResult(PostjambResultSupportViewModel vModel,FormCollection f)
        {
            try
            {
                var errors = from modelstate in ModelState.AsQueryable().Where(q => q.Value.Errors.Count > 0) select new { Title = modelstate.Key };
                string dd = f.AllKeys[0];
                ModelState["putmeResult.RegNo"].Errors.Clear();
                if (ModelState.IsValid && f.AllKeys[0] != null)
                {
                    if(vModel.putmeResult != null && vModel.putmeResult.Id > 0)
                    {
                        var putme = new PutmeResult();
                        var putmeLogic = new PutmeResultLogic();
                        putme = vModel.putmeResult;

                        string operation = "UPDATE";
                        string action = "MODIFY APPLICANT JAMB RESULT";
                        string client = Request.LogonUserIdentity.Name + " (" + HttpContext.Request.UserHostAddress +
                                        ")";

                        var putmeAudit = new PutmeResultAudit();
                        var loggeduser = new UserLogic();
                        putmeAudit.User = loggeduser.GetModelBy(u => u.User_Name == User.Identity.Name);
                        putmeAudit.Operation = operation;
                        putmeAudit.Action = action;
                        putmeAudit.Time = DateTime.Now;
                        putmeAudit.Client = client;

                        putmeLogic.Modify(putme,putmeAudit);
                        TempData["Message"] = "Record was successfully updated";
                    }
                }
            }
            catch(Exception ex)
            {
                TempData["Message"] = "System Message :" + ex.Message;
                return RedirectToAction("index");
            }

            return RedirectToAction("index");
        }
        public ActionResult PostJambResult()
        {
            PostjambResultSupportViewModel viewModel = new PostjambResultSupportViewModel();
            try
            {
                ViewBag.SessionId = viewModel.SessionSelectList;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostJambResult(PostjambResultSupportViewModel viewModel)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();

                if (viewModel.Session != null)
                {
                    var gv = new GridView();

                    viewModel.PutmeResults = applicationFormLogic.GetPutmeResults(viewModel.Session);

                    if (viewModel.PutmeResults != null && viewModel.PutmeResults.Count > 0)
                    {
                        gv.DataSource = viewModel.PutmeResults;

                        gv.Caption = "PUTME RESULT";
                        gv.DataBind();
                        string filename = "Putme result";

                        ViewBag.SessionId = viewModel.SessionSelectList;

                        return new DownloadFileActionResult(gv, filename + ".xls");
                    }

                    Response.Write("No data available for download");
                    Response.End();

                    ViewBag.SessionId = viewModel.SessionSelectList;

                    return new JavaScriptResult();

                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            ViewBag.SessionId = viewModel.SessionSelectList;

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostJambResultAll(PostjambResultSupportViewModel viewModel)
        {
            try
            {
                ApplicationFormLogic applicationFormLogic = new ApplicationFormLogic();

                viewModel.Session = new Session();
                viewModel.PutmeResults = applicationFormLogic.GetPutmeResults(viewModel.Session);

                var gv = new GridView();

                if (viewModel.PutmeResults != null && viewModel.PutmeResults.Count > 0)
                {
                    gv.DataSource = viewModel.PutmeResults;

                    gv.Caption = "PUTME RESULT";
                    gv.DataBind();
                    string filename = "Putme result";

                    ViewBag.SessionId = viewModel.SessionSelectList;

                    return new DownloadFileActionResult(gv, filename + ".xls");
                }

                Response.Write("No data available for download");
                Response.End();

                ViewBag.SessionId = viewModel.SessionSelectList;

                return new JavaScriptResult();


            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            ViewBag.SessionId = viewModel.SessionSelectList;

            return View("PostJambResult", viewModel);
        }
        public ActionResult StudentDetails()
        {
            PostjambResultSupportViewModel viewModel = new PostjambResultSupportViewModel();
            try
            {
                ViewBag.SessionId = viewModel.SessionSelectList;
                ViewBag.ProgrammeId = viewModel.ProgrammeSelectList;
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StudentDetails(PostjambResultSupportViewModel viewModel)
        {
            try
            {
                StudentLogic studentLogic = new StudentLogic();

                if (viewModel.Session != null && viewModel.Programme != null)
                {
                    viewModel.StudentDetailList = studentLogic.GetStudentDetialBy(viewModel.Session, viewModel.Programme);
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message, Message.Category.Error);
            }

            ViewBag.SessionId = viewModel.SessionSelectList;
            ViewBag.ProgrammeId = viewModel.ProgrammeSelectList;

            return View(viewModel);
        }
    }
}