using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace InteriorDesign.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            
        }


        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Properties<decimal>().Configure(c => c.HasPrecision(18, 3));
          

            base.OnModelCreating(modelBuilder);
        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

       

        public System.Data.Entity.DbSet<InteriorDesign.Models.RoleMaster> RoleMasters { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.MenuInfo> MenuInfoes { get; set; }

        

        public System.Data.Entity.DbSet<InteriorDesign.Models.UserMaster> UserMasters { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.RoleMenuMapping> RoleMenuMappings { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.SafetyModel> SafetyModels { get; set; }

      
        public System.Data.Entity.DbSet<InteriorDesign.Models.LoginInfoModel> LoginInfoModels { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.CompanyGrowth> CompanyGrowths { get; set; }

      
        public System.Data.Entity.DbSet<InteriorDesign.Models.ReportSetup> ReportSetups { get; set; }

      

        public System.Data.Entity.DbSet<InteriorDesign.Models.ArtWork> ArtWorks { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.Department> Departments { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.Organogram> Organograms { get; set; }

        

       

        public System.Data.Entity.DbSet<InteriorDesign.Models.TestCalll> TestCallls { get; set; }

       

        public System.Data.Entity.DbSet<InteriorDesign.Models.RoleBasedSignature> RoleBasedSignatures { get; set; }

      
      
       

       

        public System.Data.Entity.DbSet<InteriorDesign.Models.BankAndBranch> BankAndBranches { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.ActivePercentage> ActivePercentages { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.MaturePeriod> MaturePeriods { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.TypeOfBill> TypeOfBills { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.MatureInfo> MatureInfoes { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.MatureBillInfoDetails> MatureBillInfoDetails { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.TransactionInfo> TransactionInfoes { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.BankChequeAdvice> BankChequeAdvices { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.MatureBillReceiveDate> MatureBillReceiveDates { get; set; }

        public System.Data.Entity.DbSet<InteriorDesign.Models.MailReceiver> MailReceivers { get; set; }
    }
}