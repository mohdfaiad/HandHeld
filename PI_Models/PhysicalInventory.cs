using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PI_Abstract;

namespace PI_Models
{
    public class PhysicalInventory: ISelect
    {
        public int PhysicalInventoryID { set; get; }     
        public string LocationCode { set; get; }
        public string LocationName { set; get; }
        public string Status { set; get; }
        public DateTime StartDate { set; get; }
        public DateTime? EndDate { set; get; }
        public int? PhysicalInventoryReviewID { set; get; }
        public string LocationShortName { set; get; }
        public string User { set; get; }

        private bool select;

        public bool Select
        {
            get
            {
                return select;
            }
            set
            {
                select = value;
            }
        }
    }
}
