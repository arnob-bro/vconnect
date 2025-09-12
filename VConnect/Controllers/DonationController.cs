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

        // POST: /Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var donation = new Donation
            {
                DonorName = model.DonorName,
                Email = model.Email,
                Phone = model.Phone,
                Amount = model.Amount,
                Message = model.Message,
                PaymentMethod = Enum.Parse<PaymentMethod>(model.PaymentMethod),
                IsAnonymous = model.IsAnonymous,
                BkashNumber = model.BkashNumber,
                NagadNumber = model.NagadNumber,
                BankName = model.BankName,
                AccountNumber = model.AccountNumber,
                CardNumber = model.CardNumber,
                ExpiryDate = model.ExpiryDate,
                CVV = model.CVV,
                CardHolderName = model.CardHolderName,
                Status = DonationStatus.Pending
            };

            var created = await _donationService.CreateDonationAsync(donation);

            return RedirectToAction("ThankYou", new { id = created.Id });
        }

        // GET: /Donation/ThankYou/5
        [HttpGet]
        public async Task<IActionResult> ThankYou(int id)
        {
            var donation = await _donationService.GetDonationByIdAsync(id);
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
