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
    public class CustomersController : Controller
    {
        private LogisticsContext db = new LogisticsContext();

        // GET: Customers
        public async Task<ViewResult> Index(string searchString, string sortOrder, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "NameAsc" ? "NameDesc" : "NameAsc";
            ViewBag.BudgetSortParm = sortOrder == "BudgetAsc" ? "BudgetDesc" : "BudgetAsc";
            ViewBag.CargoSortParm = sortOrder == "CargoArAsc" ? "CargoArDesc" : "CargoArAsc";
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

            var customers = from s in db.Customers
                            select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(s => s.Name.Contains(searchString));

            }

            switch (sortOrder)
            {
                case "NameDesc":
                    customers = customers.OrderByDescending(s => s.Name);
                    break;
                case "BudgetAsc":
                    customers = customers.OrderBy(s => s.Budget);
                    break;
                case "BudgetDesc":
                    customers = customers.OrderByDescending(s => s.Budget);
                    break;
                case "CargoArAsc":
                    customers = customers.OrderBy(s => s.Cargo);
                    break;
                case "CargoArDesc":
                    customers = customers.OrderByDescending(s => s.Cargo);
                    break;
                case "DateAsc":
                    customers = customers.OrderBy(s => s.OrderDate);
                    break;
                case "DateDesc":
                    customers = customers.OrderByDescending(s => s.OrderDate);
                    break;
                default:
                    customers = customers.OrderBy(s => s.Name);
                    break;
            }


            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var count = await customers.CountAsync();
            var items = await customers.Take(pageSize).ToListAsync();

            return View(customers.ToPagedList(pageNumber, pageSize));

        }

        // GET: Customers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = await db.Customers.FindAsync(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            ViewBag.DriverID = new SelectList(db.Drivers, "ID", "FullName");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CustomerID,Name,Budget,Cargo,OrderDate,DriverID")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customers);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.DriverID = new SelectList(db.Drivers, "ID", "FullName", customers.DriverID);
            return View(customers);
        }

        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = await db.Customers.FindAsync(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            ViewBag.DriverID = new SelectList(db.Drivers, "ID", "FullName", customers.DriverID);
            return View(customers);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CustomerID,Name,Budget,Cargo,OrderDate,DriverID")] Customers customers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customers).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.DriverID = new SelectList(db.Drivers, "ID", "FullName", customers.DriverID);
            return View(customers);
        }

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = await db.Customers.FindAsync(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Customers customers = await db.Customers.FindAsync(id);
            db.Customers.Remove(customers);
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
