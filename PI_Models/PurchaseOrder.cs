using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class PurchaseOrder
    {
        public bool Select { get; set; }
        public int PurchaseOrderID { get; set; }
        public string PONumber { get; set; }
        public string POCustomerRef { get; set; }
        public string Status { get; set; }
        public DateTime ApprovalDate { get; set; }
        public int CustomerID { get; set; }
        public int CostCenterID { get; set; }
        public string CostCenterName { get; set; }
        public string Site { get; set; }
        public List<PurchaseOrderLineItem> LineItems { get; set; }
        public ASN asn { get; set; }
    }
}
