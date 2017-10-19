using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Memo_Logistics.DAL;
using Memo_Logistics.Models;
using PagedList;

namespace Memo_Logistics.Controllers
{
    public class LogisticsController : Controller
    {
        private LogisticsContext db = new LogisticsContext();

        // GET: Logistics

        public async Task<ViewResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "NameAsc" ? "NameDesc" : "NameAsc";
            ViewBag.DateSortParm = sortOrder == "DateAsc" ? "DateDesc" : "DateAsc";
            ViewBag.DateArSortParm = sortOrder == "Date_ArAsc" ? "Date_ArDesc" : "Date_ArAsc";
            ViewBag.DistSortParm = sortOrder == "DistAsc" ? "DistDesc" : "DistAsc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var logistics = from s in db.Logistics
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                logistics = logistics.Where(s => s.Customer.Name.Contains(searchString));

            }

            switch (sortOrder)
            {
                case "NameDesc":
                    logistics = logistics.OrderByDescending(s => s.Customer.Name);
                    break;
                case "DateAsc":
                    logistics = logistics.OrderBy(s => s.DepartureDate);
                    break;
                case "DateDesc":
                    logistics = logistics.OrderByDescending(s => s.DepartureDate);
                    break;
                case "Date_ArAsc":
                    logistics = logistics.OrderBy(s => s.ArrivalDate);
                    break;
                case "Date_ArDesc":
                    logistics = logistics.OrderByDescending(s => s.ArrivalDate);
                    break;
                case "DistAsc":
                    logistics = logistics.OrderBy(s => s.Distance);
                    break;
                case "DistDesc":
                    logistics = logistics.OrderByDescending(s => s.Distance);
                    break;
                default:
                    logistics = logistics.OrderBy(s => s.Customer.Name);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var count = await logistics.CountAsync();
            var items = await logistics.Take(pageSize).ToListAsync();
            return View(logistics.ToPagedList(pageNumber, pageSize));

        }

        // GET: Logistics/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Logistics logistics = await db.Logistics.FindAsync(id);
            if (logistics == null)
            {
                return HttpNotFound();
            }
            return View(logistics);
        }

        // GET: Logistics/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name");
            ViewBag.DriverID = new SelectList(db.Drivers, "ID", "FullName");
            return View();
        }

        // POST: Logistics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,CustomerID,DriverID,DeparturePoint,ArrivalPoint,Distance,DepartureDate,ArrivalDate")] Logistics logistics)
        {
            if (ModelState.IsValid)
            {
                db.Logistics.Add(logistics);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", logistics.CustomerID);
            ViewBag.DriverID = new SelectList(db.Drivers, "ID", "FullName", logistics.DriverID);
            return View(logistics);
        }

        // GET: Logistics/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Logistics logistics = await db.Logistics.FindAsync(id);
            if (logistics == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", logistics.CustomerID);
            ViewBag.DriverID = new SelectList(db.Drivers, "ID", "FullName", logistics.DriverID);
            return View(logistics);
        }

        // POST: Logistics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,CustomerID,DriverID,DeparturePoint,ArrivalPoint,Distance,DepartureDate,ArrivalDate")] Logistics logistics)
        {
            if (ModelState.IsValid)
            {
                db.Entry(logistics).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "Name", logistics.CustomerID);
            ViewBag.DriverID = new SelectList(db.Drivers, "ID", "FullName", logistics.DriverID);
            return View(logistics);
        }

        // GET: Logistics/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Logistics logistics = await db.Logistics.FindAsync(id);
            if (logistics == null)
            {
                return HttpNotFound();
            }
            return View(logistics);
        }

        // POST: Logistics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Logistics logistics = await db.Logistics.FindAsync(id);
            db.Logistics.Remove(logistics);
            await db.SaveChangesAsync();
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
