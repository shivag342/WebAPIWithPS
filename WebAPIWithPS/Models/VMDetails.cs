using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIWithPS.Models
{
    public class VMDetails
    {
        public string ResourceGroupName { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string VmSize { get; set; }
        public string OsType { get; set; }
        public string PowerState { get; set; }
        public string NIC { get; set; }
    }
}