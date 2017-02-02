using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class InventoryItem
    {
        public int itemID { get; set; }
        public string sgtin { get; set; }
        public string suppcode { get; set; }
        public string suppname { get; set; }
        public string lotnumber { get; set; }
        public DateTime? lotexpiration { get; set; }
        public string taggtin14 { get; set; }
        public string tagsn { get; set; }
        public string shortname { get; set; }
        public string productname { get; set; }
        public decimal qty { get; set; }
        public string status { get; set; }
        public string intransit { get; set; }
    }
}
