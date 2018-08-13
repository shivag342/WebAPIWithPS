using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIWithPS.Models
{
    public class ServerState
    {
        public string AccountID { get; set; }
        public string AccountType { get; set; }
        public string ServerName { get; set; }
        public string ServerID { get; set; }
        public string State { get; set; }
        public string ServerGroup { get; set; }
        public string DeploymentType { get; set; }
        public string flagColor { get; set; }
    }
}