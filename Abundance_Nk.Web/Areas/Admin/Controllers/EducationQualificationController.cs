using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity;
using Abundance_Nk.Web.Controllers;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
   [RoleBasedAttribute]
    public class EducationQualificationController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/EducationQualification
        public ActionResult Index()
        {
            return View(db.EDUCATIONAL_QUALIFICATION.ToList());
        }

        // GET: Admin/EducationQualification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EDUCATIONAL_QUALIFICATION eDUCATIONAL_QUALIFICATION = db.EDUCATIONAL_QUALIFICATION.Find(id);
            if (eDUCATIONAL_QUALIFICATION == null)
            {
                return HttpNotFound();
            }
            return View(eDUCATIONAL_QUALIFICATION);
        }

        // GET: Admin/EducationQualification/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/EducationQualification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Educational_Qualification_Id,Educational_Qualification_Abbreviation,Educational_Qualification_Name")] EDUCATIONAL_QUALIFICATION eDUCATIONAL_QUALIFICATION)
        {
            if (ModelState.IsValid)
            {
                db.EDUCATIONAL_QUALIFICATION.Add(eDUCATIONAL_QUALIFICATION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eDUCATIONAL_QUALIFICATION);
        }

        // GET: Admin/EducationQualification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EDUCATIONAL_QUALIFICATION eDUCATIONAL_QUALIFICATION = db.EDUCATIONAL_QUALIFICATION.Find(id);
            if (eDUCATIONAL_QUALIFICATION == null)
            {
                return HttpNotFound();
            }
            return View(eDUCATIONAL_QUALIFICATION);
        }

        // POST: Admin/EducationQualification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Educational_Qualification_Id,Educational_Qualification_Abbreviation,Educational_Qualification_Name")] EDUCATIONAL_QUALIFICATION eDUCATIONAL_QUALIFICATION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eDUCATIONAL_QUALIFICATION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eDUCATIONAL_QUALIFICATION);
        }

        // GET: Admin/EducationQualification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EDUCATIONAL_QUALIFICATION eDUCATIONAL_QUALIFICATION = db.EDUCATIONAL_QUALIFICATION.Find(id);
            if (eDUCATIONAL_QUALIFICATION == null)
            {
                return HttpNotFound();
            }
            return View(eDUCATIONAL_QUALIFICATION);
        }

        // POST: Admin/EducationQualification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EDUCATIONAL_QUALIFICATION eDUCATIONAL_QUALIFICATION = db.EDUCATIONAL_QUALIFICATION.Find(id);
            db.EDUCATIONAL_QUALIFICATION.Remove(eDUCATIONAL_QUALIFICATION);
            db.SaveChanges();
            return RedirectToAction("Index");
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
