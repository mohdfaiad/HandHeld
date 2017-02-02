using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PI_DAL;

namespace PI_MVC.Controllers
{
    public class ReviewController : Controller
    {
        // GET: Review
        public ActionResult Index()
        {
            return View();
        }

        // GET: Review
        [HttpGet]
        public ActionResult Create()
        {
            DAL dal = new DAL();
            var model = new PI_Models.Review();

            model.Scans = dal.GetScansFoReviw();
            model.name = "Rich Test";
            return View(model);
        }





    }
}