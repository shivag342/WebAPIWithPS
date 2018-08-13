using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPIWithPS.Commands;
using WebAPIWithPS.Models;
using WebAPIWithPS.PowershellFactory;

namespace WebAPIWithPS.Controllers
{
    public class ServerStateController : ApiController
    {

        private static ConcurrentDictionary<string, PowerShellExecutor<ServerState>> executors =
      new ConcurrentDictionary<string, PowerShellExecutor<ServerState>>();

        public IEnumerable<ServerState> GetServerStatus(string AccountType, string AccountID)
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
            PowerShellExecutor<ServerState> executor;
            string key = "";
            if (!ServerStateController.executors.TryGetValue(key, out executor))
            {
                executor = new PowerShellExecutor<ServerState>();
                executor.CmdName = "";
                executor.NewFunc = this.NewFunc;
                executor.Configure = (engine) =>
                {
                    PowerShellJob.AddVariable(engine, "CredFile", filePath);
                    PowerShellJob.AddVariable(engine, "AccountID", AccountID);
                    engine.AddScript(AzureCommandResource.Azure_ServerState_PS);
                };
                executor = ServerStateController.executors.GetOrAdd(key, executor);
            }


            var list = executor.GetList();
            return list;
        }
        public void NewFunc(dynamic psObject, List<ServerState> vmdetails)
        {
            //var users = psObject.UserGroup as string[];

            var vm = new ServerState();
            vm.AccountID = psObject.AccountID as string;
            vm.AccountType = psObject.AccountType as string;
            vm.ServerName = psObject.ServerName as string;
            vm.ServerID = psObject.ServerID as string;
            vm.State = psObject.State as string;
            vm.ServerGroup = psObject.ServerGroup as string;
            vm.DeploymentType = psObject.DeploymentType as string;
            vm.flagColor = psObject.flagColor as string;
            vmdetails.Add(vm);

        }
    }
}
