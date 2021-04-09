using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InteriorDesign.Models;

namespace InteriorDesign.Controllers
{
    public class TestCalllsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TestCallls
        public ActionResult Index()
        {
            return View(db.TestCallls.ToList());
        }

        // GET: TestCallls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCalll testCalll = db.TestCallls.Find(id);
            if (testCalll == null)
            {
                return HttpNotFound();
            }
            return View(testCalll);
        }

        // GET: TestCallls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestCallls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,value")] TestCalll testCalll)
        {
            if (ModelState.IsValid)
            {
                db.TestCallls.Add(testCalll);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testCalll);
        }

        // GET: TestCallls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCalll testCalll = db.TestCallls.Find(id);
            if (testCalll == null)
            {
                return HttpNotFound();
            }
            return View(testCalll);
        }

        // POST: TestCallls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,value")] TestCalll testCalll)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testCalll).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testCalll);
        }

        // GET: TestCallls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestCalll testCalll = db.TestCallls.Find(id);
            if (testCalll == null)
            {
                return HttpNotFound();
            }
            return View(testCalll);
        }

        // POST: TestCallls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestCalll testCalll = db.TestCallls.Find(id);
            db.TestCallls.Remove(testCalll);
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
