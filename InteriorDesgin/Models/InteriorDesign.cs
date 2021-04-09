using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace InteriorDesign.Models
{
    public class BankAndBranch
    {
        [Key]

        public int Id { get; set; }

        [Display(Name = "Bank Name")]
        public string BankName { get; set; }

        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        public virtual ICollection<MatureInfo> MatureInfo { get; set; }


    }

    public class ActivePercentage
    {
        [Key]

        public int Id { get; set; }

        [Display(Name = "Active Percentage")]
        public decimal Percentage { get; set; }
    }

    public class MaturePeriod
    {
        [Key]

        public int Id { get; set; }

        [Display(Name = "Mature Period In Days")]
        public int MaturePeriodInDays { get; set; }
    }

    public class TypeOfBill
    {
        [Key]

        public int Id { get; set; }

        [Display(Name = "Bill Type")]
        public string BillType { get; set; }

        public virtual ICollection<MatureBillInfoDetails> MatureBillInfoDetails { get; set; }
    }



    public class MatureInfo
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Bank_Branch")]
        [ForeignKey("BankAndBranch")]
        public int BankAndBranchId { get; set; }


        [Display(Name = "Open Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:dd-MM-yyyy}")]
        public DateTime? OpenDate { get; set; }
        [Display(Name = "Total Area")]
        public Decimal TotalArea { get; set; }


        [Display(Name = "Quotation Submit")]
        public bool? QuotationSubmit { get; set; }

        [Display(Name = "Total Bill Amount")]
        public Decimal TotalBillAmount { get; set; }

        [Display(Name = "Mature Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:dd-MM-yyyy}")]
        public DateTime? MatureDate { get; set; }

        [Display(Name = "Mature Total Taka")]
        public Decimal MatureTotalTaka { get; set; }


        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public virtual BankAndBranch BankAndBranch { get; set; }

        public virtual ICollection<MatureBillInfoDetails> MatureBillInfoDetails { get; set; }

        public virtual ICollection<TransactionInfo> TransactionInfo { get; set; }

        public virtual ICollection<BankChequeAdvice> BankChequeAdvice { get; set; }

        public virtual ICollection<MatureBillReceiveDate> MatureBillReceiveDate { get; set; }



    }




    public class MatureInfoTemp
    {
       
        public int Id { get; set; }

      
        public int BankAndBranchId { get; set; }


        public DateTime? OpenDate { get; set; }
       
        public Decimal TotalArea { get; set; }


      
        public bool? QuotationSubmit { get; set; }

       
        public Decimal TotalBillAmount { get; set; }

       
        public DateTime? MatureDate { get; set; }

       
        public Decimal MatureTotalTaka { get; set; }


       
        public string Notes { get; set; }

       

    }





    public class MatureBillInfoDetails
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Bank_Branch")]
        [ForeignKey("MatureInfo")]
        public int MatureInfoId { get; set; }

        [ForeignKey("TypeOfBill")]

        [Display(Name = "Bill Type")]
        public int TypeOfBillId { get; set; }

        [Display(Name = "Amount")]
        public Decimal Amount { get; set; }

        public virtual MatureInfo MatureInfo { get; set; }

        public virtual TypeOfBill TypeOfBill { get; set; }

    }


    public class MatureBillInfoDetailsTemp
    {
        
        public int Id { get; set; }

        
        public int MatureInfoId { get; set; }

       
        public int TypeOfBillId { get; set; }

       
        public Decimal Amount { get; set; }

   

    }

    public class TransactionInfo
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Bank_Branch")]
        [ForeignKey("MatureInfo")]
        public int MatureInfoId { get; set; }

        [Display(Name = "Advance Cheque/Advice/Pay Order")]
        public string AdvCheque_Advice_PayOrder { get; set; }

        [Display(Name = "Amount")]
        public Decimal Amount { get; set; }

        public virtual MatureInfo MatureInfo { get; set; }


    }

    public class TransactionInfoTemp
    {
      
        public int MatureInfoId { get; set; }

        
        public string AdvCheque_Advice_PayOrder { get; set; }

       
        public Decimal Amount { get; set; }

       


    }


    public class BankChequeAdviceTemp
    {
       
        public int Id { get; set; }
        
        public int MatureInfoId { get; set; }      
        public string BankChequeAdviceNo { get; set; }
       
    }


    public class BankChequeAdvice
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Bank_Branch")]
        [ForeignKey("MatureInfo")]
        public int MatureInfoId { get; set; }

        [Display(Name = "Bank Cheque Advice No")]
        public string BankChequeAdviceNo { get; set; }
        public virtual MatureInfo MatureInfo { get; set; }




    }


    public class MatureBillReceiveDate
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("MatureInfo")]
        [Display(Name = "Bank_Branch")]
        public int MatureInfoId { get; set; }

        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:dd-MM-yyyy}")]
        public DateTime ReceiveDate { get; set; }

        [Display(Name = "Receive Amount")]
        public Decimal ReceiveAmount { get; set; }

        public virtual MatureInfo MatureInfo { get; set; }

    }

    public class MatureBillReceiveDateTemp
    {
       
        public int Id { get; set; }

       
        public int MatureInfoId { get; set; }

       
        public DateTime ReceiveDate { get; set; }

      
        public Decimal ReceiveAmount { get; set; }

      

    }

    public class MailReceiver
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = " Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

    }
}