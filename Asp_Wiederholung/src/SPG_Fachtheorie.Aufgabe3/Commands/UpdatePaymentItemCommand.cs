using System.ComponentModel.DataAnnotations;

public record UpdatePaymentItemCommand(
    int Id,
    string ArticleName,
    int Amount,
    decimal Price,
    int PaymentId,
    DateTime? LastUpdated) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Id <= 0)
            yield return new ValidationResult("Invalid ID.", new[] { nameof(Id) });
        if (string.IsNullOrWhiteSpace(ArticleName))
            yield return new ValidationResult("Article name cannot be empty.", new[] { nameof(ArticleName) });
        if (Amount <= 0)
            yield return new ValidationResult("Amount must be greater than 0.", new[] { nameof(Amount) });
        if (Price <= 0)
            yield return new ValidationResult("Price must be greater than 0.", new[] { nameof(Price) });
        if (PaymentId <= 0)
            yield return new ValidationResult("Invalid payment ID.", new[] { nameof(PaymentId) });
    }
}
