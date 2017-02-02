using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class Carton
    {
        public string epc { get; set; }
        public List<ShipmentLineItem> LineItems { get; set; }
    }
}
