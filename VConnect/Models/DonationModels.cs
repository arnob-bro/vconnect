using System.ComponentModel.DataAnnotations;

namespace VConnect.Models
{
    public class DonationOption
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
    }

    public class DonationViewModel
    {
        public List<DonationOption> DonationOptions { get; set; } = new List<DonationOption>();

        [Display(Name = "Custom Amount")]
        [Range(1, 10000, ErrorMessage = "Please enter a valid amount between $1 and $10,000")]
        public decimal? CustomAmount { get; set; }

        [Required(ErrorMessage = "Please select a donation type")]
        [Display(Name = "Donation Type")]
        public string DonationType { get; set; } // One-time, Monthly

        [Required(ErrorMessage = "Please enter your name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Dedicate this donation")]
        public bool IsDedicated { get; set; }

        [Display(Name = "Dedication Name")]
        public string DedicatedTo { get; set; }

        [Display(Name = "Message")]
        public string DedicationMessage { get; set; }

        public bool CoverProcessingFees { get; set; }
    }
}