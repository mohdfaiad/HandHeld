using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace PI_Models
{
    public class HandHeldLocation
    {
        public Location location { get; set; }
        public IList<SelectListItem> locationsList { get; set; }
    }
}
