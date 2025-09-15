using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VConnect.Models
{


    // Donation status enum
    public enum DonationStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }




    // Model for donation thank you page
    public class DonationThankYouModel
    {
        public string DonorName { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public DateTime DonationDate { get; set; }
        public bool IsAnonymous { get; set; }
        public string PaymentMethod { get; set; }
    }

    // Model for donation statistics
    public class DonationStats
    {
        public decimal TotalDonations { get; set; }
        public int TotalDonors { get; set; }
        public List<RecentDonation> RecentDonations { get; set; }
        public Dictionary<string, decimal> DonationsByMethod { get; set; }
        public List<Donation> AllDonations { get; set; }
    }


    // Model for recent donations
    public class RecentDonation
    {
        public string DonorInitials { get; set; }
        public string DonorName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsAnonymous { get; set; }
    }

    // Model for donation impact
    public class DonationImpact
    {
        public int TreesPlanted { get; set; }
        public int MealsProvided { get; set; }
        public int EducationKits { get; set; }
        public int CleanWaterProvided { get; set; }
        public int CommunitiesSupported { get; set; }
    }

    // Model for recurring donation
    public class RecurringDonation
    {
        public int Id { get; set; }

        [Required]
        public string DonorName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Range(1, 1000000)]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

        [Required]
        [Display(Name = "Recurring Frequency")]
        public RecurringFrequency Frequency { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    // Recurring frequency enum
    public enum RecurringFrequency
    {
        Monthly,
        Quarterly,
        Yearly
    }

    // Model for donation report
    public class DonationReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalDonations { get; set; }
        public int UniqueDonors { get; set; }
       
        public Dictionary<string, decimal> AmountByPaymentMethod { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> CountByPaymentMethod { get; set; } = new Dictionary<string, int>();
    }
}
