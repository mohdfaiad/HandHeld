using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class RawScanItem
    {
        public int ORIGREC { get; set; }
        public int PhysicalInventoryID { get; set; }
        public string SGTIN { get; set; }
        public string LocationCode { get; set; }
        public DateTime Created { get; set; }       
    }
}
