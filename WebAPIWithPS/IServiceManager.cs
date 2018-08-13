namespace WebAPIWithPS
{
    using PowershellFactory;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Management.Automation;

    public interface IServicesManager
    {
        PowerShellJob MakePowerShellJob(Action<PowerShell> configure,
            Action<dynamic> process, Action<Exception> errors = null);

        PowerShellJob MakePowerShellJob<T>(Action<PowerShell> Configure,
           Action<dynamic, List<T>> NewFunc, Func<List<T>, bool> SetList);

        int ExecuteCommand(string sql, params object[] values);

        List<T> ExecuteQuery<T>(string sql, Action<IDataReader, List<T>> process, params object[] values);
    }
}
