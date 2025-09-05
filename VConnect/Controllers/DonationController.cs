using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using System.Diagnostics;

namespace VConnect.Controllers
{
    public class DonationController : Controller
    {
        // GET: /Donation
        public IActionResult Index()
        {
            return View("~/Views/Donation/Index.cshtml", new DonationViewModel());
        }

        // POST: /Donation/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Process(DonationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Convert ViewModel to Domain Model
                var donation = new Donation
                {
                    DonorName = model.DonorName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Amount = model.Amount,
                    Message = model.Message,
                    PaymentMethod = model.PaymentMethod,
                    IsAnonymous = model.IsAnonymous,
                    DonationDate = DateTime.Now,
                    TransactionId = GenerateTransactionId(),
                    Status = DonationStatus.Pending
                };

                // In a real application, you would process the payment here
                // For now, we'll simulate a successful payment
                donation.Status = DonationStatus.Completed;

                // Redirect to thank you page
                return RedirectToAction("ThankYou", new
                {
                    amount = donation.Amount,
                    transactionId = donation.TransactionId
                });
            }

            // If we got this far, something failed, redisplay form
            return View("~/Views/Donation/Index.cshtml", model);
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
        public IActionResult Stats()
        {
            // This would typically come from a database
            var stats = new DonationStats
            {
                TotalDonations = 1250000,
                TotalDonors = 3500,
                MonthlyDonations = 125000,
                MonthlyDonors = 250,
                RecentDonations = new List<RecentDonation>
                {
                    new RecentDonation { DonorInitials = "AR", DonorName = "Abdur Rahman", Amount = 5000, Date = DateTime.Now.AddDays(-1), IsAnonymous = false },
                    new RecentDonation { DonorInitials = "NS", DonorName = "Anonymous", Amount = 2500, Date = DateTime.Now.AddDays(-2), IsAnonymous = true },
                    new RecentDonation { DonorInitials = "FA", DonorName = "Fatima Ahmed", Amount = 10000, Date = DateTime.Now.AddDays(-3), IsAnonymous = false },
                    new RecentDonation { DonorInitials = "MR", DonorName = "Mohammed Rahim", Amount = 7500, Date = DateTime.Now.AddDays(-5), IsAnonymous = false }
                },
                DonationsByMethod = new Dictionary<string, decimal>
                {
                    { "bKash", 650000 },
                    { "Nagad", 350000 },
                    { "Bank Transfer", 150000 },
                    { "Card", 100000 }
                }
            };

            return View("~/Views/Donation/Stats.cshtml", stats);
        }

        // Helper method to generate transaction ID
        private string GenerateTransactionId()
        {
            return "VC" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999).ToString();
        }
    }
}