using InteriorDesign.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InteriorDesign.Controllers
{
    public class HomeController : Controller
    {

      


        public ActionResult Index()
        {

            string inworld = TakaToInWord.DecimalToWords(Convert.ToDecimal("455245.10"),"EUR");

            string KKKKK = inworld;


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}