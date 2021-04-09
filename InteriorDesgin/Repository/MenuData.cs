using InteriorDesign.Models;
using System; using InteriorDesign.Repository;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace InteriorDesign.Repository
{
    public class MenuData
    {
        string topUl = "main-navigation";
        string upperDive = "nav navbar-nav";
        string ulTopClass = "dropdown-menu";
        string liSubDropMenu = "dropdown-submenu";
        string ulMainDropMenu = "dropdown-menu";
        string ulMulitclass = "dropdown-menu multi-level";
        string aClass = "dropdown-toggle";
        string adatatoggle = "dropdown";
        string sClass = "caret";

        public static IList<MenuInfo> GetMenus(string UserId)
        {
            /* using ado.net code */
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                List<MenuInfo> menuList = new List<MenuInfo>();
                SqlCommand cmd = new SqlCommand("usp_GetMenuData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    MenuInfo menu = new MenuInfo();
                    menu.Id = Convert.ToInt32(sdr["MID"].ToString());
                    menu.MenuName = sdr["MenuName"].ToString();
                    menu.MenuURL = sdr["MenuURL"].ToString();
                    menu.MenuParentID = Convert.ToInt32(sdr["MenuParentID"].ToString());
                    menuList.Add(menu);
                }
                return menuList;
            }
        }




        private bool hasChild(MenuInfo m, IList<MenuInfo> mList)
        {
            bool retBool = false;
            int childCount = 0;
            childCount = mList.Where(ml => ml.MenuParentID == m.Id).Count();
            if (childCount == 0)
            {
                retBool = false;
            }
            else
            {
                retBool = true;
            }
            return retBool;

        }

        public string GetMenuAsString(string UserId)
        {


            StringBuilder menuBuilder = new StringBuilder();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                List<MenuInfo> menuList = new List<MenuInfo>();
                SqlCommand cmd = new SqlCommand("usp_GetMenuData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {
                    MenuInfo menu = new MenuInfo();
                    menu.Id = Convert.ToInt32(sdr["Id"].ToString());
                    menu.MenuName = sdr["MenuName"].ToString();
                    menu.MenuURL = sdr["MenuURL"].ToString();
                    menu.MenuParentID = Convert.ToInt32(sdr["MenuParentID"].ToString());
                    menuList.Add(menu);
                }


                menuBuilder.Append("<ul class=\"" + topUl + "\">");

                foreach (var sMenu in menuList)
                {

                    if (sMenu.MenuParentID == 0)
                    {
                        if (hasChild(sMenu, menuList))
                        {

                            populateMultiLevelMenu(sMenu, menuList, menuBuilder);
                        }
                        else
                        {

                            menuBuilder.Append("<li> <a    href=\"" + sMenu.MenuURL + "\">" + sMenu.MenuName + " <span class=\"" + sClass + "\"></span> </a> </li>");

                        }


                    }
                }
                menuBuilder.Append("</ul>");
            }

            return menuBuilder.ToString();

        }


        private void populateMultiLevelMenu(MenuInfo m, IList<MenuInfo> mList, StringBuilder menuBuilder)
        {

            ulMainDropMenu = string.IsNullOrEmpty(ulMulitclass) ? ulMainDropMenu : ulMulitclass;
            ulMulitclass = "";
            IList<MenuInfo> menuByParentMenu = mList.Where(mL => mL.MenuParentID == m.Id).ToList();
            menuBuilder.Append("<li  > <a  href=\"" + m.MenuURL + "\" >" + m.MenuName + " <span class=\"" + sClass + "\"></span> </a><ul >");
            foreach (var sObject in menuByParentMenu)
            {

                if (hasChild(sObject, mList))
                {
                    populateMultiLevelMenu(sObject, mList, menuBuilder);
                }
                else
                {
                    menuBuilder.Append("<li  > <a   href=\"" + sObject.MenuURL + "\" >" + sObject.MenuName + " <span class=\"" + sClass + "\"></span> </a> </li>");
                }

            }
            menuBuilder.Append("</ul></li>");


        }




      
    }
}
