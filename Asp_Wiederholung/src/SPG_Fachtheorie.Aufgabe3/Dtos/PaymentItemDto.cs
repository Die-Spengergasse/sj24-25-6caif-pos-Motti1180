using SPG_Fachtheorie.Aufgabe1.Model;

namespace SPG_Fachtheorie.Aufgabe3.Dtos
{
    public record PaymentItemDto(
        string articleName, int amount, 
        decimal price);
}