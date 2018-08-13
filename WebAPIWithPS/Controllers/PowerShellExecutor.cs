//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace WebAPIWithPS.Controllers
{
    using PowershellFactory;
    using System;
    using System.Collections.Generic;
    using System.Management.Automation;

    public class PowerShellExecutor<T>
    {
        public List<T> list;

        public object listLock = new object();

        public string CmdName { get; set;  }

        public Action<dynamic, List<T>> NewFunc { get; set; }

        public Action<PowerShell> Configure { get; set; }

        public DateTime lastCall = DateTime.UtcNow;

        private static int maxMilliseconds = 1000 * 60 * 5;

        private bool started = false;

        public List<T> GetList()
        {
            var list = this.list;
            if (!this.started)
            {
                lock (listLock)
                {
                    if (!this.started)
                    {
                        this.StartProcessPull();
                        // this.started = true; //commented to execute a new request on each service call and added below statement
                        this.started = false;
                     
                    }
                }

                list = this.list;
            }

            this.lastCall = DateTime.UtcNow;
            return list;
        }

        private void StartProcessPull()
        {
            PowershellFactory.JobManager.Instance.Execute(ServicesManager.Instance.MakePowerShellJob<T>(this.Configure, this.NewFunc, 
                (curList) =>
                {
                    this.list = curList;

                    var diff = DateTime.Now.Subtract(this.lastCall).TotalMilliseconds;
                    //if (diff > PowerShellExecutor<T>.maxMilliseconds)
                    //{
                    //    this.list = null;
                    //    this.started = false;
                    //    return true;
                    //}

                    return false;
                }));
        }
    }
}
