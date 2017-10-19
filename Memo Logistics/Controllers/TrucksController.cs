using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Memo_Logistics.DAL;
using Memo_Logistics.Models;
using System.Data.Entity.Infrastructure;
using PagedList;

namespace Memo_Logistics.Controllers
{
    public class TrucksController : Controller
    {
        private LogisticsContext db = new LogisticsContext();

        // GET: Trucks
        public ViewResult Index(int? SelectedCustomers,int? searchString, string sortOrder, int? currentFilter, int? page)

        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NumberSortParm = sortOrder == "NumberAsc" ? "NumberDesc" : "NumberAsc";
            ViewBag.TruckSortParm = sortOrder == "TruckAsc" ? "TruckDesc" : "TruckAsc";
            ViewBag.MilageSortParm = sortOrder == "MilageArAsc" ? "MilageArDesc" : "MilageArAsc";
            ViewBag.CustomerSortParm = sortOrder == "CustomerAsc" ? "CustomerDesc" : "CustomerAsc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var customers = db.Customers.OrderBy(q => q.Name).ToList();
            ViewBag.SelectedCustomers = new SelectList(customers, "CustomerID", "Name", SelectedCustomers);
            int customerID = SelectedCustomers.GetValueOrDefault();

            IQueryable<Trucks> trucks = db.Trucks
                .Where(c => !SelectedCustomers.HasValue || c.CustomerID == customerID)
                .OrderBy(d => d.TruckID)
                .Include(d => d.Customer);
            var sql = trucks.ToString();


            if (searchString != null)
            {
                trucks = trucks.Where(n => n.TruckID == searchString);
            }
            
            

            switch (sortOrder)
            {
                case "NumberDesc":
                    trucks = trucks.OrderByDescending(s => s.TruckID);
                    break;
                case "TruckAsc":
                    trucks = trucks.OrderBy(s => s.Truck);
                    break;
                case "TruckDesc":
                    trucks = trucks.OrderByDescending(s => s.Truck);
                    break;
                case "MilageArAsc":
                    trucks = trucks.OrderBy(s => s.Mileage);
                    break;
                case "MilageArDesc":
                    trucks = trucks.OrderByDescending(s => s.Mileage);
                    break;
                case "CustomerAsc":
                    trucks = trucks.OrderBy(s => s.Customer.Name);
                    break;
                case "CustomerDesc":
                    trucks = trucks.OrderByDescending(s => s.Customer.Name);
                    break;
                default:
                    trucks = trucks.OrderBy(s => s.TruckID);
                    break;
            }


            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(trucks.ToPagedList(pageNumber, pageSize));
        }

        // GET: Trucks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trucks trucks = db.Trucks.Find(id);
            if (trucks == null)
            {
                return HttpNotFound();
            }
            return View(trucks);
        }

        // GET: Trucks/Create
        public ActionResult Create()
        {
            PopulateCustomersDropDownList();
            return View();
        }

        // POST: Trucks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TruckID,Truck,Mileage,CustomerID")] Trucks trucks)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Trucks.Add(trucks);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            PopulateCustomersDropDownList(trucks.CustomerID);

            return View(trucks);
        }

        // GET: Trucks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trucks trucks = db.Trucks.Find(id);
            if (trucks == null)
            {
                return HttpNotFound();
            }
            PopulateCustomersDropDownList(trucks.CustomerID);

            return View(trucks);
        }

        // POST: Trucks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var trucksToUpdate = db.Trucks.Find(id);
            if (TryUpdateModel(trucksToUpdate, "",
               new string[] { "Truck", "Mileage", "CustomerID" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateCustomersDropDownList(trucksToUpdate.CustomerID);
            return View(trucksToUpdate);
        }

        private void PopulateCustomersDropDownList(object selectedCustomers = null)
        {
            var customersQuery = from d in db.Customers
                                   orderby d.Name
                                   select d;
            ViewBag.CustomerID = new SelectList(customersQuery, "CustomerID", "Name", selectedCustomers);
        }
        

        // GET: Trucks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trucks trucks = db.Trucks.Find(id);
            if (trucks == null)
            {
                return HttpNotFound();
            }
            return View(trucks);
        }

        // POST: Trucks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Trucks trucks = db.Trucks.Find(id);
            db.Trucks.Remove(trucks);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
