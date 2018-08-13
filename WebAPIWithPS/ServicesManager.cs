namespace WebAPIWithPS
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Management.Automation;
    using PowershellFactory;
    using WebAPIWithPS.Commands;

    public class ServicesManager : IServicesManager
    {
        static IServicesManager instance = null;

        static object lockObject = new object();

        private static Type ServicesManagerType = null;

        public static void SetServicesManagerType<T>() where T : IServicesManager
        {
            lock(lockObject)
            {
                ServicesManager.ServicesManagerType = typeof(T);
                ServicesManager.instance = null;
            }
        }

        public static IServicesManager Instance
        {
            get
            {
                IServicesManager currentInstance = ServicesManager.instance;
                if (currentInstance == null)
                {
                    lock(lockObject)
                    {
                        if(ServicesManager.instance == null)
                        {
                            var managerType = ServicesManager.ServicesManagerType;
                            if (managerType != null)
                            {
                                ServicesManager.instance =
                                    Activator.CreateInstance(managerType) as IServicesManager;
                            }else
                            {
                                ServicesManager.instance = new ServicesManager();
                            }
                        }

                        currentInstance = ServicesManager.instance;
                    }
                }
            
                return currentInstance;
            }
        }

        public PowerShellJob MakePowerShellJob(Action<PowerShell> configure,
                Action<dynamic> process, Action<Exception> errors = null)
        {
            return PowerShellJob.MakePowerShellJob(configure, process, errors);
        }

        public PowerShellJob MakePowerShellJob<T>(Action<PowerShell> Configure,
           Action<dynamic, List<T>> NewFunc, Func<List<T>, bool> SetList)
        {
            return PowerShellJob.MakePowerShellJob<T>(Configure, NewFunc, SetList);
        }

        public int ExecuteCommand(string sql, params object[] values)
        {
            return DBHelper.ExecuteCommand(sql, values);
        }

        public List<T> ExecuteQuery<T>(string sql, Action<IDataReader, List<T>> process, params object[] values)
        {
            return DBHelper.ExecuteQuery<T>(sql, process, values);
        }
    }
}
