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
using System.Web.Security;
using Newtonsoft.Json;


namespace InteriorDesign.Controllers
{
    public class DepartmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

       


        // GET: Departments
        public ActionResult Index()
        {
           MembershipHelp mHelp = new MembershipHelp(); var role =  mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canView = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("Departments") && rm.RoleId == roleId select rm.CanView).FirstOrDefault();

            if (!canView)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(db.Departments.ToList());
        }

        public JsonResult GetDepartment(string DepartmentName)
        {
            List<string> DepartmentNameList = db.Departments.Where(x => x.DepartmentName.StartsWith(DepartmentName)).Select(c => c.DepartmentName).Distinct().ToList();

            return Json(DepartmentNameList, JsonRequestBehavior.AllowGet);
        }


        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DepartmentName")] Department department)
        {
           MembershipHelp mHelp = new MembershipHelp(); var role =  mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canCreate = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("Departments") && rm.RoleId == roleId select rm.CanCreate).FirstOrDefault();

            if (!canCreate)
            {
                return RedirectToAction("Index");
            }

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
                logingInfoModel.UsedModel = "Department";
                logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(department);
                logingInfoModel.SysDate = DateTime.Now;

                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential 

                db.Departments.Add(department);
                db.SaveChanges();
               
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DepartmentName")] Department department)
        {
           MembershipHelp mHelp = new MembershipHelp(); var role =  mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canEdit = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("Departments") && rm.RoleId == roleId select rm.CanEdit).FirstOrDefault();

            if (!canEdit)
            {
                return RedirectToAction("Index");
            }

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
                logingInfoModel.UsedModel = "Department";
                logingInfoModel.TypeOfAction = TypeOfAction.Update;
                logingInfoModel.SysDate = DateTime.Now;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(department);
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential 


                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();


               


                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {

           MembershipHelp mHelp = new MembershipHelp(); var role =  mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canDelete = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("Departments") && rm.RoleId == roleId select rm.CanDelete).FirstOrDefault();
            if (!canDelete)
            {
                return RedirectToAction("Index");
            }


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);

            // Saving Longing Credential 
            LoginInfoModel logingInfoModel = new LoginInfoModel();
            PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
            logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
            if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
            {
                return RedirectToAction("Login", "Account");
            }
            logingInfoModel.UsedModel = "Department";
            logingInfoModel.TypeOfAction = TypeOfAction.Delete;
            logingInfoModel.SysDate = DateTime.Now;
            logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(department);
            db.LoginInfoModels.Add(logingInfoModel);
            db.SaveChanges();
            // Saving Longing Credential 


            db.Departments.Remove(department);
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
