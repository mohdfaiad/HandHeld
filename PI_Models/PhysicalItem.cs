using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PI_Models
{
    public class PhysicalItem
    {
        public string SGTIN { set; get; }
        public string Status { set; get; }
        public string DispositionStatus { set; get; }   
        public string DispositionParm { set; get; }     
        public string NewLocationCode { set; get; }
        public string Overridden { set; get; }
        public Int32 PhysicalItemID { set; get; }
        public string DiscrepancyNewLocation { set; get; }
        public string ScanItemStatus { set; get; }
        public string Discrepancy { set; get; }
        public string Processed { set; get; }

    }
}
