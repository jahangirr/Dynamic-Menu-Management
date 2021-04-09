using System;
using InteriorDesign.Repository;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace InteriorDesign.Models
{
   


    public class ReportSetup
    {

        [Key]
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string CompanyLogoPath { get; set; }

        public string AddressLineOne { get; set; }

        public string AddressLineTwo { get; set; }

        public string AddressLineThree { get; set; }

        public string Telephone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }


    }



    public class RoleBasedSignature
    {
        [Key]
        public int Id { get; set; }
        public string RoleName { get; set; }

        [Display(Name = "Signature Mime Type")]
        public int SignatureMimeType { get; set; }

        [Display(Name = "Signature")]
        public byte[] SignatureData { get; set; }

        public string FileAddressInfo { get; set; }


        [Display(Name = "Is Active")]
        public bool IsAcive { get; set; }

        [Display(Name = "Save Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:dd-MM-yyyy}")]
        public DateTime? CreateTime { get; set; }
    }

    public class ArtWork
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Department")]
        public string ArtWorkType { get; set; }

        [Display(Name = "Document Description")]
       
        public string ArtWorkTypeDescribtion { get; set; }

        [Display(Name = "Is Public")]
        public bool CanAccessByGeneral { get; set; }



        [Display(Name = "Is UpDown")]
        public bool IsUpDown { get; set; }


        [Display(Name = "List of Valid Department to see")]
        public string ValidDepartment { get; set; }

        [Display(Name = "Image Mime Type")]
        public int ImageMimeType { get; set; }

        [Display(Name = "Image")]
        public byte[] ImageData { get; set; }

        [Display(Name = "Thumbnail")]
        public byte[] ArtworkThumbnail { get; set; }

        public string FileAddressInfo { get; set; }


        [Display(Name = "Refference")]
       
        public string DocReference { get; set; }

        public string User { get; set; }


        [Display(Name = "Save Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"{0:dd-MM-yyyy}")]
        public DateTime? CreateTime { get; set; }

        public List<CheckBoxListDepartment> Departments { get; set; }


    }

    public class CheckBoxListDepartment
    {
       
        public int Id { get; set; }
        public string value { get; set; }
       
    }

    public class TestCalll
    {
        [Key]
        public int Id { get; set; }
        public string value { get; set; }

    }



}