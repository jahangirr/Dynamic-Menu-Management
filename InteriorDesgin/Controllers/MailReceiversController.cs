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
    public class MailReceiversController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MailReceivers
        public ActionResult Index()
        {
            return View(db.MailReceivers.ToList());
        }

        // GET: MailReceivers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailReceiver mailReceiver = db.MailReceivers.Find(id);
            if (mailReceiver == null)
            {
                return HttpNotFound();
            }
            return View(mailReceiver);
        }

        // GET: MailReceivers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MailReceivers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email")] MailReceiver mailReceiver)
        {
            if (ModelState.IsValid)
            {
                db.MailReceivers.Add(mailReceiver);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mailReceiver);
        }

        // GET: MailReceivers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailReceiver mailReceiver = db.MailReceivers.Find(id);
            if (mailReceiver == null)
            {
                return HttpNotFound();
            }
            return View(mailReceiver);
        }

        // POST: MailReceivers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email")] MailReceiver mailReceiver)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mailReceiver).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mailReceiver);
        }

        // GET: MailReceivers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailReceiver mailReceiver = db.MailReceivers.Find(id);
            if (mailReceiver == null)
            {
                return HttpNotFound();
            }
            return View(mailReceiver);
        }

        // POST: MailReceivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MailReceiver mailReceiver = db.MailReceivers.Find(id);
            db.MailReceivers.Remove(mailReceiver);
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
