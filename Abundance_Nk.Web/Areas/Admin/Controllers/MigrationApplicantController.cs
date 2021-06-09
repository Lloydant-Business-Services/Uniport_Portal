using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class MigrationApplicantController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/MigrationApplicant
        public ActionResult Index()
        {
            return View(db.MIGRATION_APPLICANTS.ToList());
        }

        // GET: Admin/MigrationApplicant/Details/5
        public ActionResult Details(double? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MIGRATION_APPLICANTS mIGRATION_APPLICANTS = db.MIGRATION_APPLICANTS.Find(id);
            if (mIGRATION_APPLICANTS == null)
            {
                return HttpNotFound();
            }
            return View(mIGRATION_APPLICANTS);
        }

        // GET: Admin/MigrationApplicant/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/MigrationApplicant/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SNO,JAMB_REG_NO,NAMES,STATE,LGA,FIRST_CHOICE,SCORE,QUALIFICATION_1,QUALIFICATION_2,DEPARTMENT")] MIGRATION_APPLICANTS mIGRATION_APPLICANTS)
        {
            if (ModelState.IsValid)
            {
                db.MIGRATION_APPLICANTS.Add(mIGRATION_APPLICANTS);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mIGRATION_APPLICANTS);
        }

        // GET: Admin/MigrationApplicant/Edit/5
        public ActionResult Edit(double? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MIGRATION_APPLICANTS mIGRATION_APPLICANTS = db.MIGRATION_APPLICANTS.Find(id);
            if (mIGRATION_APPLICANTS == null)
            {
                return HttpNotFound();
            }
            return View(mIGRATION_APPLICANTS);
        }

        // POST: Admin/MigrationApplicant/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SNO,JAMB_REG_NO,NAMES,STATE,LGA,FIRST_CHOICE,SCORE,QUALIFICATION_1,QUALIFICATION_2,DEPARTMENT")] MIGRATION_APPLICANTS mIGRATION_APPLICANTS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mIGRATION_APPLICANTS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mIGRATION_APPLICANTS);
        }

        // GET: Admin/MigrationApplicant/Delete/5
        public ActionResult Delete(double? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MIGRATION_APPLICANTS mIGRATION_APPLICANTS = db.MIGRATION_APPLICANTS.Find(id);
            if (mIGRATION_APPLICANTS == null)
            {
                return HttpNotFound();
            }
            return View(mIGRATION_APPLICANTS);
        }

        // POST: Admin/MigrationApplicant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(double id)
        {
            MIGRATION_APPLICANTS mIGRATION_APPLICANTS = db.MIGRATION_APPLICANTS.Find(id);
            db.MIGRATION_APPLICANTS.Remove(mIGRATION_APPLICANTS);
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
