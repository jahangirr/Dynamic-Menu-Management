using System; using InteriorDesign.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using InteriorDesign.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;

namespace InteriorDesign
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

            createRolesandUsers();

        }

        // In this method we will create default User roles and Admin user for login    
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            context.SafetyModels.Add(new SafetyModel()
            {

                Id = 0,
                AdminLoginIp = "0.0.0.0",
                Email = "admin@gmail.com",
                MailPassword = "admin@password"
            });
            context.SaveChanges();


            // In Startup iam creating first Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool    
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                RoleMaster rm = new RoleMaster();
                rm.Active = true;
                rm.Name = role.Name;
                context.RoleMasters.Add(rm);
                context.SaveChanges();



                //Here we create a Admin super user who will maintain the website                   

                var user = new ApplicationUser();
                user.UserName = "admin";
                user.Email = "admin@gmail.com";
                string userPWD = "admin@123";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                   
                    UserMaster userMaster = new UserMaster();
                    userMaster.UserId = user.Id;
                    userMaster.Name = user.UserName;
                    userMaster.RoleId = 1;
                    userMaster.Active = true;
                    context.UserMasters.Add(userMaster);
                    context.SaveChanges();


                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }

                
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "A",
                    MenuURL = "#",
                    MenuParentID = 0,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "S",
                    MenuURL = "#",
                    MenuParentID = 0,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "R",
                    MenuURL = "#",
                    MenuParentID = 0,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "BS",
                    MenuURL = "#",
                    MenuParentID = 0,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();

                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "BT",
                    MenuURL = "#",
                    MenuParentID = 0,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();

                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Safty",
                    MenuURL = "../SafetyModels/Index",
                    MenuParentID = 1,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();

                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Organization Structure",
                    MenuURL = "../ArtWorks/getOrganisationStructure",
                    MenuParentID = 3,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();

                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Menu",
                    MenuURL = "../MenuInfoes/Index",
                    MenuParentID = 1,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();

                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Role",
                    MenuURL = "../RoleMasters/Index",
                    MenuParentID = 1,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();

                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "User",
                    MenuURL = "../UserMasters/Index",
                    MenuParentID = 1,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Role Mapping with User",
                    MenuURL = "../RoleMenuMappings/Index",
                    MenuParentID = 1,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();

                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Report Setup",
                    MenuURL = "../ReportSetups/Index",
                    MenuParentID = 1,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Department",
                    MenuURL = "../Departments/Index",
                    MenuParentID = 1,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Organograms",
                    MenuURL = "../Organograms/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Show Chart",
                    MenuURL = "../ShowChart/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Bank And Branches",
                    MenuURL = "../BankAndBranches/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Active Percentages",
                    MenuURL = "../ActivePercentages/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Type Of Bills",
                    MenuURL = "../TypeOfBills/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Mature Periods",
                    MenuURL = "../MaturePeriods/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true
                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Mail Receivers",
                    MenuURL = "../MailReceivers/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true

                });
                context.SaveChanges();

                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Mature Infoes",
                    MenuURL = "../MatureInfoes/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true

                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Transaction Infoes",
                    MenuURL = "../TransactionInfoes/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true

                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Bank Cheque Advices",
                    MenuURL = "../BankChequeAdvices/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true

                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Mature Bill Info Details",
                    MenuURL = "../MatureBillInfoDetails/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true

                });
                context.SaveChanges();
                context.MenuInfoes.Add(new MenuInfo()
                {
                    Id = 0,
                    MenuName = "Mature Bill Receive Dates",
                    MenuURL = "../MatureBillReceiveDates/Index",
                    MenuParentID = 2,
                    MenuParentIDText = "",
                    Active = true,
                    CanClose = true,
                    CanCreate = true,
                    CanDelete = true,
                    CanEdit = true,
                    CanView = true

                });
                context.SaveChanges();

                context.Departments.Add(new Department() { Id = 0, DepartmentName = "All" });
                context.Departments.Add(new Department() { Id = 0, DepartmentName = "Sales" });
                context.Departments.Add(new Department() { Id = 0, DepartmentName = "Administration" });
                context.Departments.Add(new Department() { Id = 0, DepartmentName = "Accounts" });

                context.SaveChanges();

                string sqlToBeInserted = @" INSERT INTO [dbo].[RoleMenuMappings]
           ([RoleId]
           ,[RoleIdText]
           ,[MenuInfoId]
           ,[MenuInfoIdText]
           ,[CanView]
           ,[CanCreate]
           ,[CanEdit]
           ,[CanDelete]
           ,[CanClose]
           ,[Active])
            select RoleName.Id , RoleName.Name , MInfo.Id , MInfo.MenuName , MInfo.CanView  ,
	        MInfo.CanCreate , MInfo.CanEdit , MInfo.CanDelete , MInfo.CanClose , MInfo.Active from(
            select id, Name from dbo.RoleMasters where Name = 'Admin') RoleName Cross join
            dbo.MenuInfoes MInfo;";
              
             context.Database.ExecuteSqlCommand(sqlToBeInserted);

               

            }

            context.SaveChanges();
          






            //// creating Creating Manager role     
            //if (!roleManager.RoleExists("Manager"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Manager";
            //    roleManager.Create(role);

            //    RoleMaster rm = new RoleMaster();
            //    rm.Active = true;
            //    rm.Name = role.Name;
            //    context.RoleMasters.Add(rm);
            //    context.SaveChanges();

            //}

            //// creating Creating Employee role     
            //if (!roleManager.RoleExists("Employee"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Employee";
            //    roleManager.Create(role);

            //    RoleMaster rm = new RoleMaster();
            //    rm.Active = true;
            //    rm.Name = role.Name;
            //    context.RoleMasters.Add(rm);
            //    context.SaveChanges();

            //}

           
        }
    }
}