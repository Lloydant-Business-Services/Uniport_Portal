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
    public class DegreeController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/Degree
        public ActionResult Index()
        {
            var dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT = db.DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Include(d => d.DEPARTMENT).Include(d => d.PROGRAMME);
            return View(dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.ToList());
        }

        // GET: Admin/Degree/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT = db.DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Find(id);
            if (dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT == null)
            {
                return HttpNotFound();
            }
            return View(dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT);
        }

        // GET: Admin/Degree/Create
        public ActionResult Create()
        {
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name");
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name");
            return View();
        }

        // POST: Admin/Degree/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Department_Id,Programme_Id,Degree_Name,Duration")] DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                db.DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Add(dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Department_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Programme_Id);
            return View(dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT);
        }

        // GET: Admin/Degree/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT = db.DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Find(id);
            if (dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Department_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Programme_Id);
            return View(dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT);
        }

        // POST: Admin/Degree/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Department_Id,Programme_Id,Degree_Name,Duration")] DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Department_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Programme_Id);
            return View(dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT);
        }

        // GET: Admin/Degree/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT = db.DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Find(id);
            if (dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT == null)
            {
                return HttpNotFound();
            }
            return View(dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT);
        }

        // POST: Admin/Degree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT = db.DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Find(id);
            db.DEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT.Remove(dEGREE_AWARDS_BY_PROGRAMME_DEPARTMENT);
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
