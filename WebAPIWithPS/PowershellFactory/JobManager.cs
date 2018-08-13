using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebAPIWithPS.PowershellFactory
{
    public class JobManager
    {
        private bool done = false;

        private static int delayInMilliSeconds = 1000;

        private static JobManager instance;

        private static object instanceLock = new object();

        private RunspacePool runspacePool = null;

        private string minRunspacePool = System.Configuration.ConfigurationManager.AppSettings["MinRunspacepool"];

        private string maxRunspacePool = System.Configuration.ConfigurationManager.AppSettings["MaxRunspacepool"];

        public static JobManager Instance
        {
            get
            {
                if (JobManager.instance == null)
                {
                    lock (JobManager.instanceLock)
                    {
                        if (JobManager.instance == null)
                        {
                            JobManager.instance = new JobManager();
                            JobManager.instance.Start();
                        }
                    }
                }

                return JobManager.instance;
            }
        }

        public void Start()
        {
            int minPoolsize = 5;
            int maxPoolsize = 20;
            if (!string.IsNullOrEmpty(minRunspacePool))
            {
                int.TryParse(minRunspacePool, out minPoolsize);
            }

            if (!string.IsNullOrEmpty(maxRunspacePool))
            {
                int.TryParse(maxRunspacePool, out maxPoolsize);
            }

            this.done = false;
            // this.runspacePool = RunspaceFactory.CreateRunspacePool(minPoolsize, maxPoolsize);
            // this.runspacePool.Open();
        }

        internal void Execute(object p)
        {
            throw new NotImplementedException();
        }

        public void RequestStop()
        {
            this.done = true;
        }

        public void Execute(PowerShellJob job)
        {
            if (job != null)
            {
                if (job.ReOccuring)
                {
                    ExecuteContinuous(job);
                }
                else
                {
                    ExecuteNow(job);
                }
            }
        }

        private void ExecuteContinuous(PowerShellJob job)
        {
            Task.Run(() =>
            {
                do
                {
                    job.Done = job.Execute(runspacePool);
                    if (job.Done)
                    {
                        return;
                    }

                    Thread.Sleep(JobManager.delayInMilliSeconds);
                } while (!job.Done || this.done);
            });
        }

        private void ExecuteNow(PowerShellJob job)
        {
            if (!this.done)
            {
                job.Execute(runspacePool);
            }
        }

        ~JobManager()
        {
            if (this.runspacePool != null)
                this.runspacePool.Dispose();
        }

    }
}
