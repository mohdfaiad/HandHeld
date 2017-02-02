using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class Pallet
    {
        public string epc { get; set; }
        public List<Carton> Cartons { get; set; }
    }
}
