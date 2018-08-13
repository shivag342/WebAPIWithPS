using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPIWithPS.Commands;
using WebAPIWithPS.Models;
using WebAPIWithPS.PowershellFactory;

namespace WebAPIWithPS.Controllers
{
    public class GetLocalProcessesController : ApiController
    {
        private static ConcurrentDictionary<string, PowerShellExecutor<Processes>> executors =
            new ConcurrentDictionary<string, PowerShellExecutor<Processes>>();
        public GetLocalProcessesController()
        {

        }


        public IEnumerable<Processes> GetLocalProcesses()
        {
            PowerShellExecutor<Processes> executor;
            string key = "";
            if (!GetLocalProcessesController.executors.TryGetValue(key, out executor))
            {
                executor = new PowerShellExecutor<Processes>();
                executor.CmdName = "";
                executor.NewFunc = this.NewFunc;
                executor.Configure = (engine) =>
                {
                    engine.AddScript(AzureCommandResource.getLocalProcesses);
                };
                executor = GetLocalProcessesController.executors.GetOrAdd(key, executor);
            }


            var list = executor.GetList();
            return list;
        }

        public IEnumerable<Processes> GetLocalProcess(int Id)
        {
            PowerShellExecutor<Processes> executor;
            string key = Id.ToString();
            if (!GetLocalProcessesController.executors.TryGetValue(key, out executor))
            {
                executor = new PowerShellExecutor<Processes>();
                executor.CmdName = "";
                executor.NewFunc = this.NewFunc;
                executor.Configure = (engine) =>
                {
                    PowerShellJob.AddVariable(engine, "Id", Id.ToString());
                    engine.AddScript(AWSCommandResource.getSpecificProcess);
                };
                executor = GetLocalProcessesController.executors.GetOrAdd(key, executor);
            }

            var list = executor.GetList();
            return list;
        }

        //This is a Test Method to check if all powershell properties can be returned as json or not
        public async Task<string> ProcessDetails(string Id)
        {
            try
            {
                ICommand commandI = null;

                //string bodyText = await this.Request.Content.ReadAsStringAsync();
                //var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.DeserializeObject(bodyText).ToString());
                commandI = new ScriptCommand(AWSCommandResource.getSpecificProcess, new[] { "Id" });
                Dictionary<string, object> commandData = new Dictionary<string, object>();
                commandData.Add("Id", Id);
                commandI.Init(commandData);
                commandI.Execute();
                if (commandI.Result != null)
                {
                    return JsonConvert.SerializeObject(commandI.Result);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                //ErrorHelper.WriteErrorToEventLog(ex.Message);
                ErrorHelper.SendExcepToDB(ex, "ProcessCommand", Id);
                throw ex;
            }
        }
        public void NewFunc(dynamic psObject, List<Processes> processlist)
        {
            //var users = psObject.UserGroup as string[];

            var ps = new Processes();
            ps.ProcessName = psObject.ProcessName as string;
            ps.Id = psObject.Id;
            //ps.CPU = psObject.CPU;
            //ps.NPM = psObject.NPM;
            //ps.PM = psObject.PM;
            //ps.Handles = psObject.Handles;
            processlist.Add(ps);
         
        }
    }
}
