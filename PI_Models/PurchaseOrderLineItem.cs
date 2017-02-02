using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class PurchaseOrderLineItem
    {

        private DateTime lotExpiration = DateTime.Now.AddMonths(3);
        
        public int OrderLineItemID { get; set; }
        public string LineItemNumber { get; set; }
        public int QuantityOrdered { get; set; }
        public int? QuantityAccepted { get; set; }
        public string Status { get; set; }
        public bool Select { get; set; }
        public int ASNQty { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string GTIN { get; set; }
        public string SupplierID { get; set; }
        public string LotNumber { get; set; }
        public DateTime LotExpiration
        {
            get
            {
                if (lotExpiration == null)
                {
                    lotExpiration = DateTime.Now.AddMonths(3);
                }
                return lotExpiration;
            }

            set
            {
                lotExpiration = value;

            }
        }

    }
}
