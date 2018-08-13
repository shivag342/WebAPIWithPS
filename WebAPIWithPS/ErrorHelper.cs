using System;
using WebAPIWithPS.Commands;

namespace WebAPIWithPS
{

    public static class ErrorHelper
    {
        private static string eventSource = "WebAPIWithPSEvents";
        private static int errorEventID = 255;

        public static void WriteErrorToEventLog(string message)
        {
            System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(ErrorHelper.eventSource))
            {
                System.Diagnostics.EventLog.CreateEventSource(ErrorHelper.eventSource, "Application");
            }

            eventLog.Source = ErrorHelper.eventSource;
            eventLog.WriteEntry(message.Replace(" Check WebAPIWithEvents source events in Windows Application Event Logs on the WAP Admin API Server(s) for details.", string.Empty),
                                System.Diagnostics.EventLogEntryType.Error,
                                ErrorHelper.errorEventID);
            eventLog.Close();
        }

        public static void WriteInfoToEventLog(string message)
        {
            System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(ErrorHelper.eventSource))
            {
                System.Diagnostics.EventLog.CreateEventSource(ErrorHelper.eventSource, "Application");
            }

            eventLog.Source = ErrorHelper.eventSource;
            eventLog.WriteEntry(message,
                                System.Diagnostics.EventLogEntryType.Information,
                                ErrorHelper.errorEventID);
            eventLog.Close();
        }

        public static void SendExcepToDB(Exception ex, string methodName, string Username)
        {
            try
            {
                string stm = @"INSERT INTO ErrorLogs(Datetime, Methodname, ErrorMessage, Username) values(@Datetime,@Methodname,@ErrorMessage,@Username)";
                DBHelper.ExecuteCommand(stm, DateTime.Now, methodName,  ex.ToString(), Username);
            }
            catch (Exception exe)
            {
                ErrorHelper.WriteErrorToEventLog(exe.Message);
                throw exe;
            }
        }
    }
           
            
    }
