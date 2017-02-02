using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PI_Models
{
    public class ItemCreation
    {
        public List<Product> products { get; set; }
        public IEnumerable<SelectListItem> locationsList { get; set; }
    }
}
