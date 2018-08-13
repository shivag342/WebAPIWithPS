using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPIWithPS.Commands;
using WebAPIWithPS.Models;
using WebAPIWithPS.PowershellFactory;

namespace WebAPIWithPS.Controllers
{
    public class VMDetailsController : ApiController
    {
        private static ConcurrentDictionary<string, PowerShellExecutor<VMDetails>> executors =
          new ConcurrentDictionary<string, PowerShellExecutor<VMDetails>>();

        public IEnumerable<VMDetails> GetAzureVMDetails()
        {
            //Connect to DB and get the Creds
            string stm = @"Select SubscriptionId, Username, Password from kloudLoginDetails";
            DataTable dt= DBHelper.ExecuteAndGetDataTable(stm);
            string SubscriptionId = "";
            string Username = "";
            string Password = "";
            if(dt.Rows.Count == 1) { 
            SubscriptionId = dt.Rows[0]["SubscriptionId"] as string;
                Username = dt.Rows[0]["Username"] as string;
                Password = dt.Rows[0]["Password"] as string;

            }
            PowerShellExecutor<VMDetails> executor;
            string key = "";
            if (!VMDetailsController.executors.TryGetValue(key, out executor))
            {
                executor = new PowerShellExecutor<VMDetails>();
                executor.CmdName = "";
                executor.NewFunc = this.NewFunc;
                executor.Configure = (engine) =>
                {
                    PowerShellJob.AddVariable(engine, "SubscriptionId", SubscriptionId);
                    PowerShellJob.AddVariable(engine, "Username", Username);
                    PowerShellJob.AddVariable(engine, "Password", Password);
                    engine.AddScript(AzureCommandResource.getAzureVMDetails);
                };
                executor = VMDetailsController.executors.GetOrAdd(key, executor);
            }


            var list = executor.GetList();
            return list;
        }
        public void NewFunc(dynamic psObject, List<VMDetails> vmdetails)
        {
            //var users = psObject.UserGroup as string[];

            var vm = new VMDetails();
            vm.ResourceGroupName= psObject.ResourceGroupName as string;
            vm.Name = psObject.Name as string;
            vm.Location = psObject.Location as string;
            vm.VmSize = psObject.VmSize as string;
            vm.OsType = psObject.OsType as string;
            vm.PowerState = psObject.PowerState as string;
            vmdetails.Add(vm);

        }
    }
}
