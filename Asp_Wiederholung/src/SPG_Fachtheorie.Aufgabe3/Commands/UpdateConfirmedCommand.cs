using System.ComponentModel.DataAnnotations;

public record UpdateConfirmedCommand(DateTime? Confirmed) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Confirmed.HasValue && Confirmed.Value > DateTime.UtcNow.AddMinutes(1))
        {
            yield return new ValidationResult("Confirmed date cannot be more than 1 minute in the future.", new[] { nameof(Confirmed) });
        }
    }
}
