using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebAPIWithPS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            //// Web API routes
            //config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Routes.MapHttpRoute(
             name: "GetLocalProcesses",
             routeTemplate: "api/GetLocalProcesses",
             defaults: new { controller = "GetLocalProcesses" });
            //config.Routes.MapHttpRoute(
            // name: "GetLocalProcess",
            // routeTemplate: "api/GetLocalProcess/{Id}",
            // defaults: new { controller = "GetLocalProcesses" });
            config.Routes.MapHttpRoute(
             name: "GetLocalProcessNew",
             routeTemplate: "api/ProcessDetails/{Id}",
             defaults: new { controller = "GetLocalProcesses" });
            config.Routes.MapHttpRoute(
         name: "GetPSDate",
         routeTemplate: "api/GetPSDate",
         defaults: new { controller = "GetDate" });

        config.Routes.MapHttpRoute(
        name: "GetAzureVMDetails",
        routeTemplate: "api/GetAzureVMDetails",
        defaults: new { controller = "VMDetails" });

            //Settings Section
            //Manage Cloud Accounts Save to CSV

            //Get Supported Cloud Accounts from Csv

            //Edit

            //Dashboard

            //Get Server Status for Accounts in csv file
            config.Routes.MapHttpRoute(
            name: "GetServerStatus",
            routeTemplate: "api/GetServerStatus/{AccountType}/{AccountID}",
            defaults: new { controller = "ServerState" });
            //Get Sql Server status for Accounts in csv file
            config.Routes.MapHttpRoute(
            name: "GetSQLServerStatus",
            routeTemplate: "api/GetSQLServerStatus/{AccountType}/{AccountID}",
            defaults: new { controller = "SQLServerState" });
            //Get Url status for url's in csv file
            config.Routes.MapHttpRoute(
            name: "GetURLStatus",
            routeTemplate: "api/GetURLStatus/{AccountType}/{AccountID}",
            defaults: new { controller = "URLState" });
            //Get CPU Utilization for given servers in csv
            config.Routes.MapHttpRoute(
            name: "GetServerUtilization",
            routeTemplate: "api/GetServerUtilization/{AccountType}/{AccountID}",
            defaults: new { controller = "ServerUtilization" });
            //Get Billing details of cloud account present in csv
            config.Routes.MapHttpRoute(
            name: "GetAccountBilling",
            routeTemplate: "api/GetAccountBilling/{AccountType}/{AccountID}",
            defaults: new { controller = "AccountBilling" });
            //some other
















        }
    }
}
