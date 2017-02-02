using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class ShipmentLineItem
    {
        public string PONumber { get; set; }
        public string POLineItem { get; set; }
        public string CustomerReference { get; set; }
        public int ProductID { get; set; }
        public string GTIN { get; set; }
        public int QuantityShipped { get; set; }
        public int QuantityOrdered { get; set; }
        public string LotNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public List<IndividualItem> SGTINS { get; set; }

    }
}
