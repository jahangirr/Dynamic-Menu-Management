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

    [Authorize]
    public class LoginInfoModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        AutoNumberGenetor auNumberGenetor = new AutoNumberGenetor();

        GlobCreatedFunction gcf = new GlobCreatedFunction();

       

        // GET: LoginInfoModels
        public ActionResult Index()
        {

            List<SelectListItem> ModuleName = new List<SelectListItem>();

            foreach(var um in  db.LoginInfoModels.Select(s => s.UsedModel).Distinct().ToList())
            {
                ModuleName.Add(new SelectListItem() { Text = um , Value = um });
            }
            ViewBag.ModuleNameData = new SelectList(ModuleName, "Value", "Text");

            return View(db.LoginInfoModels.ToList());
        }

        // GET: LoginInfoModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoginInfoModel loginInfoModel = db.LoginInfoModels.Find(id);
            if (loginInfoModel == null)
            {
                return HttpNotFound();
            }
            return View(loginInfoModel);
        }


        public JsonResult getLogInfoDeserialised(string UsedModel , string LogDate , string LogDateTo)
        {
            

            DataTable dt = new DataTable();
            // For Holding Entity Data
            ArtWork artWork = new ArtWork();
            


            return Json("", JsonRequestBehavior.AllowGet);

        }





        // GET: LoginInfoModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoginInfoModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,UserPassword,UsedModel,LoginIp,TypeOfAction,SysDate")] LoginInfoModel loginInfoModel)
        {
            if (ModelState.IsValid)
            {
                db.LoginInfoModels.Add(loginInfoModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(loginInfoModel);
        }

        // GET: LoginInfoModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoginInfoModel loginInfoModel = db.LoginInfoModels.Find(id);
            if (loginInfoModel == null)
            {
                return HttpNotFound();
            }
            return View(loginInfoModel);
        }

        // POST: LoginInfoModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,UserPassword,UsedModel,LoginIp,TypeOfAction,SysDate")] LoginInfoModel loginInfoModel)
        {
            if (ModelState.IsValid)
            {
               // db.Entry(loginInfoModel).State = EntityState.Modified;
               // db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loginInfoModel);
        }

        // GET: LoginInfoModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoginInfoModel loginInfoModel = db.LoginInfoModels.Find(id);
            if (loginInfoModel == null)
            {
                return HttpNotFound();
            }
            return View(loginInfoModel);
        }

        // POST: LoginInfoModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoginInfoModel loginInfoModel = db.LoginInfoModels.Find(id);
           // db.LoginInfoModels.Remove(loginInfoModel);
           // db.SaveChanges();
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

    [Authorize]
    public class PopulateLoginInfoCredencial
    {
        LoginInfoModel loginInfoModel = new LoginInfoModel();
        ApplicationDbContext db = new ApplicationDbContext();

        public static string IPAddress
        {
            get
            {
                string userIP;
                HttpRequest Request = System.Web.HttpContext.Current.Request;
                if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != "")
                    userIP = Request.ServerVariables["REMOTE_ADDR"];
                else
                    userIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (userIP == null || userIP == "")
                    userIP = Request.UserHostAddress;
                return userIP;
            }
        }


        public bool ValidateIPv4(string ipString)
        {


            var adminLogin = db.SafetyModels.Select(s => s.AdminLoginIp).FirstOrDefault();

            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                if(ipString == "::1" || ipString == adminLogin)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public PopulateLoginInfoCredencial(LoginInfoModel loginInfoModel)
        {
            var user = db.Users.Where(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault();
            loginInfoModel.SysDate = DateTime.Now.Date;
            loginInfoModel.UserId = user.Id;
            loginInfoModel.UserPassword = user.PasswordHash;
            loginInfoModel.LoginIp = IPAddress;
            if(loginInfoModel.LoginIp == "0" || loginInfoModel.LoginIp =="")
            {
                //
                //  return to  Logining Control
                //
            }

            this.loginInfoModel = loginInfoModel;
        }

        public LoginInfoModel getLoginInfoCredencial()
        {
            return this.loginInfoModel;
        }

    }









}
