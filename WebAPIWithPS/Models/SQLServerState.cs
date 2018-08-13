using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIWithPS.Models
{
    public class SQLServerState
    {
        public string AccountID { get; set; }
        public string AccountType { get; set; }
        public string DBServerName { get; set; }
        public string DataBaseName { get; set; }
        public string EndpointAddress { get; set;}
        public string State { get; set; }
        public string ServerGroup { get; set; }
        public string DeploymentType { get; set; }
        public string flagColor { get; set; }
    }
}