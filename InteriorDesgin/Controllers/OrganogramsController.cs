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
using PagedList;
using System.Web.Security;
using System.IO;
using HiQPdf;
using System.Web.Configuration;
using CrystalDecisions.CrystalReports.Engine;

namespace InteriorDesign.Controllers
{
    public class OrganogramsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Organograms
        public ViewResult Index(string selectionItems, string Depart, string Role, string UpperRole, int? page)
        {

            if (Depart != null && Role != null && UpperRole != null)
            {
                page = 1;
            }

            var organogram = (from i in db.Organograms select i);

            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canView = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("Organograms") && rm.RoleId == roleId select rm.CanView).FirstOrDefault();

            if (!canView)
            {
                organogram = organogram.Where(w => w.Id == 0);
            }



            ViewBag.Depart = Depart;
            ViewBag.Role = Role;
            ViewBag.UpperRole = UpperRole;


            if (string.IsNullOrEmpty(Depart) == false)
            {
                organogram = organogram.Where(w => w.Depart.StartsWith(Depart));
            }
            if (string.IsNullOrEmpty(Role) == false)
            {
                organogram = organogram.Where(w => w.Role.StartsWith(Role));
            }
            if (string.IsNullOrEmpty(UpperRole) == false)
            {
                organogram = organogram.Where(w => w.UpperRole.StartsWith(UpperRole));
            }



            List<SelectListItem> selectionItemsList = new List<SelectListItem>();
            selectionItemsList.Add(new SelectListItem() { Text = "--Select-- ", Value = "100" });
            selectionItemsList.Add(new SelectListItem() { Text = "Department ", Value = "0" });
            selectionItemsList.Add(new SelectListItem() { Text = "Role", Value = "1" });
            selectionItemsList.Add(new SelectListItem() { Text = "Upper Role", Value = "2" });

            ViewBag.selectionItems = new SelectList(selectionItemsList, "Value", "Text");

          

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(organogram.OrderBy(o => o.Depart).ToPagedList(pageNumber, pageSize));

        }



       public ActionResult getOrganisationStructure()
       {


            return View();

        }

        public ActionResult getOtherImage()
        {


            return View();

        }


        


        public void OrganogramRefresh(List<Organogram> organogramList, string role)
        {

            List<Organogram> organoList = db.Organograms.Where(o => o.UpperRole == role && o.UpperRole != o.Role).ToList();
            if (organoList.Count() > 0)
            {

                foreach (var ogl in organoList)
                {
                    Organogram orgNewSort = new Organogram();
                    orgNewSort.Depart = ogl.Depart;
                    orgNewSort.Role = ogl.Role;
                    orgNewSort.UpperRole = ogl.UpperRole;
                    organogramList.Add(orgNewSort);
                    OrganogramRefresh(organogramList, ogl.Role);
                   
                }


            }



        }



        public ActionResult OrganoRefresh()
        {
            List<Organogram> organogramList = new List<Organogram>();

            var tempOrgTop = db.Organograms.Where(w =>  w.Role == w.UpperRole).FirstOrDefault();

            Organogram orgNewSort = new Organogram();
            orgNewSort.Depart = tempOrgTop.Depart;
            orgNewSort.Role = tempOrgTop.Role;
            orgNewSort.UpperRole = tempOrgTop.UpperRole;
            organogramList.Add(orgNewSort);
            OrganogramRefresh(organogramList, orgNewSort.Role);

            using (var context = new ApplicationDbContext())
            {

                using (var dbContextTransaction = context.Database.BeginTransaction())
                {

                    try
                    {
                        var orgaListDel = context.Organograms.ToList();
                        foreach(var old in orgaListDel)
                        {
                            context.Organograms.Remove(old);
                        }
                        
                        context.Organograms.AddRange(organogramList);
                        context.SaveChanges();
                        dbContextTransaction.Commit();

                        //// context.SaveChanges();
                        //// Saving Longing Credential 
                        //LoginInfoModel logingInfoModel = new LoginInfoModel();
                        //PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                        //logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                        //if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                        //{
                        //    return RedirectToAction("Login", "Account");
                        //}
                        //logingInfoModel.UsedModel = "LCModel";
                        //logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                        //logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(lcModelInsert);
                        //context.LoginInfoModels.Add(logingInfoModel);
                        //context.SaveChanges();
                        // Saving Longing Credential 

                        //dbContextTransaction.Commit();


                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                    }

                }
            }




            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult GetOrgChartData()
        {
            List<object> chartData = new List<object>();

            List<Organogram> organogramList = new List<Organogram>();

            var tempOrganoList = db.Organograms.ToList();

            foreach(var orga in tempOrganoList.OrderBy(o => o.Id))
            {
                var tempOrgTop = db.Organograms.Where(w => w.UpperRole == orga.Role && w.Role == w.UpperRole).FirstOrDefault();
                if(tempOrgTop != null)
                {
                    chartData.Add(new object[]
                    {
                        tempOrgTop.Id.ToString(),  tempOrgTop.Role , ""
                    });
                }
                else
                {
                    var tempOrg = db.Organograms.Where(w => w.Role == orga.UpperRole).FirstOrDefault();
                    chartData.Add(new object[]
                    {
                         orga.Id.ToString(),  orga.Role , tempOrg.Id.ToString()
                    });

                }
               
            }


            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrganizationStruct(string veryTop )
        {

            string organizationStructure = getOrganizationStucture(veryTop);


            var orgStructure = new { orgStructure = organizationStructure };
            return Json(orgStructure, JsonRequestBehavior.AllowGet);
        }


        string structOrgLoop = "";
        public string getOrganizationStucture(string veryTop)
        {

            string structOrgFirst = "<ul id='ul-data'><li> <a href='#'>" + veryTop + "</a>";
            structOrgLoop = "";
            string structOrgLast = "</li></ul>";


            var totalOrganogram = db.Organograms.ToList();
            var or = db.Organograms.Where(w => w.Role == veryTop).FirstOrDefault();
            var role = "";
            if (or != null)
            {
                role = or.Role;
            }
            List<Organogram> organoListConstant = db.Organograms.Where(o => o.UpperRole == role && o.UpperRole != o.Role).ToList();
            recursiveOrganogram(  organoListConstant  , totalOrganogram, role);
            string tempStructureLoop = structOrgFirst + structOrgLoop + structOrgLast;

            return tempStructureLoop;

        }


        public void recursiveOrganogram(List<Organogram> OrganogramConstant, List<Organogram> OrganogramL, string role)
        {

            List<Organogram> organoList = OrganogramL.Where(o => o.UpperRole == role && o.UpperRole != o.Role).ToList();
            if (organoList.Count() > 0)
            {

                foreach (var ogl in organoList)
                {

                    var isNext = db.Organograms.Where(o => o.UpperRole == ogl.Role //&& o.UpperRole != ogl.Role
                    ).ToList();

                    if (isNext.Count() > 0)
                    {
                        structOrgLoop = structOrgLoop + "<li><a href ='#'>" + ogl.Role + "</a><ul>";
                    }
                    else
                    {
                        structOrgLoop = structOrgLoop + "<li><a href ='#'>" + ogl.Role+ "</a>";
                    }

                    if (isNext.Count() > 0)
                    {
                        recursiveOrganogram(OrganogramConstant,OrganogramL, ogl.Role);
                    }

                    if (isNext.Count() > 0)
                    {
                       var tempOrgCheck = OrganogramConstant.Where(w => w.Role == ogl.Role).FirstOrDefault();

                        if(tempOrgCheck != null)
                        {
                            structOrgLoop = structOrgLoop + "</li></ul></li>";
                        }else
                        {
                            structOrgLoop = structOrgLoop + "</li></ul>";
                        }
                        
                    }

                   
                   


                }


            }



        }

        // GET: Organograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organogram organogram = db.Organograms.Find(id);
            if (organogram == null)
            {
                return HttpNotFound();
            }
            return View(organogram);
        }


        public JsonResult GetDepartment(string DepartmentName)
        {
            List<string> DepartmentList = db.Departments.Where(x => x.DepartmentName.StartsWith(DepartmentName)).Select(c => c.DepartmentName).Distinct().ToList();

            return Json(DepartmentList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRole(string Role)
        {
            List<string> RoleList = db.RoleMasters.Where(x => x.Name.StartsWith(Role)).Select(c => c.Name).Distinct().ToList();

            return Json(RoleList, JsonRequestBehavior.AllowGet);
        }

        // GET: Organograms/Create
        public ActionResult Create()
        {
            return View();
        }


      

        // POST: Organograms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Depart,Role,UpperRole")] Organogram organogram)
        {

            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canCreate = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("Organograms") && rm.RoleId == roleId select rm.CanCreate).FirstOrDefault();


            var upperNotExisted = db.Organograms.Where(w => w.Role == organogram.UpperRole).FirstOrDefault();


            if (!canCreate || upperNotExisted == null || db.Organograms.Where(w => w.Role == organogram.Role).Count()> 0)
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
                logingInfoModel.UsedModel = "Organogram";
                logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(organogram);
                logingInfoModel.SysDate = DateTime.Now;
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential 


                db.Organograms.Add(organogram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(organogram);
        }

        // GET: Organograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (db.Organograms.Min(o => o.Id) == id)
            {
                return RedirectToAction("Index");
            }
            Organogram organogram = db.Organograms.Find(id);
            if (organogram == null)
            {
                return HttpNotFound();
            }
            return View(organogram);
        }

        // POST: Organograms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Depart,Role,UpperRole")] Organogram organogram)
        {
            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canEdit = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("Organograms") && rm.RoleId == roleId select rm.CanEdit).FirstOrDefault();

            if (!canEdit || db.Organograms.Min(o => o.Id) == organogram.Id || db.Organograms.Where(w => w.Id != organogram.Id && w.Role == organogram.Role).Count()> 0)
            {
                return RedirectToAction("Index");
            }
            

            if (ModelState.IsValid)
            {

                using (var context = new ApplicationDbContext())
                {

                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {

                        try
                        {


                            var organogramToUpdate = context.Organograms.Where(w => w.Id == organogram.Id).FirstOrDefault();
                            organogramToUpdate.Depart = organogram.Depart;
                            organogramToUpdate.Role = organogram.Role;
                            organogramToUpdate.UpperRole = organogram.UpperRole;

                            List<LoginInfoModel>  logingInfoModelList  = new  List< LoginInfoModel>();
                            // Saving Longing Credential 
                            LoginInfoModel logingInfoModel = new LoginInfoModel();
                            PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                            logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                            if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                            {
                                return RedirectToAction("Login", "Account");
                            }

                            logingInfoModel.UsedModel = "Organogram";
                            logingInfoModel.TypeOfAction = TypeOfAction.Update;
                            logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(organogramToUpdate);
                            logingInfoModel.SysDate = DateTime.Now;
                            logingInfoModelList.Add(logingInfoModel);


                            string PreviousRole = context.Organograms.Where(w => w.Id == organogramToUpdate.Id).FirstOrDefault().Role;

                            foreach (var organogramUpdate in context.Organograms.Where(w => w.UpperRole == PreviousRole).ToList())
                            {
                                organogramUpdate.UpperRole = organogram.Role;

                                LoginInfoModel logingInfoModelOne = new LoginInfoModel();
                                PopulateLoginInfoCredencial populateLoginInfoCredencialOne = new PopulateLoginInfoCredencial(logingInfoModelOne);
                                logingInfoModelOne = populateLoginInfoCredencialOne.getLoginInfoCredencial();
                                if (!populateLoginInfoCredencialOne.ValidateIPv4(logingInfoModelOne.LoginIp))
                                {
                                    return RedirectToAction("Login", "Account");
                                }

                                logingInfoModelOne.UsedModel = "Organogram";
                                logingInfoModelOne.TypeOfAction = TypeOfAction.Update;
                                logingInfoModelOne.Data = Newtonsoft.Json.JsonConvert.SerializeObject(organogramUpdate);
                                logingInfoModelOne.SysDate = DateTime.Now;
                                logingInfoModelList.Add(logingInfoModelOne);
                            }
                            context.LoginInfoModels.AddRange(logingInfoModelList);
                            context.Entry(organogramToUpdate).State = EntityState.Modified;
                            context.SaveChanges();
                            dbContextTransaction.Commit();

                            return RedirectToAction("Index");






                        }
                        catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                        {
                            dbContextTransaction.Rollback();

                        }
                        catch (System.Data.Entity.Core.EntityCommandCompilationException ex)
                        {
                            dbContextTransaction.Rollback();

                        }
                        catch (System.Data.Entity.Core.UpdateException ex)
                        {
                            dbContextTransaction.Rollback();

                        }

                        catch (System.Data.Entity.Infrastructure.DbUpdateException ex) //DbContext
                        {
                            dbContextTransaction.Rollback();

                        }

                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();


                        }
                    }
                }




               
            }
            return View(organogram);
        }

        // GET: Organograms/Delete/5
        public ActionResult Delete(int? id)
        {
           

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (db.Organograms.Min(o => o.Id) == id)
            {
                return RedirectToAction("Index");
            }
            Organogram organogram = db.Organograms.Find(id);
            if (organogram == null)
            {
                return HttpNotFound();
            }
            return View(organogram);
        }

        // POST: Organograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canDelete = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("Organograms") && rm.RoleId == roleId select rm.CanDelete).FirstOrDefault();

            if (!canDelete || db.Organograms.Min(o => o.Id) == id)
            {
                return RedirectToAction("Index");
            }

            using (var context = new ApplicationDbContext())
            {

                using (var dbContextTransaction = context.Database.BeginTransaction())
                {

                    try
                    {
                        Organogram organogram = context.Organograms.Find(id);

                        List<LoginInfoModel> logingInfoModelList = new List<LoginInfoModel>();
                        // Saving Longing Credential 
                        LoginInfoModel logingInfoModel = new LoginInfoModel();
                        PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                        logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                        if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                        {
                            return RedirectToAction("Login", "Account");
                        }

                        logingInfoModel.UsedModel = "Organogram";
                        logingInfoModel.TypeOfAction = TypeOfAction.Delete;
                        logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(organogram);
                        logingInfoModel.SysDate = DateTime.Now;
                        logingInfoModelList.Add(logingInfoModel);

                        var DeletedRoleEntity = context.Organograms.Where(w => w.Id == organogram.Id).FirstOrDefault();
                        var Role = DeletedRoleEntity.Role;
                        var UpperRole = DeletedRoleEntity.UpperRole;

                        foreach (var organogramUpdate in context.Organograms.Where(w => w.UpperRole == Role).ToList())
                        {
                            organogramUpdate.UpperRole = UpperRole;
                            LoginInfoModel logingInfoModelOne = new LoginInfoModel();
                            PopulateLoginInfoCredencial populateLoginInfoCredencialOne = new PopulateLoginInfoCredencial(logingInfoModelOne);
                            logingInfoModelOne = populateLoginInfoCredencialOne.getLoginInfoCredencial();
                            if (!populateLoginInfoCredencialOne.ValidateIPv4(logingInfoModelOne.LoginIp))
                            {
                                return RedirectToAction("Login", "Account");
                            }

                            logingInfoModelOne.UsedModel = "Organogram";
                            logingInfoModelOne.TypeOfAction = TypeOfAction.Update;
                            logingInfoModelOne.Data = Newtonsoft.Json.JsonConvert.SerializeObject(organogramUpdate);
                            logingInfoModelOne.SysDate = DateTime.Now;
                            logingInfoModelList.Add(logingInfoModelOne);
                        }
                        context.LoginInfoModels.AddRange(logingInfoModelList);
                        context.Organograms.Remove(organogram);
                        context.SaveChanges();
                        dbContextTransaction.Commit();

                        return RedirectToAction("Index");


                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                    {
                        dbContextTransaction.Rollback();

                    }
                    catch (System.Data.Entity.Core.EntityCommandCompilationException ex)
                    {
                        dbContextTransaction.Rollback();

                    }
                    catch (System.Data.Entity.Core.UpdateException ex)
                    {
                        dbContextTransaction.Rollback();

                    }

                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex) //DbContext
                    {
                        dbContextTransaction.Rollback();

                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();


                    }
                }
            }


            Organogram organogramTemp = db.Organograms.Find(id);
            if (organogramTemp == null)
            {
                return HttpNotFound();
            }
            return View(organogramTemp);


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
