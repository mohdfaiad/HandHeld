using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Testing.Models;

namespace MVC_Testing.Controllers
{
    public class TestingController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }



        public JsonResult getCustomer()
        {
            Customer obj = new Customer();
            obj.CustomerCode = "1001";
            obj.CustomerName = "Shiv";
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [ValidateAntiForgeryToken]
        public ActionResult Transfer()
        {
            // password sending logic will be here
            return Content(Request.Form["amount"] +
                " has been transferred to account "
                + Request.Form["account"]);
        }


    }
}