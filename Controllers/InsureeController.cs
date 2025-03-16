using System;
using System.Linq;
using System.Web.Mvc;
using Insurance.Models;

namespace Insurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // POST: Insuree/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,SpeedingTickets,DUI,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                decimal totalQuote = 50m; // Base price

                int age = DateTime.Now.Year - insuree.DateOfBirth.Year;
                if (insuree.DateOfBirth > DateTime.Now.AddYears(-age)) 
                    age--;

                // Age-based pricing
                if (age <= 18) totalQuote += 100;
                else if (age >= 19 && age <= 25) totalQuote += 50;
                else totalQuote += 25;

                // Car year-based pricing
                if (insuree.CarYear < 2000) totalQuote += 25;
                else if (insuree.CarYear > 2015) totalQuote += 25;

                // Car make/model-based pricing
                if (insuree.CarMake.ToLower() == "porsche")
                {
                    totalQuote += 25;
                    if (insuree.CarModel.ToLower() == "911 carrera")
                        totalQuote += 25;
                }

                // Speeding tickets
                totalQuote += insuree.SpeedingTickets * 10;

                // DUI penalty
                if (insuree.DUI) totalQuote *= 1.25m;

                // Full coverage increase
                if (insuree.CoverageType) totalQuote *= 1.50m;

                insuree.Quote = totalQuote;
                db.Insurees.Add(insuree);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Admin View
        public ActionResult Admin()
        {
            var insurees = db.Insurees.Select(i => new {
                i.FirstName,
                i.LastName,
                i.EmailAddress,
                i.Quote
            }).ToList();

            return View(insurees);
        }
    }
}