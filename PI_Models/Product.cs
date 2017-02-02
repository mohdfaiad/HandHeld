using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;


namespace PI_Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string GTIN { get; set; }
        public int qty { get; set; }
        public string location { get; set; }
        public string CostCenterID { get; set; }
        public IEnumerable<SelectListItem> AllocatedCostCenters { get; set; }
        [MaxLength(50, ErrorMessage = "Max length 50")]
        public string lotnumber { get; set; }

        private DateTime lotexpiry = DateTime.Now.AddDays(60);
        public DateTime lotExpiry { get { return lotexpiry; } set { lotexpiry = value; } }
    }

}
