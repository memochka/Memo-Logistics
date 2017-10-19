using Memo_Logistics.DAL;
using Memo_Logistics.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Memo_Logistics.Controllers
{
    public class HomeController : Controller
    {
        private LogisticsContext db = new LogisticsContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            IQueryable<OrderDateGroup> data = from customers in db.Customers
                       group customers by customers.OrderDate into dateGroup
                       select new OrderDateGroup()
                       {
                           OrderDate = dateGroup.Key,
                           CustomersCount = dateGroup.Count()
                       };
            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}