namespace WebAPIWithPS.Commands
{
    using PowershellFactory;
    using System;
    using System.Collections.Generic;

    public class ScriptCommand : ICommand
    {
        private Dictionary<string, object> data;
        private string script;
        private Action<dynamic, ScriptCommand> process;
        private string[] paramNames;

        public ScriptCommand(string script, string[] paramNames, Action<dynamic, ScriptCommand> process = null)
        {
            this.script = script;
            this.process = process;
            this.paramNames = paramNames;
        }

        public object Result { get; set; }

        public Action<Dictionary<string, object>> PreProcessData { get; set; }

        public void Init(Dictionary<string, object> data)
        {
            this.data = data;
        }

        public string Execute()
        {
            string error = "unknown";
            var pShell = ServicesManager.Instance.MakePowerShellJob((engine) =>
            {
                foreach (var paramName in paramNames)
                {
                    PowerShellJob.AddVariable(engine, paramName, data);
                }
                engine.AddScript(this.script);
            }, (data) =>
            {
                if(this.process != null)
                {
                    this.process(data, this);
                }
                this.Result = data; //added to store the resultant data to Result previous was not storing
            }, (ex) =>
            {
                error = ex.Message;
            });

            JobManager.Instance.Execute(pShell);
            if (!pShell.Success)
            {
                throw new Exception("Error executing command:" + error);
            }

            return string.Empty;
        }

        internal void Init(object data)
        {
            throw new NotImplementedException();
        }
    }
}