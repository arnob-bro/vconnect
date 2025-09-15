using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VConnect.Models
{
    [Index(nameof(TransactionId), IsUnique = true)]
    public class Donation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string DonorName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }

        [Required]
        public DonationStatus Status { get; set; } = DonationStatus.Pending;

        [Required]
        public string PaymentMethod { get; set; }

        public bool IsAnonymous { get; set; } = false;

        // Optional payment-specific fields
        [MaxLength(20)]
        [Required(ErrorMessage = "bKash number is required")]
        public string BkashNumber { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Nagad number is required")]
        public string NagadNumber { get; set; }

        [MaxLength(150)]
        [Required(ErrorMessage = "Bank name number is required")]
        public string BankName { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Account  number is required")]
        public string AccountNumber { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "card number is required")]
        public string CardNumber { get; set; }

        [MaxLength(5)]
        [Required(ErrorMessage = "expiary data is required")]
        public string ExpiryDate { get; set; }

        [MaxLength(4)]
        [Required(ErrorMessage = "cvv is required")]
        public string CVV { get; set; }

        [MaxLength(150)]
        [Required(ErrorMessage = "CardHolderName is required")]
        public string? CardHolderName { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Optional: Transaction ID after payment
        [MaxLength(100)]
        public string? TransactionId { get; set; }
    }
}
