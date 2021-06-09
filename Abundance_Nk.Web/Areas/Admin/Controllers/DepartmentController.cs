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
    public class DepartmentController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/Department
        public ActionResult Index()
        {
            var dEPARTMENT = db.DEPARTMENT.Include(d => d.FACULTY);
            return View(dEPARTMENT.ToList());
        }

        // GET: Admin/Department/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPARTMENT dEPARTMENT = db.DEPARTMENT.Find(id);
            if (dEPARTMENT == null)
            {
                return HttpNotFound();
            }
            return View(dEPARTMENT);
        }

        // GET: Admin/Department/Create
        public ActionResult Create()
        {
            ViewBag.Faculty_Id = new SelectList(db.FACULTY, "Faculty_Id", "Faculty_Name");
            return View();
        }

        // POST: Admin/Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Department_Id,Department_Name,Department_Code,Faculty_Id")] DEPARTMENT dEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                db.DEPARTMENT.Add(dEPARTMENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Faculty_Id = new SelectList(db.FACULTY, "Faculty_Id", "Faculty_Name", dEPARTMENT.Faculty_Id);
            return View(dEPARTMENT);
        }

        // GET: Admin/Department/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPARTMENT dEPARTMENT = db.DEPARTMENT.Find(id);
            if (dEPARTMENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.Faculty_Id = new SelectList(db.FACULTY, "Faculty_Id", "Faculty_Name", dEPARTMENT.Faculty_Id);
            return View(dEPARTMENT);
        }

        // POST: Admin/Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Department_Id,Department_Name,Department_Code,Faculty_Id")] DEPARTMENT dEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dEPARTMENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Faculty_Id = new SelectList(db.FACULTY, "Faculty_Id", "Faculty_Name", dEPARTMENT.Faculty_Id);
            return View(dEPARTMENT);
        }

        // GET: Admin/Department/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPARTMENT dEPARTMENT = db.DEPARTMENT.Find(id);
            if (dEPARTMENT == null)
            {
                return HttpNotFound();
            }
            return View(dEPARTMENT);
        }

        // POST: Admin/Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DEPARTMENT dEPARTMENT = db.DEPARTMENT.Find(id);
            db.DEPARTMENT.Remove(dEPARTMENT);
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
