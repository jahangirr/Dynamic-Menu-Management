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
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using PagedList;
using System.Threading.Tasks;

namespace InteriorDesign.Controllers
{
    [Authorize]
    public class UserMastersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserMasters
        public ActionResult Index(int? page, string RoleName, string Name, bool? Active , string UserCell)
        {


            if (RoleName == null && Name == null && Active == null && UserCell == null)
            {
                page = 1;
            }


            var userMasters = db.UserMasters.Include(u => u.RoleMaster);

            var user = db.Users.Where(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Id;
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var s = UserManager.GetRoles(user);
            if (s[0].ToString() != "Admin")
            {
                userMasters = userMasters.Where(w => w.RoleMaster.Name != "Admin");
            }


            var roleEnity = userMasters.Where(w => w.Name == User.Identity.Name).FirstOrDefault();
            var roleName = db.RoleMasters.Where(w => w.Id == roleEnity.RoleId).FirstOrDefault();
            var canView = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("UserMasters") && rm.RoleId == roleEnity.RoleId select rm.CanView).FirstOrDefault();

            if (!canView)
            {
                userMasters = userMasters.Where(w => w.Id == 0);
            }


            if (string.IsNullOrEmpty(RoleName) == false)
            {
                userMasters = userMasters.Where(w => w.RoleMaster.Name.Contains(RoleName));
            }

            if (string.IsNullOrEmpty(Name) == false)
            {
                userMasters = userMasters.Where(w => w.Name.Contains(Name));
            }


            if (string.IsNullOrEmpty(UserCell) == false)
            {
                userMasters = userMasters.Where(w => w.UserCell.Contains(UserCell));
            }

            bool ActiveVar = false;

            if (Active != null)
            {
                ActiveVar = Convert.ToBoolean(Active);
            }

            userMasters = userMasters.Where(w => w.Active == ActiveVar);

            ViewBag.RoleName = RoleName;
            ViewBag.Name = Name;
            ViewBag.UserCell = UserCell;
            ViewBag.Active = ActiveVar;


            List<SelectListItem> selectionItems = new List<SelectListItem>();
            selectionItems.Add(new SelectListItem() { Text = "--Select-- ", Value = "100" });
            selectionItems.Add(new SelectListItem() { Text = "Role Name", Value = "0" });
            selectionItems.Add(new SelectListItem() { Text = "Name", Value = "1" });
            selectionItems.Add(new SelectListItem() { Text = "Active", Value = "2" });
            selectionItems.Add(new SelectListItem() { Text = "Cell", Value = "3" });

            ViewBag.selectionItems = new SelectList(selectionItems, "Value", "Text");


            List<SelectListItem> ShowUserIdList = new List<SelectListItem>();
            foreach(var userName in db.UserMasters.Where(w => w.Name != "sLine" ).Select(n => n.Name).ToList())
            {
                ShowUserIdList.Add(new SelectListItem() { Text = userName, Value = userName });           
            }
            ViewBag.ShowUserId = new SelectList(ShowUserIdList, "Value", "Text");



            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(userMasters.OrderBy(o => o.Name).ToPagedList(pageNumber, pageSize));


        }

        // GET: UserMasters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserMaster userMaster = db.UserMasters.Find(id);
            if (userMaster == null)
            {
                return HttpNotFound();
            }
            return View(userMaster);
        }

        // GET: UserMasters/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.RoleMasters, "Id", "Name");
            return View();
        }


        public JsonResult GetShowUserId(string ShowUserId)
        {
            UserMaster userMaster = db.UserMasters.Where(w => w.Name == ShowUserId).FirstOrDefault();

            ApplicationUser appUser = db.Users.Where(w => w.UserName == ShowUserId).FirstOrDefault();
            var TempRoleName = db.RoleMasters.Where(w => w.Id == userMaster.RoleId).FirstOrDefault().Name;
            var userUpdateInfo = new { RoleName = TempRoleName, Email = appUser.Email, Active = userMaster.Active , UserCell = userMaster.UserCell};
            return Json(userUpdateInfo, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserUpdate(string ShowUserId, string ShowEmail , bool ShowActive , bool ShowPasswordReset , string UserCell)
        {
            
            if(string.IsNullOrEmpty(ShowEmail))
            {
                foreach (char singleChar in ShowUserId)
                {
                   if(Char.IsLetter(singleChar))
                    {
                        ShowEmail += singleChar.ToString();
                    }
                }

                Random r = new Random();

                ShowEmail = ShowEmail + "_" + r.Next(10000, 100000).ToString() + "@silverglobalfashion.com";

            }


            string newPassword = "";
            if(ShowPasswordReset)
            {
                newPassword = "ALU7xYCfV1Qo56fEiuax1lj3Nbpym/gkxLX5Z18hzjEsk69BWw8YgPgXX2eufdbu2A==";
            }


            using (var db = new ApplicationDbContext())
            {

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {

                    try
                    {

                        UserMaster userMaster = db.UserMasters.Where(w => w.Name == ShowUserId).FirstOrDefault();
                        ApplicationUser appUser =   db.Users.Where(w => w.UserName == ShowUserId).FirstOrDefault();

                        if (!string.IsNullOrEmpty(ShowEmail))
                        {
                            appUser.Email = ShowEmail;
                            
                        }

                        if (!string.IsNullOrEmpty(UserCell))
                        {
                            userMaster.UserCell = UserCell;

                        }

                        if (ShowPasswordReset)
                        {
                            appUser.PasswordHash = newPassword;
                        }

                        userMaster.Active = ShowActive;


                        UserMaster logUserMaster = new UserMaster();
                        logUserMaster.Active = userMaster.Active;
                        logUserMaster.Id = userMaster.Id;
                        logUserMaster.Name = userMaster.Name;
                        logUserMaster.RoleId = userMaster.RoleId;
                        logUserMaster.UserId = userMaster.UserId;
                        logUserMaster.UserCell = userMaster.UserCell;







                        // Saving Longing Credential 
                        LoginInfoModel logingInfoModel = new LoginInfoModel();
                        PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                        logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                        if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        logingInfoModel.UsedModel = "UserMaster";
                        logingInfoModel.TypeOfAction = TypeOfAction.Update;
                        logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(logUserMaster);
                        logingInfoModel.SysDate = DateTime.Now;
                        db.LoginInfoModels.Add(logingInfoModel);


                        LoginInfoModel logingInfoModelOne = new LoginInfoModel();
                        PopulateLoginInfoCredencial populateLoginInfoCredencialOne = new PopulateLoginInfoCredencial(logingInfoModelOne);
                        logingInfoModelOne = populateLoginInfoCredencialOne.getLoginInfoCredencial();
                        if (!populateLoginInfoCredencialOne.ValidateIPv4(logingInfoModelOne.LoginIp))
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        logingInfoModelOne.UsedModel = "ApplicationUser";
                        logingInfoModelOne.TypeOfAction = TypeOfAction.Update;
                        logingInfoModelOne.Data = Newtonsoft.Json.JsonConvert.SerializeObject(appUser);
                        logingInfoModelOne.SysDate = DateTime.Now;
                        db.LoginInfoModels.Add(logingInfoModelOne);

                        db.Entry(userMaster).State = EntityState.Modified;
                        db.Entry(appUser).State = EntityState.Modified;

                        db.SaveChanges();
                        dbContextTransaction.Commit();





                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                    }

                }
            }




            return RedirectToAction("Index");
        }

        // POST: UserMasters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Name,RoleId,Active,UserCell")] UserMaster userMaster)
        {
            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canEdit = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("UserMasters") && rm.RoleId == roleId select rm.CanEdit).FirstOrDefault();

            if (!canEdit)
            {
                return RedirectToAction("Index");
            }



            // to ensure same role single user
            var roleAlreadyIn = db.UserMasters.Where(w => w.RoleId == userMaster.RoleId).FirstOrDefault();

            if(roleAlreadyIn != null)
            {
                return RedirectToAction("Index");
            }

            // to ensure same role single user

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
                logingInfoModel.UsedModel = "UserMaster";
                logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                logingInfoModel.SysDate = DateTime.Now;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(userMaster);
                
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential


                db.UserMasters.Add(userMaster);
                db.SaveChanges();



                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.RoleMasters, "Id", "Name", userMaster.RoleId);
            return View(userMaster);
        }

        // GET: UserMasters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserMaster userMaster = db.UserMasters.Find(id);
            if (userMaster == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.RoleMasters, "Id", "Name", userMaster.RoleId);
            return View(userMaster);
        }

        // POST: UserMasters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Name,RoleId,Active,UserCell")] UserMaster userMaster)
        {

            //MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            //var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            //var canEdit = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("UserMasters") && rm.RoleId == roleId select rm.CanEdit).FirstOrDefault();

            //if (!canEdit)
            //{
            //    return RedirectToAction("Index");
            //}

            //if (ModelState.IsValid)
            //{


            //    // Saving Longing Credential 
            //    LoginInfoModel logingInfoModel = new LoginInfoModel();
            //    PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
            //    if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
            //    {
            //        return RedirectToAction("Login", "Account");
            //    }
            //    logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
            //    logingInfoModel.UsedModel = "UserMaster";
            //    logingInfoModel.TypeOfAction = TypeOfAction.Update;
            //    logingInfoModel.SysDate = DateTime.Now;
            //    logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(userMaster);
            //    db.LoginInfoModels.Add(logingInfoModel);
            //    // db.SaveChanges();
            //    // Saving Longing Credential


            //    db.Entry(userMaster).State = EntityState.Modified;
            //    //  db.SaveChanges();


            //    return RedirectToAction("Index");
            //}
            //ViewBag.RoleId = new SelectList(db.RoleMasters, "Id", "Name", userMaster.RoleId);
            return View(userMaster);
        }

        // GET: UserMasters/Delete/5
        public ActionResult Delete(int? id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserMaster userMaster = db.UserMasters.Find(id);
            if (userMaster == null)
            {
                return HttpNotFound();
            }
            return View(userMaster);
        }

        // POST: UserMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {


            var RoleId = db.UserMasters.Where(w => w.Id == id).FirstOrDefault().RoleId;
            var RoleName = db.RoleMasters.Where(w => w.Id == RoleId).FirstOrDefault().Name;
            if (RoleName == "Admin")
            {
                return RedirectToAction("Index");
            }

            var RoleIdInUse = db.RoleMenuMappings.Where(w => w.RoleId == RoleId).FirstOrDefault();

            if(RoleIdInUse != null)
            {
                return RedirectToAction("Index");
            }

            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var CanDelete = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("UserMasters") && rm.RoleId == roleId select rm.CanDelete).FirstOrDefault();

            if (!CanDelete)
            {
                return RedirectToAction("Index");
            }


            using (var db = new ApplicationDbContext())
            {

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {

                    try
                    {

                        UserMaster userMaster = db.UserMasters.Find(id);

                        // Saving Longing Credential 
                        LoginInfoModel logingInfoModel = new LoginInfoModel();
                        PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                        logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                        if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        logingInfoModel.UsedModel = "UserMaster";
                        logingInfoModel.TypeOfAction = TypeOfAction.Delete;
                        var uMaster = new
                        {
                            Id = userMaster.Id
      ,
                            UserId = userMaster.UserId
      ,
                            UserCell = userMaster.UserCell
      ,
                            Name = userMaster.Name
      ,
                            RoleId = userMaster.RoleId
      ,
                            Active = userMaster.Active
                        };
                        logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(uMaster);
                        logingInfoModel.SysDate = DateTime.Now;
                        db.LoginInfoModels.Add(logingInfoModel);
                        db.UserMasters.Remove(userMaster);
                        var UserNameToDel = userMaster.Name;
                        db.SaveChanges();

                        var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                        ApplicationUser userDel = manager.FindByName(UserNameToDel);
                        manager.DeleteAsync(userDel);
                        Task.Delay(2000).Wait();

                        dbContextTransaction.Commit();

                       



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
