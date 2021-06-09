using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Business;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Model.Model;
using Abundance_Nk.Web.Areas.Admin.ViewModels;
using Abundance_Nk.Web.Controllers;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class HostelController : BaseController
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();
        private HostelViewModel viewModel = null;

        // GET: Admin/Hostel
        public ActionResult Index()
        {
            var hOSTEL = db.HOSTEL.Include(h => h.HOSTEL_TYPE);
            return View(hOSTEL.ToList());
        }

        // GET: Admin/Hostel/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSTEL hOSTEL = db.HOSTEL.Find(id);
            if (hOSTEL == null)
            {
                return HttpNotFound();
            }
            return View(hOSTEL);
        }

        // GET: Admin/Hostel/Create
        public ActionResult Create()
        {
            ViewBag.Hostel_Type_Id = new SelectList(db.HOSTEL_TYPE, "Hostel_Type_Id", "Hostel_Type_Name");
            return View();
        }

        // POST: Admin/Hostel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Hostel_Id,Hostel_Type_Id,Hostel_Name,Capacity,Description,Date_Entered,Activated")] HOSTEL hOSTEL)
        {
            if (ModelState.IsValid)
            {
                db.HOSTEL.Add(hOSTEL);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Hostel_Type_Id = new SelectList(db.HOSTEL_TYPE, "Hostel_Type_Id", "Hostel_Type_Name", hOSTEL.Hostel_Type_Id);
            return View(hOSTEL);
        }

        // GET: Admin/Hostel/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSTEL hOSTEL = db.HOSTEL.Find(id);
            if (hOSTEL == null)
            {
                return HttpNotFound();
            }
            ViewBag.Hostel_Type_Id = new SelectList(db.HOSTEL_TYPE, "Hostel_Type_Id", "Hostel_Type_Name", hOSTEL.Hostel_Type_Id);
            return View(hOSTEL);
        }

        // POST: Admin/Hostel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Hostel_Id,Hostel_Type_Id,Hostel_Name,Capacity,Description,Date_Entered,Activated")] HOSTEL hOSTEL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hOSTEL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Hostel_Type_Id = new SelectList(db.HOSTEL_TYPE, "Hostel_Type_Id", "Hostel_Type_Name", hOSTEL.Hostel_Type_Id);
            return View(hOSTEL);
        }

        // GET: Admin/Hostel/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HOSTEL hOSTEL = db.HOSTEL.Find(id);
            if (hOSTEL == null)
            {
                return HttpNotFound();
            }
            return View(hOSTEL);
        }

        // POST: Admin/Hostel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HOSTEL hOSTEL = db.HOSTEL.Find(id);
            db.HOSTEL.Remove(hOSTEL);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ViewHostelTypes()
        {
            try
            {
                viewModel = new HostelViewModel();
                HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();

                viewModel.HostelTypes = hostelTypeLogic.GetAll();
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult HostelTypeDetails(int id)
        {
            try
            {
                if (id > 0)
                {
                    viewModel = new HostelViewModel();
                    HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                    HostelType hostelType = hostelTypeLogic.GetModelBy(h => h.Hostel_Type_Id == id);

                    viewModel.HostelType = hostelType;
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }
            return View(viewModel);
        }

        public ActionResult CreateHostelType()
        {
            HostelViewModel viewModel = new HostelViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateHostelType(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                    hostelTypeLogic.Create(viewModel.HostelType);

                    return RedirectToAction("ViewHostelTypes");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult EditHostelType(int id)
        {
            viewModel = new HostelViewModel();
            try
            {
                if (id == null || id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                HostelType hostelType = hostelTypeLogic.GetModelBy(h => h.Hostel_Type_Id == id);
                if (hostelType == null)
                {
                    return HttpNotFound();
                }

                viewModel.HostelType = hostelType;
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditHostelType(HostelViewModel viewModel)
        {
            try
            {
                if (viewModel != null)
                {
                    HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                    hostelTypeLogic.Modify(viewModel.HostelType);

                    SetMessage("Operation Successful!", Message.Category.Information);
                    return RedirectToAction("ViewHostelTypes");
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }

        public ActionResult DeleteHostelType(int id)
        {
            try
            {
                if (id == null || id <= 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                viewModel = new HostelViewModel();
                HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                viewModel.HostelType = hostelTypeLogic.GetModelBy(h => h.Hostel_Type_Id == id);

                if (viewModel.HostelType == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteHostelType(HostelViewModel viewModel)
        {
            try
            {
                HostelTypeLogic hostelTypeLogic = new HostelTypeLogic();
                hostelTypeLogic.Delete(h => h.Hostel_Type_Id == viewModel.HostelType.Hostel_Type_Id);

                SetMessage("Operation Successful!", Message.Category.Information);
                return RedirectToAction("ViewHostelTypes");
            }
            catch (Exception ex)
            {
                SetMessage("Error! " + ex.Message, Message.Category.Error);
            }

            return View(viewModel);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
