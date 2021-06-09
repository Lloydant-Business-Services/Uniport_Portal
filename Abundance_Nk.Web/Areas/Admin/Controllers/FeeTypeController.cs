using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Abundance_Nk.Model.Entity;

namespace Abundance_Nk.Web.Areas.Admin.Controllers
{
    public class FeeTypeController : Controller
    {
        private Abundance_NkEntities db = new Abundance_NkEntities();

        // GET: Admin/FeeType
        public async Task<ActionResult> Index()
        {
            return View(await db.FEE_TYPE.ToListAsync());
        }

        // GET: Admin/FeeType/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE_TYPE fEE_TYPE = await db.FEE_TYPE.FindAsync(id);
            if (fEE_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(fEE_TYPE);
        }

        // GET: Admin/FeeType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/FeeType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Fee_Type_Id,Fee_Type_Name,Fee_Type_Description")] FEE_TYPE fEE_TYPE)
        {
            if (ModelState.IsValid)
            {
                db.FEE_TYPE.Add(fEE_TYPE);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(fEE_TYPE);
        }

        // GET: Admin/FeeType/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE_TYPE fEE_TYPE = await db.FEE_TYPE.FindAsync(id);
            if (fEE_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(fEE_TYPE);
        }

        // POST: Admin/FeeType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Fee_Type_Id,Fee_Type_Name,Fee_Type_Description,Active")] FEE_TYPE fEE_TYPE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fEE_TYPE).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(fEE_TYPE);
        }

        // GET: Admin/FeeType/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FEE_TYPE fEE_TYPE = await db.FEE_TYPE.FindAsync(id);
            if (fEE_TYPE == null)
            {
                return HttpNotFound();
            }
            return View(fEE_TYPE);
        }

        // POST: Admin/FeeType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            FEE_TYPE fEE_TYPE = await db.FEE_TYPE.FindAsync(id);
            db.FEE_TYPE.Remove(fEE_TYPE);
            await db.SaveChangesAsync();
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
