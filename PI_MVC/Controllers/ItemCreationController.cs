using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_Models;
using PI_DAL;

namespace PI_MVC.Controllers
{
    public class ItemCreationController : Controller
    {
        // GET: ItemCreation
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Create()
        {
            DAL dal = new DAL();
            ItemCreation model = dal.GetItemCreation(108);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ItemCreation model)
        {
            DAL dal = new DAL();

            if (ModelState.IsValid)
            {
                dal.CreateItems(model);
            }

            model = dal.GetItemCreation(108);
            return View(model);

        }



    }
}