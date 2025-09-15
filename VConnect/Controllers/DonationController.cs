using Microsoft.AspNetCore.Mvc;
using VConnect.Models;
using VConnect.Services;
using System.Threading.Tasks;

namespace VConnect.Controllers
{
    public class DonationController : Controller
    {
        private readonly IDonationService _donationService;

        public DonationController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        // GET: /Donation/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Fixing the issue with the Enum.Parse line in the POST Create method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Donation donation)
        {
            // Remove irrelevant ModelState errors
            if (donation.PaymentMethod == "bKash")
            {
                ModelState.Remove(nameof(donation.CVV));
                ModelState.Remove(nameof(donation.CardNumber));
                ModelState.Remove(nameof(donation.ExpiryDate));
                ModelState.Remove(nameof(donation.CardHolderName));
                ModelState.Remove(nameof(donation.NagadNumber));
                ModelState.Remove(nameof(donation.BankName));
                ModelState.Remove(nameof(donation.AccountNumber));
            }
            else if (donation.PaymentMethod == "Nagad")
            {
                ModelState.Remove(nameof(donation.CVV));
                ModelState.Remove(nameof(donation.CardNumber));
                ModelState.Remove(nameof(donation.ExpiryDate));
                ModelState.Remove(nameof(donation.CardHolderName));
                ModelState.Remove(nameof(donation.BkashNumber));
                ModelState.Remove(nameof(donation.BankName));
                ModelState.Remove(nameof(donation.AccountNumber));
            }
            else if (donation.PaymentMethod == "BankTransfer")
            {
                ModelState.Remove(nameof(donation.CVV));
                ModelState.Remove(nameof(donation.CardNumber));
                ModelState.Remove(nameof(donation.ExpiryDate));
                ModelState.Remove(nameof(donation.CardHolderName));
                ModelState.Remove(nameof(donation.BkashNumber));
                ModelState.Remove(nameof(donation.NagadNumber));
            }
            else if (donation.PaymentMethod == "CreditCard" || donation.PaymentMethod == "DebitCard")
            {
                ModelState.Remove(nameof(donation.BkashNumber));
                ModelState.Remove(nameof(donation.NagadNumber));
                ModelState.Remove(nameof(donation.BankName));
                ModelState.Remove(nameof(donation.AccountNumber));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();
                // Debug: inspect errors
                Console.WriteLine(string.Join(", ", errors));
                return View(donation); // return with validation errors
            }

            var created = await _donationService.CreateDonationAsync(donation);
            return RedirectToAction("ThankYou", new { id = created.TransactionId });
        }


        // GET: /Donation/ThankYou/5
        [HttpGet]
        public async Task<IActionResult> ThankYou(string id)
        {
            var donation = await _donationService.GetDonationByTransactionIdAsync(id);
            if (donation == null)
                return NotFound();

            var model = new DonationThankYouModel
            {
                DonorName = donation.DonorName,
                Amount = donation.Amount,
                DonationDate = donation.CreatedAt,
                TransactionId = donation.TransactionId,
                IsAnonymous = donation.IsAnonymous,
                PaymentMethod = donation.PaymentMethod.ToString()
            };

            return View(model);
        }

        // GET: /Donation/Stats
        [HttpGet]
        public async Task<IActionResult> Stats()
        {
            var stats = await _donationService.GetDonationStatsAsync();
            return View(stats);
        }
    }
}
