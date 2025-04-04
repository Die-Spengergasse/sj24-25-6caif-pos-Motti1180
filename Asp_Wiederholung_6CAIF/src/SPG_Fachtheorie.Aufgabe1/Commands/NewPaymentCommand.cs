using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPG_Fachtheorie.Aufgabe1.Commands
{
    public record NewPaymentCommand(
        [Required, MaxLength(255)]
        string ArticleName,
        [Range(1, int.MaxValue, ErrorMessage = "Invalid amount.")]
        int Amount,
        [Range(0.01, double.MaxValue, ErrorMessage = "Invalid price.")]
        decimal Price,
        [Range(1, int.MaxValue, ErrorMessage = "Invalid PaymentId.")]
        int PaymentId) : IValidatableObject
    {
        public int CashDeskNumber { get; internal set; }
        public int EmployeeRegistrationNumber { get; internal set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ArticleName))
            {
                yield return new ValidationResult("ArticleName cannot be empty.",
                    new[] { nameof(ArticleName) });
            }
        }
    }
}
