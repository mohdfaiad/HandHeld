using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace PI_Models
{
    public class AdvancedShipingNotice
    {

        public bool XmlFileOutput { get; set; }   
        public bool DirectInsert { get; set; }
        //public IEnumerable<SelectListItem> Suppliers { get; set; }
        public IEnumerable<SelectListItem> ShipToCustomers { get; set; }
        public List<PurchaseOrder> PurchaseOrders { get; set; }
    }
}
