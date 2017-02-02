using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using PI_Models;
using PI_DAL;
 
 


namespace PI_Api.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PhysicalInventoryController : ApiController
    {

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values
        [ActionName("Items")]
        public IEnumerable<PhysicalInventory> GetPhysicalInventoryItems()
        {
            DAL dal = new DAL();

            var ret = dal.GetPhysicalInventories();

            return ret;

            // return new string[] { "value4", "value5" };
        }

        // GET api/AddItem
        [ActionName("AddItem")]
        [HttpPost]
        public string AddPhysicalInventoryItems(string msg)
        {
            DAL dal = new DAL();
            var ret = "nothing";
            return ret;
        }

    }
}
