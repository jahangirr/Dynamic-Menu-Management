using System;
using InteriorDesign.Repository;
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
using System.Web.Security;
using PagedList;

namespace InteriorDesign.Controllers
{
    [Authorize]
    public class RoleMastersController : Controller
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

        public RoleMastersController()
        {
            if (!isAdminUser())
            {
                RedirectToAction("Login", "Account");
            }
        }

        // GET: RoleMasters
        public ActionResult Index(int? page, string Name , bool? Active)
        {

            if (Name == null && Active == null )
            {
                page = 1;
            }


            var RoleMasters = (from r in db.RoleMasters select r);


            if (string.IsNullOrEmpty(Name) == false)
            {

                RoleMasters = RoleMasters.Where(w => w.Name.StartsWith(Name));
            }

            bool ActiveVar = false;
            if(Active.HasValue == true)
            {
                ActiveVar = Convert.ToBoolean(Active);
            }
                

            RoleMasters = RoleMasters.Where(w => w.Active == ActiveVar);


            ViewBag.Name = Name;
            ViewBag.Active = ActiveVar;





            var user = db.Users.Where(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Id;
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var s = UserManager.GetRoles(user);



            List<SelectListItem> selectionItems = new List<SelectListItem>();
            selectionItems.Add(new SelectListItem() { Text = "--Select-- ", Value = "100" });
            selectionItems.Add(new SelectListItem() { Text = "Role Name", Value = "0" });
            selectionItems.Add(new SelectListItem() { Text = "Active", Value = "1" });
           
            ViewBag.selectionItems = new SelectList(selectionItems, "Value", "Text");
            int pageSize = 10;
            int pageNumber = (page ?? 1);

          


            if (s[0].ToString() == "Admin")
            {

                return View(RoleMasters.OrderBy(o => o.Name).ToPagedList(pageNumber, pageSize));        

            }else
            {
                return View(RoleMasters.Where(w => w.Name != "Admin").OrderBy(o => o.Name).ToPagedList(pageNumber, pageSize));
               
            }
            
        }

        // GET: RoleMasters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMaster roleMaster = db.RoleMasters.Find(id);
            if (roleMaster == null)
            {
                return HttpNotFound();
            }
            return View(roleMaster);
        }

        // GET: RoleMasters/Create
        public ActionResult Create()
        {
            List<SelectListItem> DepartmentNameList = new List<SelectListItem>();
            foreach (var Depart in db.Departments.ToList())
            {
                DepartmentNameList.Add(new SelectListItem() { Text = Depart.DepartmentName.ToString(), Value = Depart.DepartmentName.ToString() });
            }
            ViewBag.DepartmentNameData = new SelectList(DepartmentNameList.Distinct(), "Value", "Text");
           
            return View();
        }

        // POST: RoleMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Active")] RoleMaster roleMaster, string DepartmentName)
        {
            roleMaster.Name = roleMaster.Name + " " + DepartmentName;

            if(db.RoleMasters.Where(w => w.Name == roleMaster.Name).ToList().Count()>0)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                db.RoleMasters.Add(roleMaster);
                db.SaveChanges();

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = roleMaster.Name;
                roleManager.Create(role);


                // Saving Longing Credential 
                LoginInfoModel logingInfoModel = new LoginInfoModel();
                PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                {
                    return RedirectToAction("Login", "Account");
                }

                logingInfoModel.UsedModel = "RoleMaster";
                logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(role);
                logingInfoModel.SysDate = DateTime.Now;
                
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential

                return RedirectToAction("Index");
            }

            List<SelectListItem> DepartmentNameList = new List<SelectListItem>();
            foreach (var Depart in db.Departments.ToList())
            {
                DepartmentNameList.Add(new SelectListItem() { Text = Depart.DepartmentName.ToString(), Value = Depart.DepartmentName.ToString() });
            }
            ViewBag.DepartmentNameData = new SelectList(DepartmentNameList.Distinct(), "Value", "Text");

            return View(roleMaster);
        }

        // GET: RoleMasters/Edit/5
        public ActionResult Edit(int? id)
        {

            return RedirectToAction("Index");

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMaster roleMaster = db.RoleMasters.Find(id);
            if (roleMaster == null)
            {
                return HttpNotFound();
            }
            return View(roleMaster);
        }

        // POST: RoleMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Active")] RoleMaster roleMaster)
        {



            return RedirectToAction("Index");

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

                logingInfoModel.UsedModel = "RoleMaster";
                logingInfoModel.TypeOfAction = TypeOfAction.Update;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(roleMaster);
                logingInfoModel.SysDate = DateTime.Now;
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential


                db.Entry(roleMaster).State = EntityState.Modified;
                db.SaveChanges();




                return RedirectToAction("Index");
            }
            return View(roleMaster);
        }

        // GET: RoleMasters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMaster roleMaster = db.RoleMasters.Find(id);
            if (roleMaster == null)
            {
                return HttpNotFound();
            }
            return View(roleMaster);
        }

        // POST: RoleMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var RoleName = db.RoleMasters.Where(w => w.Id == id).FirstOrDefault().Name;

            var RoleExistedInUser = db.RoleMasters.Where(w => w.Name == RoleName).FirstOrDefault();
            if (RoleName == "Admin" || RoleExistedInUser != null)
            {
                return RedirectToAction("Index");
            }

            RoleMaster roleMasterDel = db.RoleMasters.Find(id);

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            role = roleManager.FindByName(roleMasterDel.Name);
            var roleResult = roleManager.DeleteAsync(role);

            
            using (var db = new ApplicationDbContext())
            {

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {

                    try
                    {

                        RoleMaster roleMaster = db.RoleMasters.Find(id);

                        // Saving Longing Credential 
                        LoginInfoModel logingInfoModel = new LoginInfoModel();
                        PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                        logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                        if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        logingInfoModel.UsedModel = "RoleMaster";
                        logingInfoModel.TypeOfAction = TypeOfAction.Delete;
                        logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(roleMaster);
                        logingInfoModel.SysDate = DateTime.Now;
                        db.LoginInfoModels.Add(logingInfoModel);         
                        db.RoleMasters.Remove(roleMaster);

                        if (roleResult.Result.Errors.Count() == 0)
                        {
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }

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
