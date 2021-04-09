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
    public class ActivePercentagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ActivePercentages
        public ActionResult Index()
        {
            return View(db.ActivePercentages.ToList());
        }

        // GET: ActivePercentages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivePercentage activePercentage = db.ActivePercentages.Find(id);
            if (activePercentage == null)
            {
                return HttpNotFound();
            }
            return View(activePercentage);
        }

        // GET: ActivePercentages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ActivePercentages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Percentage")] ActivePercentage activePercentage)
        {
            if (ModelState.IsValid)
            {
                db.ActivePercentages.Add(activePercentage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(activePercentage);
        }

        // GET: ActivePercentages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivePercentage activePercentage = db.ActivePercentages.Find(id);
            if (activePercentage == null)
            {
                return HttpNotFound();
            }
            return View(activePercentage);
        }

        // POST: ActivePercentages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Percentage")] ActivePercentage activePercentage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(activePercentage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(activePercentage);
        }

        // GET: ActivePercentages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivePercentage activePercentage = db.ActivePercentages.Find(id);
            if (activePercentage == null)
            {
                return HttpNotFound();
            }
            return View(activePercentage);
        }

        // POST: ActivePercentages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ActivePercentage activePercentage = db.ActivePercentages.Find(id);
            db.ActivePercentages.Remove(activePercentage);
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
