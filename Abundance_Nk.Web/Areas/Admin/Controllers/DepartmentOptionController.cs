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
    public class DepartmentOptionController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/DepartmentOption
        public ActionResult Index()
        {
            var dEPARTMENT_OPTION = db.DEPARTMENT_OPTION.Include(d => d.DEPARTMENT);
            return View(dEPARTMENT_OPTION.ToList());
        }

        // GET: Admin/DepartmentOption/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPARTMENT_OPTION dEPARTMENT_OPTION = db.DEPARTMENT_OPTION.Find(id);
            if (dEPARTMENT_OPTION == null)
            {
                return HttpNotFound();
            }
            return View(dEPARTMENT_OPTION);
        }

        // GET: Admin/DepartmentOption/Create
        public ActionResult Create()
        {
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name");
            return View();
        }

        // POST: Admin/DepartmentOption/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Department_Option_Id,Department_Id,Department_Option_Name,Activated")] DEPARTMENT_OPTION dEPARTMENT_OPTION)
        {
            if (ModelState.IsValid)
            {
                db.DEPARTMENT_OPTION.Add(dEPARTMENT_OPTION);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", dEPARTMENT_OPTION.Department_Id);
            return View(dEPARTMENT_OPTION);
        }

        // GET: Admin/DepartmentOption/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPARTMENT_OPTION dEPARTMENT_OPTION = db.DEPARTMENT_OPTION.Find(id);
            if (dEPARTMENT_OPTION == null)
            {
                return HttpNotFound();
            }
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", dEPARTMENT_OPTION.Department_Id);
            return View(dEPARTMENT_OPTION);
        }

        // POST: Admin/DepartmentOption/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Department_Option_Id,Department_Id,Department_Option_Name,Activated")] DEPARTMENT_OPTION dEPARTMENT_OPTION)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dEPARTMENT_OPTION).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Department_Id = new SelectList(db.DEPARTMENT, "Department_Id", "Department_Name", dEPARTMENT_OPTION.Department_Id);
            return View(dEPARTMENT_OPTION);
        }

        // GET: Admin/DepartmentOption/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DEPARTMENT_OPTION dEPARTMENT_OPTION = db.DEPARTMENT_OPTION.Find(id);
            if (dEPARTMENT_OPTION == null)
            {
                return HttpNotFound();
            }
            return View(dEPARTMENT_OPTION);
        }

        // POST: Admin/DepartmentOption/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DEPARTMENT_OPTION dEPARTMENT_OPTION = db.DEPARTMENT_OPTION.Find(id);
            db.DEPARTMENT_OPTION.Remove(dEPARTMENT_OPTION);
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
