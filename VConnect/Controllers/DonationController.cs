using Microsoft.AspNetCore.Mvc;
using VConnect.Models;

namespace VConnect.Controllers
{
    public class DonationController : Controller
    {
        // GET: /Donation
        public IActionResult Index()
        {
            var model = new DonationViewModel
            {
                DonationOptions = new List<DonationOption>
                {
                    new DonationOption { Amount = 25, Description = "Provides meals for 5 families" },
                    new DonationOption { Amount = 50, Description = "Supports education for 2 children" },
                    new DonationOption { Amount = 100, Description = "Helps build community infrastructure" },
                    new DonationOption { Amount = 250, Description = "Sponsors a community event" },
                    new DonationOption { Amount = 500, Description = "Supports emergency relief efforts" }
                }
            };

            return View(model);
        }

        // POST: /Donation/Process
        [HttpPost]
        public IActionResult Process(DonationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Process donation logic here
                // Redirect to thank you page or payment gateway
                return RedirectToAction("Success", "Donation");
            }
            return View("Index", model);
        }

        // GET: /Donation/Success
        public IActionResult Success()
        {
            return View();
        }
    }
}