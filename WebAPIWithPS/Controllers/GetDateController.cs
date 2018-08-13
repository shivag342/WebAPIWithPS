using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPIWithPS.Commands;

namespace WebAPIWithPS.Controllers
{
    public class GetDateController : ApiController
    {
        [HttpGet]
        public async Task<string> GetPSDate()
        {
            try
            {
                ICommand commandI = null;

                //string bodyText = await this.Request.Content.ReadAsStringAsync();
                //var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.DeserializeObject(bodyText).ToString());
                commandI = new ScriptCommand(AWSCommandResource.getProcessNew, new[] {"Id"});
                Dictionary<string, object> commandData = new Dictionary<string, object>();
                commandData.Add("Id", "temp");
                commandI.Init(commandData);
                commandI.Execute();
                if (commandI.Result != null)
                {
                
                    return JsonConvert.SerializeObject(commandI.Result,Formatting.Indented);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                //ErrorHelper.WriteErrorToEventLog(ex.Message);
                ErrorHelper.SendExcepToDB(ex, "ProcessCommand", "GetDate");
                throw ex;
            }
        }
    }
}
