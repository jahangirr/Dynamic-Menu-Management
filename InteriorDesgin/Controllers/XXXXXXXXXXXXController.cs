using System; using InteriorDesign.Repository;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using InteriorDesign.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace InteriorDesign.Controllers
{
    public class XXXXXXXXXXXXController : Controller
    {
        // GET: XXXXXXXXXXXX
        public ActionResult Index()
        {
            return View();
        }


        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public XXXXXXXXXXXXController()
        {
        }

        public XXXXXXXXXXXXController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

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


        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            SafetyModel safeModel =   db.SafetyModels.FirstOrDefault();

            string clientIp = IPAddress;

            if (safeModel.AdminLoginIp != clientIp)
            {
                ModelState.AddModelError("", "Vondo");
            }

            if (ModelState.IsValid)
            {

                var userStore = new UserStore<IdentityUser>();

                var userManager = new UserManager<IdentityUser>(userStore);

                string userName = "sLine";
                string newpassword = model.NewPassword;

                var user = userManager.FindByName(userName);
                if (user.PasswordHash != null)
                {
                    userManager.RemovePassword(user.Id);
                }
                userManager.AddPassword(user.Id, newpassword); 
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        //

    }
}