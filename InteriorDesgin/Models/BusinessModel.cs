//using System; using InteriorDesign.Repository;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System; using InteriorDesign.Repository;

namespace InteriorDesign.Models
{

    public class LoginInfoModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "User Id")]
        public string UserId { get; set; }

        [Display(Name = "User Password")]
        public string UserPassword { get; set; }

        [Display(Name = "Used Model")]
        public string UsedModel { get; set; }
        public string LoginIp { get; set; }
        public string Data { get; set; }
        public TypeOfAction TypeOfAction { get; set; }

        [Display(Name = "System Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:dd-MM-yyyy}")]
        public DateTime? SysDate { get; set; }

       
    }

    public enum TypeOfAction
    {
        [Display(Name = "Create")]
        Insert,
        [Display(Name = "Edit")]
        Update,
        [Display(Name = "Delete")]
        Delete,
    }

    public class SafetyModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Admin Login Ip")]
        public string AdminLoginIp { get; set; }

        [Display(Name = " Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Display(Name = "Mail Password")]
        public string MailPassword { get; set; }
       
    }
    public class CompanyGrowth
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Start Period")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:dd-MM-yyyy}")]
        public DateTime StartDay { get; set; }
        [Display(Name = "End Period")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:dd-MM-yyyy}")]
        public DateTime EndDay { get; set; }
        public decimal Growth { get; set; }
    }
    public class MenuInfo
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Menu Name ")]
        public string MenuName { get; set; }
        [Display(Name = "Menu Url ")]
        public string MenuURL { get; set; }
        [Display(Name = "Menu Parent ID ")]
        public int MenuParentID { get; set; }
        [Display(Name = "Menu Parent Text ")]
        public string MenuParentIDText { get; set; }

        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanClose { get; set; }
        public bool Active { get; set; }

    }

    public class RoleMaster
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool  Active { get; set; }
        public virtual ICollection< UserMaster > UserMaster { get; set; }
    }


    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Department")]
        public string DepartmentName { get; set; }


       
    }


    public class Organogram
    {
        [Key]
        public int Id { get; set; }


        [Display(Name = "Department")]
        public string Depart { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Upper Role")]
        public string UpperRole { get; set; }

        [NotMapped]
        public int UpperRoleTo { get; set; }


    }





    public class UserMaster
    {
          
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

        public string UserCell { get; set; }
        public string Name { get; set; }

        [ForeignKey("RoleMaster")]
        public int RoleId { get; set; }
        public virtual RoleMaster RoleMaster { get; set; }
        public bool Active { get; set; }
    }

    public class RoleMenuMapping
    {
        [Key]
        public int Id { get; set; }

       
        public int RoleId { get; set; }

        public string RoleIdText { get; set; }

        public int MenuInfoId { get; set; }

        public string MenuInfoIdText { get; set; }

        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanClose { get; set; }

        public bool Active { get; set; }

    }


}