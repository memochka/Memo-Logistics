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
using Memo_Logistics.ViewModels;
using PagedList;

namespace Memo_Logistics.Controllers
{
    public class DriversController : Controller
    {
        private LogisticsContext db = new LogisticsContext();

        // GET: Drivers
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "NameAsc" ? "NameDesc" : "NameAsc";
            ViewBag.DateSortParm = sortOrder == "DateAsc" ? "DateDesc" : "DateAsc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var drivers = from s in db.Drivers
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                drivers = drivers.Where(s => s.LastName.Contains(searchString)
                                             || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "NameDesc":
                    drivers = drivers.OrderByDescending(s => s.LastName);
                    break;
                case "DateAsc":
                    drivers = drivers.OrderBy(s => s.HireDate);
                    break;
                case "DateDesc":
                    drivers = drivers.OrderByDescending(s => s.HireDate);
                    break;
                default:
                    drivers = drivers.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(drivers.ToPagedList(pageNumber, pageSize));


        }



        // GET: Drivers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Drivers drivers = db.Drivers.Find(id);
            if (drivers == null)
            {
                return HttpNotFound();
            }
            return View(drivers);
        }

        // GET: Drivers/Create
        public ActionResult Create()
        {

            var drivers = new Drivers();
            drivers.Trucks = new List<Trucks>();
            PopulateAssignedTrucksData(drivers);
            return View();
        }

        // POST: Drivers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,HireDate,OfficeAssignment")]Drivers drivers, string[] selectedTrucks)
        {
            if (selectedTrucks != null)
            {
                drivers.Trucks = new List<Trucks>();
                foreach (var course in selectedTrucks)
                {
                    var trucksToAdd = db.Trucks.Find(int.Parse(course));
                    drivers.Trucks.Add(trucksToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                db.Drivers.Add(drivers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateAssignedTrucksData(drivers);
            return View(drivers);
        }


        // GET: Drivers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Drivers drivers = db.Drivers.Find(id);
            Drivers drivers = db.Drivers
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Trucks)
                .Where(i => i.ID == id)
                .Single();
            PopulateAssignedTrucksData(drivers);
            if (drivers == null)
            {
                return HttpNotFound();
            }
            return View(drivers);
        }

        private void PopulateAssignedTrucksData(Drivers drivers)
        {
            var allTrucks = db.Trucks;
            var driversTrucks = new HashSet<int>(drivers.Trucks.Select(c => c.TruckID));
            var viewModel = new List<AssignedTrucksData>();
            foreach (var trucks in allTrucks)
            {
                viewModel.Add(new AssignedTrucksData
                {
                    TruckID = trucks.TruckID,
                    Truck = trucks.Truck,
                    Assigned = driversTrucks.Contains(trucks.TruckID)
                });
            }
            ViewBag.Trucks = viewModel;
        }

        // POST: Drivers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedTrucks)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var driversToUpdate = db.Drivers
               .Include(i => i.OfficeAssignment)
               .Include(i => i.Trucks)
               .Where(i => i.ID == id)
               .Single();

            if (TryUpdateModel(driversToUpdate, "",
               new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(driversToUpdate.OfficeAssignment.Location))
                    {
                        driversToUpdate.OfficeAssignment = null;
                    }

                    UpdateDriversTrucks(selectedTrucks, driversToUpdate);

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            PopulateAssignedTrucksData(driversToUpdate);
            return View(driversToUpdate);
        }

        private void UpdateDriversTrucks(string[] selectedTrucks, Drivers driversToUpdate)
        {
            if (selectedTrucks == null)
            {
                driversToUpdate.Trucks = new List<Trucks>();
                return;
            }

            var selectedTrucksHS = new HashSet<string>(selectedTrucks);
            var driversTrucks = new HashSet<int>
                (driversToUpdate.Trucks.Select(c => c.TruckID));
            foreach (var trucks in db.Trucks)
            {
                if (selectedTrucksHS.Contains(trucks.TruckID.ToString()))
                {
                    if (!driversTrucks.Contains(trucks.TruckID))
                    {
                        driversToUpdate.Trucks.Add(trucks);
                    }
                }
                else
                {
                    if (driversTrucks.Contains(trucks.TruckID))
                    {
                        driversToUpdate.Trucks.Remove(trucks);
                    }
                }
            }
        }
        

        // GET: Drivers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Drivers drivers = db.Drivers.Find(id);
            if (drivers == null)
            {
                return HttpNotFound();
            }
            return View(drivers);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            

            Drivers drivers = db.Drivers
                .Include(i => i.OfficeAssignment)
                 .Where(i => i.ID == id)
                 .Single();

            db.Drivers.Remove(drivers);

            var customers = db.Customers
                .Where(d => d.DriverID == id)
                .SingleOrDefault();
            if (customers != null)
            {
                customers.DriverID = null;
            }

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
