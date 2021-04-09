using System; using InteriorDesign.Repository;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InteriorDesign.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InteriorDesign.Controllers
{
    [Authorize]
    public class MenuInfoesController : Controller
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

        public MenuInfoesController()
        {
            if (!isAdminUser())
            {
                RedirectToAction("Login", "Account");
            }
        }

        // GET: MenuInfoes
        public ActionResult Index()
        {
          
            ViewBag.MenuInfoes = new SelectList(db.MenuInfoes, "Id", "MenuName");

            return View(db.MenuInfoes.ToList());
        }

        // GET: MenuInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuInfo menuInfo = db.MenuInfoes.Find(id);
            if (menuInfo == null)
            {
                return HttpNotFound();
            }
            return View(menuInfo);
        }

        // GET: MenuInfoes/Create
        public ActionResult Create()
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Login","Home");
            }

            ViewBag.MenuParentID = new SelectList(db.MenuInfoes.Select(x => new SelectListItem() { Text = x.MenuName, Value = x.Id.ToString() }).ToList(), "Value", "Text");
            return View();
        }

        // POST: MenuInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MenuName,MenuURL,MenuParentID,MenuParentIDText,CanView,CanCreate,CanEdit,CanDelete,Active")] MenuInfo menuInfo)
        {
           
            if (ModelState.IsValid)
            {
                var MenuParent = db.MenuInfoes.Where(w => w.Id == menuInfo.MenuParentID).FirstOrDefault();
                menuInfo.MenuParentIDText = MenuParent.MenuName;

                if (!User.IsInRole("Admin"))
                {
                    return RedirectToAction("Login", "Home");
                }


                // Saving Longing Credential 
                LoginInfoModel logingInfoModel = new LoginInfoModel();
                PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                {
                    return RedirectToAction("Login", "Account");
                }
                logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                logingInfoModel.UsedModel = "MenuInfo";
                logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(menuInfo);
                logingInfoModel.SysDate = DateTime.Now;
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential 

                db.MenuInfoes.Add(menuInfo);
                db.SaveChanges();

              


                return RedirectToAction("Index");
            }

            return View(menuInfo);
        }

        // GET: MenuInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }




          



            MenuInfo menuInfo = db.MenuInfoes.Find(id);
            ViewBag.MenuParentID = new SelectList(db.MenuInfoes.Select(x => new SelectListItem() { Text = x.MenuName, Value = x.Id.ToString() }).ToList(), "Value", "Text",menuInfo.MenuParentID);


            if (menuInfo == null)
            {
                return HttpNotFound();
            }
            return View(menuInfo);
        }

        // POST: MenuInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MenuName,MenuURL,MenuParentID,MenuParentIDText,CanView,CanCreate,CanEdit,CanDelete,Active")] MenuInfo menuInfo)
        {
            if (ModelState.IsValid)
            {
                if (!User.IsInRole("Admin"))
                {
                    return RedirectToAction("Login", "Home");
                }

                var MenuParent = db.MenuInfoes.Where(w => w.Id == menuInfo.MenuParentID).FirstOrDefault();
                menuInfo.MenuParentIDText = MenuParent.MenuName;


                // Saving Longing Credential 
                LoginInfoModel logingInfoModel = new LoginInfoModel();
                PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                {
                    return RedirectToAction("Login", "Account");
                }
                logingInfoModel.UsedModel = "MenuInfo";
                logingInfoModel.TypeOfAction = TypeOfAction.Update;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(menuInfo);
                logingInfoModel.SysDate = DateTime.Now;
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential 

                db.Entry(menuInfo).State = EntityState.Modified;
                db.SaveChanges();

                

                return RedirectToAction("Index");
            }
            return View(menuInfo);
        }

        // GET: MenuInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuInfo menuInfo = db.MenuInfoes.Find(id);
            if (menuInfo == null)
            {
                return HttpNotFound();
            }
            return View(menuInfo);
        }

        // POST: MenuInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Home");
            }

            MenuInfo menuInfo = db.MenuInfoes.Find(id);

            // Saving Longing Credential 
            LoginInfoModel logingInfoModel = new LoginInfoModel();
            PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
            logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
            if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
            {
                return RedirectToAction("Login", "Account");
            }
            logingInfoModel.UsedModel = "MenuInfo";
            logingInfoModel.TypeOfAction = TypeOfAction.Delete;
            logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(menuInfo);
            logingInfoModel.SysDate = DateTime.Now;
            db.LoginInfoModels.Add(logingInfoModel);
            db.SaveChanges();
            // Saving Longing Credential 


            db.MenuInfoes.Remove(menuInfo);
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
