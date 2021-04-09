using System; using InteriorDesign.Repository;
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
    public class ReportSetupsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReportSetups
        public ActionResult Index()
        {
            return View(db.ReportSetups.ToList());
        }

        // GET: ReportSetups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportSetup reportSetup = db.ReportSetups.Find(id);
            if (reportSetup == null)
            {
                return HttpNotFound();
            }
            return View(reportSetup);
        }

        // GET: ReportSetups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReportSetups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CompanyName,CompanyLogoPath,AddressLineOne,AddressLineTwo,AddressLineThree,Telephone,Fax,Email")] ReportSetup reportSetup)
        {
            if (ModelState.IsValid)
            {
                db.ReportSetups.Add(reportSetup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reportSetup);
        }

        // GET: ReportSetups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportSetup reportSetup = db.ReportSetups.Find(id);
            if (reportSetup == null)
            {
                return HttpNotFound();
            }
            return View(reportSetup);
        }

        // POST: ReportSetups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CompanyName,CompanyLogoPath,AddressLineOne,AddressLineTwo,AddressLineThree,Telephone,Fax,Email")] ReportSetup reportSetup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reportSetup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reportSetup);
        }

        // GET: ReportSetups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportSetup reportSetup = db.ReportSetups.Find(id);
            if (reportSetup == null)
            {
                return HttpNotFound();
            }
            return View(reportSetup);
        }

        // POST: ReportSetups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReportSetup reportSetup = db.ReportSetups.Find(id);
            db.ReportSetups.Remove(reportSetup);
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
