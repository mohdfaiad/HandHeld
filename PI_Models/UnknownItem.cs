using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class UnknownItem
    {
        public string gtin { get; set; }
        public string sgtin { get; set; }
        public string serialnumber { get; set; }
        public string productname { get; set; }
        public bool IsValid { get; set; }
    }
}
