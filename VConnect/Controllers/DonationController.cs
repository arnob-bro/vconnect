using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using VConnect.Services;

namespace VConnect.Controllers
{
    public class DonationController : Controller
    {
        private readonly DonationService _donationService;

        public DonationController(DonationService donationService)
        {
            _donationService = donationService;
        }

        // GET: /Donation
        public IActionResult Index()
        {
            return View("~/Views/Donation/Index.cshtml", new DonationViewModel());
        }

        // POST: /Donation/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(DonationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Donation/Index.cshtml", model);
            }

            var donation = await _donationService.ProcessDonationAsync(model);

            return RedirectToAction("ThankYou", new
            {
                amount = donation.Amount,
                transactionId = donation.TransactionId
            });
        }

        // GET: /Donation/ThankYou
        public IActionResult ThankYou(decimal amount, string transactionId)
        {
            var model = new DonationThankYouModel
            {
                Amount = amount,
                TransactionId = transactionId,
                DonationDate = DateTime.Now
            };

            return View("~/Views/Donation/ThankYou.cshtml", model);
        }

        // GET: /Donation/Stats
        public async Task<IActionResult> Stats()
        {
            var stats = await _donationService.GetDonationStatsAsync();
            return View("~/Views/Donation/Stats.cshtml", stats);
        }
    }
}