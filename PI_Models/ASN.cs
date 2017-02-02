using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace PI_Models
{
    public class ASN
    {
        DateTime? shipdate;
        public string ASNNumber { get; set; }
        public DateTime? ShipmentDate
        {
            get
            {
                if (shipdate == null)
                {
                    shipdate = DateTime.Now.AddDays(3);
                }
                return shipdate;
            }

            set
            {
                shipdate = value;

            }
        }
        public string SupplierID { get; set; }
        public string ShipToCustomerID { get; set; } 
        public List<Pallet> Pallets { get; set; }

    }
}
