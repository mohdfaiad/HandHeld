
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Models;
using PI_DAL;

namespace PI_MVC.Controllers
{
    public class HandheldController : Controller
    {
        // GET: Handheld
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Load()
        {
            PI_DAL.DAL dal = new PI_DAL.DAL();
            IEnumerable<InventoryItem> model = dal.GetInventoryItemsByLocation("Cooler");
            return View(model);
        }


        public ActionResult Locations()
        {

            if (TempData.ContainsKey("lastloc"))
            {
                try
                {
                    ViewBag.locationcode = TempData["lastloc"].ToString();
                }
                catch
                {

                }
            }

            PI_DAL.DAL dal = new PI_DAL.DAL();
            IEnumerable<Location> model = dal.GetLocations(false);
            return View(model);
        }


        public ActionResult HandHeld()
        {
            Handheld model = new Handheld();
            PI_DAL.DAL dal = new PI_DAL.DAL();
            model = dal.GetHandHeld(1000);
            return View(model);
        }

        [HttpPost]
        public ActionResult HandHeld(Handheld model)
        {
            BooleanPlus bp = new BooleanPlus();

            if (ModelState.IsValid)
            {

                DAL dal = new DAL();
                var found = dal.GetFoundItems(model);

                foreach (var x in model.locations)
                {
                    if (x.Select && x.Skip != true)
                    {
                        bp = dal.StartPhysicalInventory(x.locationCode);
                        dal.CompletePhysicalInventory(bp.ID, x.locationCode, found);
                    }
                }


                var zz = dal.UpdateCounterPI();
                model = dal.GetHandHeld(10000);
                return View(model);
            }

            else
            {

                return View(model);
            }


        }


        [HttpGet]
        public ActionResult LocationItems(string locationCode)
        {
            PI_DAL.DAL dal = new PI_DAL.DAL();
            HandHeldLocation model = dal.GetHandHeldForLocation(locationCode);
            ViewBag.records = model.location.items.Count;
            return View(model);
        }


        [HttpPost]
        public ActionResult LocationItems(HandHeldLocation model)
        {

            if (ModelState.IsValid)
            {
                var x = model;

                var z = x.location.items;
                TempData["lastloc"] = model.location.locationCode;

                return RedirectToAction("Locations");
            }
            else
            {

                return View(model);
            }



        }


        public ActionResult Stock(string locationCode)
        {

            //return RedirectToAction("Stock", new { LocationCode = x.locationCode });


            PI_DAL.DAL dal = new PI_DAL.DAL();

            IEnumerable<InventoryItem> model = from stock in dal.GetInventoryItemsByLocation(locationCode) where (stock.intransit == null || stock.intransit == "N") && (stock.status == "Pending" || stock.status == "Stock") orderby stock.itemID select stock;

            ViewBag.Count = model.Count();

            return View(model);

        }

    }
}