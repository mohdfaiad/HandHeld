using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class Review
    {
        public string name { get; set; }
        public IList<PhysicalInventory> Scans { get; set; }
    }
}
