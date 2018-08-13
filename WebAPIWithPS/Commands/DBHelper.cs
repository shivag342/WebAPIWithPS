namespace WebAPIWithPS.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Xml;
    public static class DBHelper
    {

        private static string[] GetParamNames(string sql)
        {
            List<string> parameters = new List<string>();
            Regex regex = new Regex("@[a-zA-z0-9^\\w]*");
            var match = regex.Match(sql);
            while (match.Success)
            {
                parameters.Add(match.Value);
                match = match.NextMatch();
            }

            return parameters.ToArray();
        }

        public static int ExecuteCommand (string sql, params object[] values)
        {
            try
            {
                //string cs = ConfigurationManager.ConnectionStrings["Microsoft.MgmtSvc.RDDeployments"].ConnectionString;
                Dictionary<string, string> credentialList = GetDBCredentials();
                string cs = "Initial Catalog=" + credentialList["dbName"].ToString() + ";Server=" + credentialList["serverName"].ToString() + ";Password=" + credentialList["password"].ToString() + ";User Id=" + credentialList["userId"].ToString() + ";";
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        var parameterNames = DBHelper.GetParamNames(sql);
                        for (int i = 0; i < parameterNames.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(parameterNames[i], values[i]);
                        }

                        return cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHelper.WriteErrorToEventLog(ex.Message);
                // ErrorHelper.SendExcepToDB(ex, "ExecuteCommand",);
                throw ex;
            }
        }
        public static DataTable ExecuteAndGetDataTable(string sql, params object[] values)
        {
            try
            {
                //string cs = ConfigurationManager.ConnectionStrings["Microsoft.MgmtSvc.RDDeployments"].ConnectionString;
                Dictionary<string, string> credentialList = GetDBCredentials();
                string cs = "Initial Catalog=" + credentialList["dbName"].ToString() + ";Server=" + credentialList["serverName"].ToString() + ";Password=" + credentialList["password"].ToString() + ";User Id=" + credentialList["userId"].ToString() + ";";
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        var parameterNames = DBHelper.GetParamNames(sql);
                        for (int i = 0; i < parameterNames.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(parameterNames[i], values[i]);
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = cmd;
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHelper.WriteErrorToEventLog(ex.Message);
                throw ex;
            }
        }
        public static DataSet ExecuteAndGetDataSet(string sql, params object[] values)
        {
            try
            {
                //string cs = ConfigurationManager.ConnectionStrings["Microsoft.MgmtSvc.RDDeployments"].ConnectionString;
                Dictionary<string, string> credentialList = GetDBCredentials();
                string cs = "Initial Catalog=" + credentialList["dbName"].ToString() + ";Server=" + credentialList["serverName"].ToString() + ";Password=" + credentialList["password"].ToString() + ";User Id=" + credentialList["userId"].ToString() + ";";
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        var parameterNames = DBHelper.GetParamNames(sql);
                        for (int i = 0; i < parameterNames.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(parameterNames[i], values[i]);
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter();
                        adapter.SelectCommand = cmd;
                        DataSet dataset = new DataSet();
                        adapter.Fill(dataset);
                        return dataset;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorHelper.WriteErrorToEventLog(ex.Message);
                throw ex;
            }
        }
        public static List<T> ExecuteQuery<T>(string sql, Action<IDataReader, List<T>> process, params object[] values)
        {
            //string cs = ConfigurationManager.ConnectionStrings["Microsoft.MgmtSvc.RDDeployments"].ConnectionString;
            Dictionary<string, string> credentialList = GetDBCredentials();
            string cs = "Initial Catalog=" + credentialList["dbName"].ToString() + ";Server=" + credentialList["serverName"].ToString() + ";Password=" + credentialList["password"].ToString() + ";User Id=" + credentialList["userId"].ToString() + ";";
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    var list = new List<T>();

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        var parameterNames = DBHelper.GetParamNames(sql);
                        for (int i = 0; i < parameterNames.Length; i++)
                        {
                            cmd.Parameters.AddWithValue(parameterNames[i], values[i]);
                        }

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                process(rdr, list);
                            }
                        }
                    }

                    return list;
                }
            }
            catch (Exception ex)
            {
                ErrorHelper.WriteErrorToEventLog(ex.Message);
                throw ex;
            }
        }

        public static Dictionary<string, string> GetDBCredentials()
        {
            Dictionary<string, string> credentialList = new Dictionary<string, string>();
            try
            {
                XmlDataDocument xmldoc = new XmlDataDocument();
                XmlNodeList xmlnode;
                string filePath = HttpContext.Current.Server.MapPath("~/Settings/DBServerCredentials.xml");
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);
                xmlnode = xmldoc.GetElementsByTagName("Credentials");
                for (int i = 0; i <= xmlnode.Count - 1; i++)
                {
                    credentialList.Add("serverName", xmlnode[i].ChildNodes.Item(0).InnerText.Trim());
                    credentialList.Add("dbName", xmlnode[i].ChildNodes.Item(1).InnerText.Trim());
                    credentialList.Add("userId", xmlnode[i].ChildNodes.Item(2).InnerText.Trim());
                    credentialList.Add("password", xmlnode[i].ChildNodes.Item(3).InnerText.Trim());
                }
                fs.Close();
                fs.Dispose();
            }
            catch { }
            return credentialList;
        }
    }
}