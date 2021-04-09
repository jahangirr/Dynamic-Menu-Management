using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using InteriorDesign.Models;
using System; using InteriorDesign.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteriorDesign.Repository
{
    public class GlobCreatedFunction
    {
        public string GetDate(string datetime)
        {
            string rtDate = "";

            try
            {
                List<string> stringList = datetime.Split('-').ToList();
                rtDate = stringList[2].ToString().Substring(0, 4).ToString() + "-" + stringList[1].ToString() + "-" + stringList[0].ToString() + " 00:00:00.000";

            }
            catch (Exception ex)
            {

            }
            return rtDate;

        }

        public string GetDateReverse(DateTime? datetime)
        {
            

            var dateFormat = "";
            var yesProceed = false;

            try
            {
                DateTime dt = Convert.ToDateTime(datetime.ToString());
                yesProceed = true;
            }
            catch(Exception ex)
            {

            }

            if (yesProceed)
            {

                if (datetime.Value.Day.ToString().Length > 1)
                {
                    dateFormat = datetime.Value.Day.ToString() + "-";
                }
                else
                {
                    dateFormat = "0" + datetime.Value.Day.ToString() + "-";
                }

                if (datetime.Value.Month.ToString().Length > 1)
                {
                    dateFormat = dateFormat + datetime.Value.Month.ToString() + "-";
                }
                else
                {
                    dateFormat = dateFormat + "0" + datetime.Value.Month.ToString() + "-";
                }

                if (datetime.Value.Year.ToString().Length > 1)
                {
                    dateFormat = dateFormat + datetime.Value.Year.ToString();
                }
                else
                {
                    dateFormat = dateFormat + "0" + datetime.Value.Year.ToString();
                }
            }

            return dateFormat;


        }
    }


    public class FileStoreCreateFolderName
    {
        public string getFolderName()
        {
            string folName = DateTime.Now.Year.ToString();

            if (DateTime.Now.Month.ToString().Length == 1)
            {
                folName = folName + "0" + DateTime.Now.Month.ToString();
            }
            else
            {
                folName = folName + DateTime.Now.Month.ToString();
            }

            if (DateTime.Now.Day.ToString().Length == 1)
            {
                folName = folName + "0" + DateTime.Now.Day.ToString();
            }
            else
            {
                folName = folName + DateTime.Now.Day.ToString();
            }


            return folName;


        }
    }


    public class MembershipHelp
    {
      
        public string logingUserRole(string Name)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var user = context.Users.Where(x => x.UserName == Name).FirstOrDefault().Id;
           
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string s = UserManager.GetRoles(user).FirstOrDefault();

            return s;
        }
    }
}