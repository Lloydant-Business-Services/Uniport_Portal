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
    [RoleBasedAttribute]
    public class ProgrammeDepartmentController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/ProgrammeDepartment
        public ActionResult Index()
        {
            var pROGRAMME_DEPARTMENT = db.PROGRAMME_DEPARTMENT.Include(p => p.DEPARTMENT).Include(p => p.PROGRAMME);
            return View(pROGRAMME_DEPARTMENT.ToList());
        }

        // GET: Admin/ProgrammeDepartment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROGRAMME_DEPARTMENT pROGRAMME_DEPARTMENT = db.PROGRAMME_DEPARTMENT.Find(id);
            if (pROGRAMME_DEPARTMENT == null)
            {
                return HttpNotFound();
            }
            return View(pROGRAMME_DEPARTMENT);
        }

        // GET: Admin/ProgrammeDepartment/Create
        public ActionResult Create()
        {
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name");
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name");
            return View();
        }

        // POST: Admin/ProgrammeDepartment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Programme_Department_Id,Programme_Id,Department_Id")] PROGRAMME_DEPARTMENT pROGRAMME_DEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                db.PROGRAMME_DEPARTMENT.Add(pROGRAMME_DEPARTMENT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", pROGRAMME_DEPARTMENT.Department_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", pROGRAMME_DEPARTMENT.Programme_Id);
            return View(pROGRAMME_DEPARTMENT);
        }

        // GET: Admin/ProgrammeDepartment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROGRAMME_DEPARTMENT pROGRAMME_DEPARTMENT = db.PROGRAMME_DEPARTMENT.Find(id);
            if (pROGRAMME_DEPARTMENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", pROGRAMME_DEPARTMENT.Department_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", pROGRAMME_DEPARTMENT.Programme_Id);
            return View(pROGRAMME_DEPARTMENT);
        }

        // POST: Admin/ProgrammeDepartment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Programme_Department_Id,Programme_Id,Department_Id")] PROGRAMME_DEPARTMENT pROGRAMME_DEPARTMENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pROGRAMME_DEPARTMENT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", pROGRAMME_DEPARTMENT.Department_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", pROGRAMME_DEPARTMENT.Programme_Id);
            return View(pROGRAMME_DEPARTMENT);
        }

        // GET: Admin/ProgrammeDepartment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PROGRAMME_DEPARTMENT pROGRAMME_DEPARTMENT = db.PROGRAMME_DEPARTMENT.Find(id);
            if (pROGRAMME_DEPARTMENT == null)
            {
                return HttpNotFound();
            }
            return View(pROGRAMME_DEPARTMENT);
        }

        // POST: Admin/ProgrammeDepartment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PROGRAMME_DEPARTMENT pROGRAMME_DEPARTMENT = db.PROGRAMME_DEPARTMENT.Find(id);
            db.PROGRAMME_DEPARTMENT.Remove(pROGRAMME_DEPARTMENT);
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
