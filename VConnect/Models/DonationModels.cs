using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VConnect.Models
{
    // Main donation model
    public class Donation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string DonorName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 1000000, ErrorMessage = "Amount must be at least 1 BDT")]
        [Display(Name = "Donation Amount (BDT)")]
        public decimal Amount { get; set; }

        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        [Display(Name = "Message (Optional)")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Make this donation anonymous")]
        public bool IsAnonymous { get; set; }

        public DateTime DonationDate { get; set; } = DateTime.Now;

        [Display(Name = "Transaction ID")]
        public string TransactionId { get; set; }

        public DonationStatus Status { get; set; } = DonationStatus.Pending;
    }

    // Donation status enum
    public enum DonationStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    // Payment method enum
    public enum PaymentMethod
    {
        bKash,
        Nagad,
        BankTransfer,
        CreditCard,
        DebitCard
    }

    // ViewModel for the donation form
    public class DonationViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Full Name")]
        public string DonorName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number (Optional)")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 1000000, ErrorMessage = "Amount must be at least 1 BDT")]
        [Display(Name = "Donation Amount (BDT)")]
        public decimal Amount { get; set; }

        [Display(Name = "Message (Optional)")]
        public string Message { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }

        [Display(Name = "Make this donation anonymous")]
        public bool IsAnonymous { get; set; }

        // For specific payment methods
        [Display(Name = "bKash Number")]
        [RequiredIf(nameof(PaymentMethod), "bKash", ErrorMessage = "bKash number is required")]
        public string BkashNumber { get; set; }

        [Display(Name = "Nagad Number")]
        [RequiredIf(nameof(PaymentMethod), "Nagad", ErrorMessage = "Nagad number is required")]
        public string NagadNumber { get; set; }

        [Display(Name = "Bank Name")]
        [RequiredIf(nameof(PaymentMethod), "BankTransfer", ErrorMessage = "Bank name is required")]
        public string BankName { get; set; }

        [Display(Name = "Account Number")]
        [RequiredIf(nameof(PaymentMethod), "BankTransfer", ErrorMessage = "Account number is required")]
        public string AccountNumber { get; set; }

        [Display(Name = "Card Number")]
        [RequiredIf(nameof(PaymentMethod), "CreditCard", "DebitCard", ErrorMessage = "Card number is required")]
        [CreditCard(ErrorMessage = "Invalid card number")]
        public string CardNumber { get; set; }

        [Display(Name = "Expiry Date")]
        [RequiredIf(nameof(PaymentMethod), "CreditCard", "DebitCard", ErrorMessage = "Expiry date is required")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Invalid expiry date format (MM/YY)")]
        public string ExpiryDate { get; set; }

        [Display(Name = "CVV")]
        [RequiredIf(nameof(PaymentMethod), "CreditCard", "DebitCard", ErrorMessage = "CVV is required")]
        [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "Invalid CVV")]
        public string CVV { get; set; }

        [Display(Name = "Card Holder Name")]
        [RequiredIf(nameof(PaymentMethod), "CreditCard", "DebitCard", ErrorMessage = "Card holder name is required")]
        public string CardHolderName { get; set; }
    }

    // Custom validation attribute for conditional required fields
    public class RequiredIfAttribute : ValidationAttribute
    {
        private string[] DependentProperties { get; set; }

        public RequiredIfAttribute(params string[] dependentProperties)
        {
            DependentProperties = dependentProperties;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var type = instance.GetType();

            foreach (var propertyName in DependentProperties)
            {
                var propertyValue = type.GetProperty(propertyName)?.GetValue(instance, null)?.ToString();

                if (propertyValue != null && propertyValue == validationContext.MemberName)
                {
                    if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }
            }

            return ValidationResult.Success;
        }
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
        public decimal MonthlyDonations { get; set; }
        public int MonthlyDonors { get; set; }
        public List<RecentDonation> RecentDonations { get; set; } = new List<RecentDonation>();
        public Dictionary<string, decimal> DonationsByMethod { get; set; } = new Dictionary<string, decimal>();
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
        public List<Donation> Donations { get; set; } = new List<Donation>();
        public Dictionary<string, decimal> AmountByPaymentMethod { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> CountByPaymentMethod { get; set; } = new Dictionary<string, int>();
    }
}