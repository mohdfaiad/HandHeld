using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PI_Models
{
    public class Handheld
    {
        public List<Location> locations { get; set; }
        public IList<SelectListItem> locationsList { get; set; }
        public List<Product> products { get; set; }
        public string htmlResults { get; set; }
    }
}
