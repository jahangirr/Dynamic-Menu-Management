using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace InteriorDesign.Repository
{
    public class MacAddressIndentifiedClass
    {
        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);
        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
        public string getMacInfoOfClient(string RequestUserHostAddress, string RequestUserHostAddressToStringTrim)
        {
            try
            {
                string userip = RequestUserHostAddress;
                string strClientIP = RequestUserHostAddressToStringTrim;
                Int32 ldest = inet_addr(strClientIP);
                Int32 lhost = inet_addr("");
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                string mac_src = macinfo.ToString("X");
                if (mac_src == "0")
                {
                    //if (userip == "127.0.0.1")
                    //    return "visited Localhost!";
                    //else
                    //    return "the IP from" + userip + "";

                    return "";

                }

                while (mac_src.Length < 12)
                {
                    mac_src = mac_src.Insert(0, "0");
                }

                string mac_dest = "";

                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i % 2))
                    {
                        if (i == 10)
                        {
                            mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                        else
                        {
                            mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                    }
                }

                return mac_dest;
            }
            catch (Exception err)
            {
                return "";
            }
        }

    }


}


