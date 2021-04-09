using System; using InteriorDesign.Repository;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InteriorDesign.Models;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InteriorDesign.Controllers
{
    [Authorize]
    public class SafetyModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public Boolean isAdminUser()
        {
            var user = db.Users.Where(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Id;
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var s = UserManager.GetRoles(user);
            if (s[0].ToString() == "Admin")
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public SafetyModelsController()
        {
            if (!isAdminUser())
            {
                RedirectToAction("Login", "Account");
            }
        }

        // GET: SafetyModels
        public ActionResult Index()
        {
           
            return View(db.SafetyModels.ToList());
        }

        // GET: SafetyModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SafetyModel safetyModel = db.SafetyModels.Find(id);
            if (safetyModel == null)
            {
                return HttpNotFound();
            }
            return View(safetyModel);
        }

        // GET: SafetyModels/Create
        public ActionResult Create()
        {
           
            return View();
        }

        // POST: SafetyModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AdminLoginIp,Email,MailPassword")] SafetyModel safetyModel)
        {




           

            if (ModelState.IsValid)
            {
               

                // Saving Longing Credential 
                LoginInfoModel logingInfoModel = new LoginInfoModel();
                PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                {
                    return RedirectToAction("Login", "Account");
                }
                logingInfoModel.UsedModel = "SafetyModel";
                logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(safetyModel);
                logingInfoModel.SysDate = DateTime.Now;
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential

                db.SafetyModels.Add(safetyModel);
                db.SaveChanges();


                return RedirectToAction("Index");
            }

            return View(safetyModel);
        }

        // GET: SafetyModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SafetyModel safetyModel = db.SafetyModels.Find(id);
            if (safetyModel == null)
            {
                return HttpNotFound();
            }
            return View(safetyModel);
        }

        // POST: SafetyModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AdminLoginIp,Email,MailPassword")] SafetyModel safetyModel)
        {
           


            if (ModelState.IsValid)
            {
               

                // Saving Longing Credential 
                LoginInfoModel logingInfoModel = new LoginInfoModel();
                PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                {
                    return RedirectToAction("Login", "Account");
                }
                logingInfoModel.UsedModel = "SafetyModel";
                logingInfoModel.TypeOfAction = TypeOfAction.Update;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(safetyModel);
                logingInfoModel.SysDate = DateTime.Now;
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential

                db.Entry(safetyModel).State = EntityState.Modified;
                db.SaveChanges();



                return RedirectToAction("Index");
            }
            return View(safetyModel);
        }

        // GET: SafetyModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SafetyModel safetyModel = db.SafetyModels.Find(id);
            if (safetyModel == null)
            {
                return HttpNotFound();
            }
            return View(safetyModel);
        }

        // POST: SafetyModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

           
            SafetyModel safetyModel = db.SafetyModels.Find(id);

            // Saving Longing Credential 
            LoginInfoModel logingInfoModel = new LoginInfoModel();
            PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
            logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();

            if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
            {
                return RedirectToAction("Login", "Account");
            }
            logingInfoModel.UsedModel = "SafetyModel";
            logingInfoModel.TypeOfAction = TypeOfAction.Delete;
            logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(safetyModel);
            logingInfoModel.SysDate = DateTime.Now;
            db.LoginInfoModels.Add(logingInfoModel);
            db.SaveChanges();
            // Saving Longing Credential



            db.SafetyModels.Remove(safetyModel);
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
