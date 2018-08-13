using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Web;

namespace WebAPIWithPS.PowershellFactory
{
    public class PowerShellJob
    {
        private bool success = false;

        private Func<RunspacePool, bool> execute;

        public Func<RunspacePool, bool> Execute
        {
            get
            {
                return (pool) =>
                {
                    bool retVal = this.execute(pool);
                    this.success = retVal;
                    this.resetEvent.Set();
                    return retVal;
                };
            }
        }

        public bool ReOccuring { get; private set; }

        public bool Done { get; set; }

        public bool Success
        {
            get
            {
                this.resetEvent.WaitOne();
                return this.success;
            }
        }

        public ManualResetEvent resetEvent = new ManualResetEvent(false);

        public PowerShellJob(Func<RunspacePool, bool> execute, bool reoccuring = false)
        {
            this.execute = execute;
            //this.ReOccuring = reoccuring;
            this.ReOccuring = false;
        }

        public static PowerShellJob MakePowerShellJob<T>(Action<PowerShell> Configure,
           Action<dynamic, List<T>> NewFunc, Func<List<T>, bool> SetList)
        {
            return new PowerShellJob((runspacePool) =>
            {
                try
                {
                    var curList = new List<T>();
                    using (var localRunspacePool = RunspaceFactory.CreateRunspacePool())
                    {
                        localRunspacePool.Open();
                        using (var engine = PowerShell.Create())
                        {
                            engine.RunspacePool = localRunspacePool;
                            engine.AddScript(@"");
                            //engine.Commands.AddCommand("Import-Module").AddArgument("ActiveDirectory");
                            var importResult = engine.BeginInvoke();
                            var result = engine.EndInvoke(importResult);
                            Configure(engine);
                            var invokeResult = engine.BeginInvoke();
                            var commandIter = engine.EndInvoke(invokeResult);

                            if (engine.Streams.Error.Count > 0)
                            {
                                ErrorHelper.WriteErrorToEventLog(PowerShellJob.GetErrorMessage(engine.Streams.Error));
                                throw new Exception(PowerShellJob.GetErrorMessage(engine.Streams.Error));
                            }

                            foreach (dynamic psObject in commandIter)
                            {
                                NewFunc(psObject, curList);
                            }
                        }
                    }

                    return SetList(curList);

                }
                catch (Exception ex)
                {
                    ErrorHelper.WriteErrorToEventLog(ex.Message);
                }

                return false;
            }, true);
        }

        public static PowerShellJob MakePowerShellJob(Action<PowerShell> configure,
                Action<dynamic> process, Action<Exception> errors = null)
        {
            var pShell = new PowerShellJob((runspacePool) =>
            {
                try
                {
                    using (var localRunspacePool = RunspaceFactory.CreateRunspacePool())
                    {
                        localRunspacePool.Open();
                        using (var engine = PowerShell.Create())
                        {
                            engine.RunspacePool = localRunspacePool;
                            engine.AddScript(@"");
                            //engine.AddScript(@"Import-Module RemoteDesktop");
                            //engine.Commands.AddCommand("Import-Module").AddArgument("ActiveDirectory");
                            var importResult = engine.BeginInvoke();
                            var result = engine.EndInvoke(importResult);
                            configure(engine);

                            var invokeResult = engine.BeginInvoke();
                            var commandIter = engine.EndInvoke(invokeResult);

                            if (engine.Streams.Error != null &&
                                engine.Streams.Error.Count > 0)
                            {
                                ErrorHelper.WriteErrorToEventLog(PowerShellJob.GetErrorMessage(engine.Streams.Error));
                                throw new Exception(PowerShellJob.GetErrorMessage(engine.Streams.Error));
                            }

                            foreach (dynamic psObject in commandIter)
                            {
                                if (process != null)
                                {
                                    process(psObject);
                                }
                            }
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    ErrorHelper.WriteErrorToEventLog(ex.Message);
                    if (errors != null)
                    {
                        errors(ex);
                    }
                }

                return false;
            });

            pShell.ReOccuring = false;
            return pShell;
        }

        public static void AddVariable(PowerShell engine, string name, string value)
        {
            engine.AddCommand("Set-Variable");
            engine.AddParameter("Name", name);
            engine.AddParameter("Value", value);
        }

        public static void AddVariable(PowerShell engine, string name, Dictionary<string, object> value)
        {
            engine.AddCommand("Set-Variable");
            engine.AddParameter("Name", name);
            engine.AddParameter("Value", value[name]);
        }

        public static string GetErrorMessage(PSDataCollection<ErrorRecord> errors)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var error in errors)
            {
                builder.AppendLine(error.Exception.ToString());
            }

            return builder.ToString();
        }
    }
}
