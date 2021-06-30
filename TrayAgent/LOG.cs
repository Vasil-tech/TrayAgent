using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Data.SqlClient; //Библиотека для работы с MS SQL-сервером

namespace TrayAgent
{
    public class LOG
    {   //Работа с журналами агента
        public static string GetTodayFile( )
        {
            string result = DateTime.Today.ToString("yyyyMMdd" + ".log");
            return result;
        }

        public static void DoAccessLog(string SInfo)
        {
            if (CNST.WRITELOG)
            try
            {
                using (StreamWriter writer = new StreamWriter(GetTodayFile(), true))
                {
                    writer.WriteLine(DateTime.Now.ToString()+"\t"+ SInfo);
                }
            }
            catch 
            {
            }
        }

        public static void WriteLog(string server, int port, int sig, int Tmax, int Tavg, int Tmax0, int Tavg0)
        {
            string cnStr = "User ID=trayagent;Password=trayagent_2018;" +
                "Data Source =" + server + "," + port + ";" +
                "Initial Catalog=NEWSTAT;Connection Timeout=300 ";
            try
            {
                using (SqlConnection con = new SqlConnection(cnStr))
                {
                    IPHostEntry myhost = Dns.GetHostEntry(Dns.GetHostName());
                    string IPAddress="";
                    foreach (IPAddress ip in myhost.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            IPAddress=ip.ToString();
                            break;
                        }
                    }
                    con.Open();
                    string sExec = "EXECUTE [dbo].[pTrayAgentWrite] "
                               + "@SIG=" + sig + ", "
                              + "@CName='" + Environment.MachineName + "', "
                              + "@IP='" + IPAddress + "', "
                              + "@TMAX=" + Tmax + ", "
                              + "@TAVG=" + Tavg + ", "
                              + "@TMAX0=" + Tmax0 + ", "
                              + "@TAVG0=" + Tavg0 + ";";

                    SqlCommand cmd = new SqlCommand(sExec, con);     
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                DoAccessLog("\t"+ ex.Message);   
            }
        }
    }
}