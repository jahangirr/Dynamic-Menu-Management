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
    public class RoleMenuMappingsController : Controller
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

        public RoleMenuMappingsController()
        {
            if (!isAdminUser())
            {
                RedirectToAction("Login", "Account");
            }
        }

        // GET: RoleMenuMappings
        public ActionResult Index()
        {

          
            return View(db.RoleMenuMappings.ToList());
        }

        // GET: RoleMenuMappings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMenuMapping roleMenuMapping = db.RoleMenuMappings.Find(id);
            if (roleMenuMapping == null)
            {
                return HttpNotFound();
            }
            return View(roleMenuMapping);
        }

        // GET: RoleMenuMappings/Create

        public ActionResult ReLoadExistingPermission(string RoleName)
        {

            ViewBag.RoleName = new SelectList(db.Roles.Where(w => w.Name != "Admin").ToList(), "Name", "Name");

            if (string.IsNullOrEmpty( RoleName) == true)
            {
                return View();
            }

            if (!string.IsNullOrEmpty(RoleName))
            {

                var ReloadedExisted = (from m in db.MenuInfoes
                                       join l in db.RoleMenuMappings on m.Id equals l.MenuInfoId
                                       where l.RoleIdText == RoleName
                                       select new
                                       {
                                           Id = m.Id
          ,
                                           MenuName = m.MenuName
          ,
                                           MenuURL = m.MenuURL
          ,
                                           MenuParentID = m.MenuParentID
          ,
                                           MenuParentIDText = m.MenuParentIDText
          ,
                                           CanView = l.CanView
          ,
                                           CanCreate = l.CanCreate
          ,
                                           CanEdit = l.CanEdit
          ,
                                           CanDelete = l.CanDelete
          ,
                                           CanClose = l.CanClose
          ,
                                           Active = l.Active

                                       }).Concat(from mo in db.MenuInfoes
                                                 join lo in db.RoleMenuMappings.Where(r => r.RoleIdText == RoleName) on mo.Id equals lo.MenuInfoId into MR
                                                 from missingMenu in MR.DefaultIfEmpty()
                                                 where missingMenu == null
                                                 select new
                                                 {

                                                     Id = mo.Id
          ,
                                                     MenuName = mo.MenuName
          ,
                                                     MenuURL = mo.MenuURL
          ,
                                                     MenuParentID = mo.MenuParentID
          ,
                                                     MenuParentIDText = mo.MenuParentIDText
          ,
                                                     CanView = mo.CanView
          ,
                                                     CanCreate = mo.CanCreate
          ,
                                                     CanEdit = mo.CanEdit
          ,
                                                     CanDelete = mo.CanDelete
          ,
                                                     CanClose = mo.CanClose
          ,
                                                     Active = mo.Active

                                                 });


                List<MenuInfo> DeservemenuInfo = new List<MenuInfo>();

                foreach (var x in ReloadedExisted.ToList())
                {
                    MenuInfo mInfo = new MenuInfo();
                    mInfo.Id = x.Id;
                    mInfo.MenuName = x.MenuName;
                    mInfo.MenuParentID = x.MenuParentID;
                    mInfo.MenuParentIDText = x.MenuParentIDText;
                    mInfo.MenuURL = x.MenuURL;
                    mInfo.CanView = x.CanView;
                    mInfo.CanEdit = x.CanEdit;
                    mInfo.CanDelete = x.CanDelete;
                    mInfo.CanCreate = x.CanCreate;
                    mInfo.CanClose = x.CanClose;
                    mInfo.Active = x.Active;
                    DeservemenuInfo.Add(mInfo);
                }

                Session["ReLoadExistingPermission"] = DeservemenuInfo;

                Session["ReLoadExistingPermissionRole"] = RoleName;
            }
           
            return RedirectToAction("Create");
        }
        public ActionResult Create()
        {
            if (Session["ReLoadExistingPermissionRole"] != null)
            {
                string RoleNamePram = (string)Session["ReLoadExistingPermissionRole"];
                ViewBag.RoleName = new SelectList(db.Roles.Where(w => w.Name == RoleNamePram).ToList(), "Name", "Name", RoleNamePram);
                ViewBag.RoleNamePram = RoleNamePram;
            }

            if (Session["ReLoadExistingPermission"] != null)
            {

                var menuInfoList = (List<MenuInfo>)Session["ReLoadExistingPermission"];
                ViewBag.totalMenu = menuInfoList.Count();
                return View(menuInfoList);

            }

            return RedirectToAction("ReLoadExistingPermission");


        }

        // POST: RoleMenuMappings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( List<MenuInfo> menuList , string RoleName)
        {

            var roleToDel = 0;

            List<RoleMenuMapping> roleMenuMappings = db.RoleMenuMappings.Where(drm => drm.RoleIdText == RoleName).ToList();

            if(roleMenuMappings != null && roleMenuMappings.Count()>0)
            {
                roleToDel = roleMenuMappings[0].RoleId;
            }

            foreach(var item in roleMenuMappings)
            {
                db.RoleMenuMappings.Remove(item);
                db.SaveChanges();
            }
            
            RoleMenuMapping roleMenuMapping = new  RoleMenuMapping();
            var RoleList = db.RoleMasters.ToList();
            var AllMenu = db.MenuInfoes.ToList();

            if (ModelState.IsValid)
            {
                foreach(var item in menuList)
                {
                    var MenoInfoIdText = AllMenu.Where(am => am.Id == item.Id).Select(s => s.MenuParentIDText).FirstOrDefault();
                    if( MenoInfoIdText == null || MenoInfoIdText == "")
                    {
                        MenoInfoIdText = AllMenu.Where(am => am.Id == item.Id).Select(s => s.MenuName).FirstOrDefault();
                    }
                    var RoleId = RoleList.Where(rl => rl.Name == RoleName).Select(r => r.Id).FirstOrDefault();
                    roleMenuMapping.MenuInfoId = item.Id;
                    roleMenuMapping.MenuInfoIdText = MenoInfoIdText;
                    roleMenuMapping.CanView = Convert.ToBoolean( item.CanView);
                    roleMenuMapping.CanCreate = Convert.ToBoolean(item.CanCreate);
                    roleMenuMapping.CanEdit = Convert.ToBoolean(item.CanEdit);
                    roleMenuMapping.CanDelete = Convert.ToBoolean(item.CanDelete);
                    roleMenuMapping.CanClose = Convert.ToBoolean(item.CanClose);
                    roleMenuMapping.Active = item.Active;
                    roleMenuMapping.RoleId = RoleId == 0 ? roleToDel : RoleId;
                    roleMenuMapping.RoleIdText = RoleName;
                    db.RoleMenuMappings.Add(roleMenuMapping);


                    // Saving Longing Credential 
                    LoginInfoModel logingInfoModel = new LoginInfoModel();
                    PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                    logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                    if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    logingInfoModel.UsedModel = "RoleMenuMapping";
                    logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                    logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(roleMenuMapping);
                    logingInfoModel.SysDate = DateTime.Now;
                    db.LoginInfoModels.Add(logingInfoModel);
                    db.SaveChanges();
                    // Saving Longing Credential


                    db.SaveChanges();


                   
                }

                return RedirectToAction("Index");
            }

            return View(roleMenuMapping);
        }

        // GET: RoleMenuMappings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMenuMapping roleMenuMapping = db.RoleMenuMappings.Find(id);
            if (roleMenuMapping == null)
            {
                return HttpNotFound();
            }
            return View(roleMenuMapping);
        }

        // POST: RoleMenuMappings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RoleId,RoleIdText,MenuInfoId,MenuInfoIdText,CanView,CanCreate,CanEdit,CanDelete,Active")] RoleMenuMapping roleMenuMapping)
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
                logingInfoModel.UsedModel = "RoleMenuMapping";
                logingInfoModel.TypeOfAction = TypeOfAction.Update;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(roleMenuMapping);
                logingInfoModel.SysDate = DateTime.Now;
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential

                db.Entry(roleMenuMapping).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(roleMenuMapping);
        }

        // GET: RoleMenuMappings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleMenuMapping roleMenuMapping = db.RoleMenuMappings.Find(id);
            if (roleMenuMapping == null)
            {
                return HttpNotFound();
            }
            return View(roleMenuMapping);
        }

        // POST: RoleMenuMappings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoleMenuMapping roleMenuMapping = db.RoleMenuMappings.Find(id);

            // Saving Longing Credential 
            LoginInfoModel logingInfoModel = new LoginInfoModel();
            PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
            logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
            if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
            {
                return RedirectToAction("Login", "Account");
            }
            logingInfoModel.UsedModel = "RoleMenuMapping";
            logingInfoModel.TypeOfAction = TypeOfAction.Delete;
            logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(roleMenuMapping);
            logingInfoModel.SysDate = DateTime.Now;
            db.LoginInfoModels.Add(logingInfoModel);
            db.SaveChanges();
            // Saving Longing Credential


            db.RoleMenuMappings.Remove(roleMenuMapping);
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
