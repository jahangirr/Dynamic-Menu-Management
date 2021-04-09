using System;
using InteriorDesign.Repository;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using InteriorDesign.Models;
using System.ComponentModel;

namespace InteriorDesign.Repository
{
    public class AutoNumberGenetor
    {

        private ApplicationDbContext db = new ApplicationDbContext();



        public DataTable ConvertListToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        public static DataTable ListToDataTable<T>(IList<T> data, IList<LoginInfoModel> logingModel)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<PartialLoginInfo> partialLoginInfo = new List<PartialLoginInfo>();

            foreach (var loginInfo in logingModel)
            {

                partialLoginInfo.Add(new PartialLoginInfo()
                {

                    LoginIp = loginInfo.LoginIp,
                    SysDate = loginInfo.SysDate.ToString(),
                    TypeOfAction = loginInfo.TypeOfAction.ToString(),
                    UserId = loginInfo.UserId

                });
            }

            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, typeof(String));
            }


            //  Extra column adding for Indentification

            PropertyDescriptorCollection propsLogings = TypeDescriptor.GetProperties(typeof(PartialLoginInfo));
            for (int i = 0; i < propsLogings.Count; i++)
            {
                PropertyDescriptor propsLoging = propsLogings[i];
                table.Columns.Add(propsLoging.Name, typeof(String));


            }
            // Extra column adding for Indentification


            // checking table row

            DataRow dr = table.NewRow();



            string userName = "";

            object[] values = new object[props.Count + 4];
            foreach (T item in data)
            {
                int indexToLoginInfo = data.IndexOf(item);

                var logInfoSingle = partialLoginInfo[indexToLoginInfo];

                for (int i = 0; i < values.Length; i++)
                {
                    if (i < props.Count)
                    {
                        values[i] = props[i].GetValue(item);
                    }
                    else
                    {
                        if (propsLogings[i - props.Count].Name == "UserId")
                        {
                            userName = (string)(propsLogings[i - props.Count].GetValue(logInfoSingle));
                            userName = db.UserMasters.Where(w => w.UserId == userName).FirstOrDefault().Name;
                            values[i] = userName;
                        }
                        else
                        {
                            values[i] = propsLogings[i - props.Count].GetValue(logInfoSingle);
                        }

                    }

                }

                table.Rows.Add(values);
            }
            return table;
        }

        public string ConvertDataTableToHTML(DataTable dt)
        {
            string html = "<table class='table-bordered' style='background-color:darksalmon; border: 2px; border-color:green; color: black; '>";
            //add header row
            html += "<tr>";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<td style='width:300'>" + dt.Columns[i].ColumnName + "</td>";
            html += "</tr>";
            //add rows
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td style='width:300'>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</table>";
            return html;
        }


        public string GetMaxIdFromLogic(string heightNumber)
        {
            int totalStr = heightNumber.Length;
            int totalLoop = 8;
            int totalMove = totalLoop - totalStr;
            string loopString = "";
            for (int i = 1; i < totalMove; i++)
            {
                loopString = loopString + "0";
            }


            return "SPR-" + loopString + heightNumber.ToString();
        }


        public string GetMaxId(string controllerName, string prfix)
        {
            string returnProcessedId = "";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {

                SqlCommand cmd = new SqlCommand("select  ((case when (select count(*) from dbo." + controllerName + " ) = 0 then 0 else ( select max(Id) from  dbo." + controllerName + " ) end) + 1)Id ", con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                while (sdr.Read())
                {

                    returnProcessedId = sdr["Id"].ToString();
                    int totalStr = returnProcessedId.Length;
                    int totalLoop = 8;
                    int totalMove = totalLoop - totalStr;
                    string loopString = "";
                    for (int i = 1; i < totalMove; i++)
                    {
                        loopString = loopString + "0";
                    }
                    returnProcessedId = prfix + loopString + returnProcessedId;
                    break;

                }

            }


            return returnProcessedId;
        }

        public int GetAutoGenFromMaxId(string controllerName, string controllerAlias)
        {

            string heightNumber = "";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            SqlCommand cmd = new SqlCommand("select   (IDENT_CURRENT ('" + controllerName + "') + IDENT_INCR('" + controllerName + "') )NextMax", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable datatable = new DataTable("sql_MaxIdFor" + controllerAlias);
            da.Fill(datatable);

            heightNumber = datatable.Rows[0]["NextMax"].ToString();

            return Convert.ToInt32(heightNumber);

        }

        public string GetProcessedAutoGenFromMaxId(string controllerName, string controllerAlias)
        {

            string heightNumber = "";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            SqlCommand cmd = new SqlCommand("select   (IDENT_CURRENT ('" + controllerName + "') + IDENT_INCR('" + controllerName + "') )NextMax", con);
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataTable datatable = new DataTable("sql_MaxIdFor" + controllerAlias);
            da.Fill(datatable);

            heightNumber = datatable.Rows[0]["NextMax"].ToString();

            int totalStr = heightNumber.Length;
            int totalLoop = 8;
            int totalMove = totalLoop - totalStr;
            string loopString = "";
            for (int i = 1; i < totalMove; i++)
            {
                loopString = loopString + "0";
            }

            return controllerAlias + "-" + loopString + heightNumber.ToString();



        }
        public string GetMOMSprOrIndentNoS(ApplicationDbContext db)
        {

            return "";
        }



        public string GetGRNNumber(ApplicationDbContext db)
        {
            return "";


        }



        public class PartialLoginInfo
        {

            public string UserId { get; set; }
            public string LoginIp { get; set; }

            public string TypeOfAction { get; set; }
            public string SysDate { get; set; }


        }

    }
}