using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIWithPS.Models
{
    public class Processes
    {
        public String ProcessName {get; set;}
        public int Id { get; set; }
        public float CPU { get; set; }
        public int WS { get; set; }
        public int PM { get; set; }
        public int Handles { get; set; }
        public int NPM { get; set; }
    }
}