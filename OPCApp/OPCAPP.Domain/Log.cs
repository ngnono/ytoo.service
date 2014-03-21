using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPCAPP.Domain
{
    public class Log
    {
        public int Id { get; set; }
        public string Mo { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public string Operation { get; set; }
    }
}
