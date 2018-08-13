using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPIWithPS.Models;
using WebAPIWithPS.PowershellFactory;

namespace WebAPIWithPS.Controllers
{
    public class SQLServerStateController : ApiController
    {
        private static ConcurrentDictionary<string, PowerShellExecutor<SQLServerState>> executors =
         new ConcurrentDictionary<string, PowerShellExecutor<SQLServerState>>();

        public IEnumerable<SQLServerState> GetServerStatus(string AccountType, string AccountID)
        {
            string filePath = "";
            if (AccountType == "Azure")
            {
                filePath = HttpContext.Current.Server.MapPath("~/DB_Csv_files/AzureAccountCreds.csv");
            }
            else if (AccountType == "AWS")
            {
                filePath = HttpContext.Current.Server.MapPath("~/DB_Csv_files/AWSAccountCreds.csv");
            }
            PowerShellExecutor<SQLServerState> executor;
            string key = "";
            if (!SQLServerStateController.executors.TryGetValue(key, out executor))
            {
                executor = new PowerShellExecutor<SQLServerState>();
                executor.CmdName = "";
                executor.NewFunc = this.NewFunc;
                executor.Configure = (engine) =>
                {
                    PowerShellJob.AddVariable(engine, "CredFile", filePath);
                    PowerShellJob.AddVariable(engine, "AccountID", AccountID);
                    engine.AddScript(AzureCommandResource.Azure_SQLServerState_PS);
                };
                executor = SQLServerStateController.executors.GetOrAdd(key, executor);
            }


            var list = executor.GetList();
            return list;
        }
        public void NewFunc(dynamic psObject, List<SQLServerState> sqldetails)
        {
            //var users = psObject.UserGroup as string[];

            var sql = new SQLServerState();
            sql.AccountID = psObject.AccountID as string;
            sql.AccountType = psObject.AccountType as string;
            sql.DBServerName = psObject.DBServerName as string;
            sql.DataBaseName = psObject.DataBaseName as string;
            sql.EndpointAddress = psObject.EndpointAddress as string;
            sql.State = psObject.State as string;
            sql.ServerGroup = psObject.ServerGroup as string;
            sql.DeploymentType = psObject.DeploymentType as string;
            sql.flagColor = psObject.flagColor as string;
            sqldetails.Add(sql);

        }
    }
}
