using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InteriorDesign.Models;
using InteriorDesign.Repository;
using PagedList;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace InteriorDesign.Controllers
{
    public class BankAndBranchesController : Controller
    {

        public ActionResult MatureReport(string ReportType,  string FromReceiveDate, string ToReceiveDate)
        {

            GlobCreatedFunction gcf = new GlobCreatedFunction();

            DataSet ds = new DataSet("MatureReport");

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            SqlCommand cmd = new SqlCommand("usp_ReportHeader", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Id", "1");
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable datatable = new DataTable("usp_ReportHeader");
            da.Fill(datatable);


            SqlConnection con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            SqlCommand cmd1 = new SqlCommand("usp_MatureInfoReport", con1);
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("@FromReceiveDate", gcf.GetDate(FromReceiveDate));
            cmd1.Parameters.AddWithValue("@ToReceiveDate", gcf.GetDate(ToReceiveDate));
            
            SqlDataAdapter da1 = new SqlDataAdapter();
            da1.SelectCommand = cmd1;
            DataTable datatable1 = new DataTable("usp_MatureInfoReport");
            da1.Fill(datatable1);

            if (datatable1.Rows.Count == 0)
            {
                DataRow dr = datatable1.NewRow();
                dr["BankName"] = "Dhaka";
                dr["BranchName"] = "Palton";
                dr["Amount"] = "1250000";
                dr["ReceiveAmount"] = "1000000";
                datatable1.Rows.Add(dr);

            }

            ds.Tables.Add(datatable);
            ds.Tables.Add(datatable1);


            // ds.WriteXml("D:\\Chunu\\InteriorDesign\\InteriorDesign\\CrystalReports\\MatureInfoReport.xsd", XmlWriteMode.WriteSchema);


            string UserId = WebConfigurationManager.AppSettings["UserId"].ToString();
            string Password = WebConfigurationManager.AppSettings["Password"].ToString();
            string ServerName = WebConfigurationManager.AppSettings["Server"].ToString();
            string Database = WebConfigurationManager.AppSettings["Database"].ToString();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/CrystalReports"), "MatureInfoReport.rpt"));


            rd.SetDatabaseLogon(UserId, Password, ServerName, Database);
            rd.SetDataSource(ds);

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            if (ReportType == "PDF")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "MatureInfoReport.pdf");
            }

            if (ReportType == "EXCEL")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/vnd.ms-excel", "MatureInfoReport.xls");
            }

            if (ReportType == "EXCELRECORD")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.ExcelRecord);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/vnd.ms-excel", "MatureInfoReport.xls");
            }

            if (ReportType == "WORD")
            {
                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/msword", "MatureInfoReport.doc");
            }
            return RedirectToAction("Index");



        }

        


        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAndBranches
        public ViewResult Index(string BankName, string BranchName, int? page)
        {




            if (BankName != null && BranchName != null )
            {
                page = 1;
            }

            

            var BankAndBranches = (from i in db.BankAndBranches select i);



            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canView = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("BankAndBranches") && rm.RoleId == roleId select rm.CanView).FirstOrDefault();

            if (!canView)
            {
                BankAndBranches = BankAndBranches.Where(w => w.Id == 0);
            }




            if (string.IsNullOrEmpty(BankName) == false)
            {

                BankAndBranches = BankAndBranches.Where(w => w.BankName.Contains(BankName));
            }

            if (string.IsNullOrEmpty(BranchName) == false)
            {

                BankAndBranches = BankAndBranches.Where(w => w.BranchName.Contains(BranchName));
            }

            ViewBag.BankName = BankName;
            ViewBag.BranchName = BranchName;
           

            List<SelectListItem> selectionItems = new List<SelectListItem>();
            selectionItems.Add(new SelectListItem() { Text = "--Select-- ", Value = "100" });
            selectionItems.Add(new SelectListItem() { Text = "Bank Name", Value = "0" });
            selectionItems.Add(new SelectListItem() { Text = "Branch Name", Value = "1" });
            ViewBag.selectionItems = new SelectList(selectionItems, "Value", "Text");

            BankAndBranches = BankAndBranches.Take(100);

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(BankAndBranches.OrderByDescending(o => o.Id).ToPagedList(pageNumber, pageSize));

        }

        // GET: BankAndBranches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAndBranch bankAndBranch = db.BankAndBranches.Find(id);
            if (bankAndBranch == null)
            {
                return HttpNotFound();
            }
            return View(bankAndBranch);
        }

        // GET: BankAndBranches/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BankAndBranches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,BankName,BranchName")] BankAndBranch bankAndBranch)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ApplicationDbContext())
                {

                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {

                        try
                        {
                            context.BankAndBranches.Add(bankAndBranch);
                            // context.SaveChanges();
                            // Saving Longing Credential 
                            LoginInfoModel logingInfoModel = new LoginInfoModel();
                            PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                            logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                            if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                            {
                                return RedirectToAction("Login", "Account");
                            }
                            logingInfoModel.UsedModel = "BankAndBranches";
                            logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                            logingInfoModel.SysDate = DateTime.Now;
                            logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(bankAndBranch);
                            context.LoginInfoModels.Add(logingInfoModel);
                            context.SaveChanges();
                            // Saving Longing Credential 
                            dbContextTransaction.Commit();
                            return RedirectToAction("Index");

                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
               
            }

            return View(bankAndBranch);
        }

        // GET: BankAndBranches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAndBranch bankAndBranch = db.BankAndBranches.Find(id);
            if (bankAndBranch == null)
            {
                return HttpNotFound();
            }
            return View(bankAndBranch);
        }

        // POST: BankAndBranches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,BankName,BranchName")] BankAndBranch bankAndBranch)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ApplicationDbContext())
                {

                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {

                        try
                        {
                            context.Entry(bankAndBranch).State = EntityState.Modified;                 
                            // Saving Longing Credential 
                            LoginInfoModel logingInfoModel = new LoginInfoModel();
                            PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                            logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                            if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                            {
                                return RedirectToAction("Login", "Account");
                            }
                            logingInfoModel.UsedModel = "BankAndBranches";
                            logingInfoModel.TypeOfAction = TypeOfAction.Update;
                            logingInfoModel.SysDate = DateTime.Now;

                            logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(bankAndBranch);
                            context.LoginInfoModels.Add(logingInfoModel);
                            context.SaveChanges();
                            // Saving Longing Credential 
                            dbContextTransaction.Commit();
                            return RedirectToAction("Index");

                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
                
            }
            return View(bankAndBranch);
        }

        // GET: BankAndBranches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAndBranch bankAndBranch = db.BankAndBranches.Find(id);
            if (bankAndBranch == null)
            {
                return HttpNotFound();
            }
            return View(bankAndBranch);
        }

        // POST: BankAndBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            using (var context = new ApplicationDbContext())
            {

                using (var dbContextTransaction = context.Database.BeginTransaction())
                {

                    try
                    {

                        BankAndBranch bankAndBranch = context.BankAndBranches.Find(id);
                       
                        
                        // Saving Longing Credential 
                        LoginInfoModel logingInfoModel = new LoginInfoModel();
                        PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                        logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                        if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                        {
                            return RedirectToAction("Login", "Account");
                        }
                        logingInfoModel.UsedModel = "BankAndBranches";
                        logingInfoModel.TypeOfAction = TypeOfAction.Delete;
                        logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(bankAndBranch);
                        logingInfoModel.SysDate = DateTime.Now;
                        context.LoginInfoModels.Add(logingInfoModel);
                        context.BankAndBranches.Remove(bankAndBranch);
                        // Saving Longing Credential 
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        return RedirectToAction("Index");

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
