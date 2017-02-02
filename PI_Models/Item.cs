using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class Item
    {
        public int? costcenterID { get; set; }
        public int itemID { get; set; }
        public string sgtin { get; set; }
        public string taggtin14 { get; set; }
        public string tagsn { get; set; }
        public string status { get; set; }
        public DateTime statusTime { get; set; }
        public bool found { get; set; }
        public string changeLocation { get; set; }
        public bool unknownItem { get; set; }
        public string costcentername { get; set; }
    }
}
