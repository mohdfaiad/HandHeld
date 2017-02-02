using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI_Models
{
    public class Found
    {

        public Found()
        {
            unkownItem = new UnknownItem();
            unkownItem.IsValid = false;
        }

        public string SGTIN { get; set; }
        public string LocationCode { get; set; }
        public string LastLocation { get; set; }
        public UnknownItem unkownItem { get; set; }
    }
}
