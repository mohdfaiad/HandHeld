using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Models;
using PI_DAL;

namespace PI_MVC.Controllers
{
    public class ASNController : Controller
    {
        // GET: ASN
        public ActionResult Index()
        {
            return View();
        }

        // GET: ASN
        public ActionResult Create()
        {
            AdvancedShipingNotice model = new AdvancedShipingNotice();
            PI_DAL.DAL dal = new PI_DAL.DAL();
            model = dal.CreateASN();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AdvancedShipingNotice model)
        {

            BooleanPlus bp = new BooleanPlus();
            DAL dal = new DAL();

            int counter = 0;

            if (ModelState.IsValid)
            {
                dal.CreateSerialNumberDictiony();

                foreach (var po in model.PurchaseOrders.Where(x => x.Select == true))
                {
                    dal.ReceiveASN(po, model);
                    counter++;
                }
            }


            model = new AdvancedShipingNotice();
            model = dal.CreateASN();
            return View(model);
        }



    }
}