using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Http;
using System.Web.Security;
using System.Threading;
using InteriorDesign.Models;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace InteriorDesign
{
    public class MvcApplication : System.Web.HttpApplication
    {



        protected void Application_BeginRequest()
        {
            if (FormsAuthentication.RequireSSL && !Request.IsSecureConnection)
            {
                Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
            }
        }


        private ApplicationDbContext db = new ApplicationDbContext();

        public async Task<string> CallReminderSending()
        {

            while(true)
            {
               await ReminderSending();
            }       
        }
        
        public async Task ReminderSending()
        {

            await  Task.Run(() =>
            {
                try
                {
                    var saftyModel = db.SafetyModels.FirstOrDefault();
                    var senderEmail = new MailAddress(saftyModel.Email, " Interior Admin");

                    foreach (var mr in db.MailReceivers.ToList())
                    {

                        var receiverEmail = new MailAddress(mr.Email, " Interior");
                        var password = saftyModel.MailPassword;



                        DateTime? curDate = DateTime.Now.Date;

                        var matureInfoList = db.MatureInfoes.Where(w => w.MatureDate.Value.Day == curDate.Value.Day && w.MatureDate.Value.Month == curDate.Value.Month && w.MatureDate.Value.Year == curDate.Value.Year).ToList();


                        foreach (var f in matureInfoList)
                        {
                            var fileInfo = f.BankAndBranch.BankName + "_" + f.BankAndBranch.BranchName + "<br/>" + "Mature Date :" + f.MatureDate + "<br/>Mature Amount:" + f.MatureTotalTaka.ToString();

                            string body = fileInfo;


                            var smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(senderEmail.Address, password)
                            };


                            string beginHtmlTag = "<html><head></head><body>";

                            string endHtmlTag = "</body></html>";

                            using (var mess = new MailMessage(senderEmail, receiverEmail)
                            {
                                Subject = "Reminding on Mature Amount",
                                Body = beginHtmlTag + body + endHtmlTag
                            })
                            {
                                mess.IsBodyHtml = true;
                                try
                                {
                                    smtp.Send(mess);
                                }
                                catch (Exception ex)
                                {
                                    Session["mailNotGoingMsg"] = "<script>alert('" + ex.Message.ToString() + "');</script>";

                                }
                            }

                        }

                    }

                    int sleepMilisecond = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TimerIntervalInMilliseconds"].ToString());
                    Task.Delay(sleepMilisecond);
                }catch(Exception ex)
                {

                }
                
            });
           
        }




        protected async void Application_Start()
        {

            ModelBinders.Binders.Add(typeof(string), new TrimStringModelBinder());
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            string sendingEmail = await CallReminderSending();

        }
    }

    public class TrimStringModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var attemptedValue = value?.AttemptedValue;

            return string.IsNullOrWhiteSpace(attemptedValue) ? attemptedValue : attemptedValue.Trim();
        }
    }

}
