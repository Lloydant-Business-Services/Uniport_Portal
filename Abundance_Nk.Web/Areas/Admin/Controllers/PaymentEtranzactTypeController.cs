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
    public class PaymentEtranzactTypeController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/PaymentEtranzactType
        public ActionResult Index()
        {
            var pAYMENT_ETRANZACT_TYPE = db.PAYMENT_ETRANZACT_TYPE.Include(p => p.FEE_TYPE).Include(p => p.LEVEL).Include(p => p.PAYMENT_MODE).Include(p => p.PROGRAMME).Include(p => p.SESSION);
            return View(pAYMENT_ETRANZACT_TYPE.ToList());
        }

        // GET: Admin/PaymentEtranzactType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYMENT_ETRANZACT_TYPE pAYMENT_ETRANZACT_TYPE = db.PAYMENT_ETRANZACT_TYPE.Find(id);
            if (pAYMENT_ETRANZACT_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(pAYMENT_ETRANZACT_TYPE);
        }

        // GET: Admin/PaymentEtranzactType/Create
        public ActionResult Create()
        {
            ViewBag.Fee_Type_Id = new SelectList(db.FEE_TYPE, "Fee_Type_Id", "Fee_Type_Name");
            ViewBag.Level_Id = new SelectList(db.LEVEL, "Level_Id", "Level_Name");
            ViewBag.Payment_Mode_Id = new SelectList(db.PAYMENT_MODE, "Payment_Mode_Id", "Payment_Mode_Name");
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name");
            ViewBag.Session_Id = new SelectList(db.SESSION, "Session_Id", "Session_Name");
            return View();
        }

        // POST: Admin/PaymentEtranzactType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Payment_Etranzact_Type_Id,Payment_Etranzact_Type_Name,Fee_Type_Id,Programme_Id,Level_Id,Payment_Mode_Id,Session_Id")] PAYMENT_ETRANZACT_TYPE pAYMENT_ETRANZACT_TYPE)
        {
            if (ModelState.IsValid)
            {
                db.PAYMENT_ETRANZACT_TYPE.Add(pAYMENT_ETRANZACT_TYPE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Fee_Type_Id = new SelectList(db.FEE_TYPE, "Fee_Type_Id", "Fee_Type_Name", pAYMENT_ETRANZACT_TYPE.Fee_Type_Id);
            ViewBag.Level_Id = new SelectList(db.LEVEL, "Level_Id", "Level_Name", pAYMENT_ETRANZACT_TYPE.Level_Id);
            ViewBag.Payment_Mode_Id = new SelectList(db.PAYMENT_MODE, "Payment_Mode_Id", "Payment_Mode_Name", pAYMENT_ETRANZACT_TYPE.Payment_Mode_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", pAYMENT_ETRANZACT_TYPE.Programme_Id);
            ViewBag.Session_Id = new SelectList(db.SESSION, "Session_Id", "Session_Name", pAYMENT_ETRANZACT_TYPE.Session_Id);
            return View(pAYMENT_ETRANZACT_TYPE);
        }

        // GET: Admin/PaymentEtranzactType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYMENT_ETRANZACT_TYPE pAYMENT_ETRANZACT_TYPE = db.PAYMENT_ETRANZACT_TYPE.Find(id);
            if (pAYMENT_ETRANZACT_TYPE == null)
            {
                return HttpNotFound();
            }
            ViewBag.Fee_Type_Id = new SelectList(db.FEE_TYPE, "Fee_Type_Id", "Fee_Type_Name", pAYMENT_ETRANZACT_TYPE.Fee_Type_Id);
            ViewBag.Level_Id = new SelectList(db.LEVEL, "Level_Id", "Level_Name", pAYMENT_ETRANZACT_TYPE.Level_Id);
            ViewBag.Payment_Mode_Id = new SelectList(db.PAYMENT_MODE, "Payment_Mode_Id", "Payment_Mode_Name", pAYMENT_ETRANZACT_TYPE.Payment_Mode_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", pAYMENT_ETRANZACT_TYPE.Programme_Id);
            ViewBag.Session_Id = new SelectList(db.SESSION, "Session_Id", "Session_Name", pAYMENT_ETRANZACT_TYPE.Session_Id);
            return View(pAYMENT_ETRANZACT_TYPE);
        }

        // POST: Admin/PaymentEtranzactType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Payment_Etranzact_Type_Id,Payment_Etranzact_Type_Name,Fee_Type_Id,Programme_Id,Level_Id,Payment_Mode_Id,Session_Id")] PAYMENT_ETRANZACT_TYPE pAYMENT_ETRANZACT_TYPE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pAYMENT_ETRANZACT_TYPE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Fee_Type_Id = new SelectList(db.FEE_TYPE, "Fee_Type_Id", "Fee_Type_Name", pAYMENT_ETRANZACT_TYPE.Fee_Type_Id);
            ViewBag.Level_Id = new SelectList(db.LEVEL, "Level_Id", "Level_Name", pAYMENT_ETRANZACT_TYPE.Level_Id);
            ViewBag.Payment_Mode_Id = new SelectList(db.PAYMENT_MODE, "Payment_Mode_Id", "Payment_Mode_Name", pAYMENT_ETRANZACT_TYPE.Payment_Mode_Id);
            ViewBag.Programme_Id = new SelectList(db.PROGRAMME, "Programme_Id", "Programme_Name", pAYMENT_ETRANZACT_TYPE.Programme_Id);
            ViewBag.Session_Id = new SelectList(db.SESSION, "Session_Id", "Session_Name", pAYMENT_ETRANZACT_TYPE.Session_Id);
            return View(pAYMENT_ETRANZACT_TYPE);
        }

        // GET: Admin/PaymentEtranzactType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAYMENT_ETRANZACT_TYPE pAYMENT_ETRANZACT_TYPE = db.PAYMENT_ETRANZACT_TYPE.Find(id);
            if (pAYMENT_ETRANZACT_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(pAYMENT_ETRANZACT_TYPE);
        }

        // POST: Admin/PaymentEtranzactType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PAYMENT_ETRANZACT_TYPE pAYMENT_ETRANZACT_TYPE = db.PAYMENT_ETRANZACT_TYPE.Find(id);
            db.PAYMENT_ETRANZACT_TYPE.Remove(pAYMENT_ETRANZACT_TYPE);
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
