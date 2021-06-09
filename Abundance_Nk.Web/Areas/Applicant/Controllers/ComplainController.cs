using Abundance_Nk.Business;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Abundance_Nk.Web.Areas.Applicant.Controllers
{
    public class ComplainController :BaseController
    {
        // GET: Applicant/Complain
        [AllowAnonymous]
        public ActionResult Index()
        {
            var log = new ComplaintLog();
            try
            {

            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(log);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ComplaintLog log)
        {
            try
            {
                if(log != null)
                {
                    var logLogic = new ComplainLogLogic();
                    log = logLogic.Create(log);
                    SetMessage(
                        "Your complaint has been received with ticket ID " + log.TicketID +
                        ". Your issue would be resolved shortly.",Message.Category.Information);
                    log = new ComplaintLog();
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(log);
        }

        [AllowAnonymous]
        public ActionResult Status()
        {
            var log = new ComplaintLog();
            try
            {
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(log);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Status(ComplaintLog log)
        {
            try
            {
                if(log != null)
                {
                    var logLogic = new ComplainLogLogic();
                    log = logLogic.GetBy(log.TicketID);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(log);
        }

        public ActionResult View()
        {
            var logList = new List<ComplaintLog>();
            var logLogic = new ComplainLogLogic();
            try
            {
                logList = logLogic.GetAllUnResolved();
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return View(logList);
        }

        public ActionResult Resolve(long id)
        {
            var log = new ComplaintLog();
            var logLogic = new ComplainLogLogic();
            try
            {
                log = logLogic.GetBy(id);
                if(log != null)
                {
                    return View(log);
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resolve(ComplaintLog log)
        {
            var logLogic = new ComplainLogLogic();
            try
            {
                if(log != null)
                {
                    if(logLogic.Modify(log))
                    {
                        SetMessage("Issue has been updated! ",Message.Category.Information);
                    }
                }
            }
            catch(Exception ex)
            {
                SetMessage("Error Occurred! " + ex.Message,Message.Category.Error);
            }
            return RedirectToAction("View");
        }

        [AllowAnonymous]
        public ActionResult Fix()
        {
            return View();
        }
    }
}