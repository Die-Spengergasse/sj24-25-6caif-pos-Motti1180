using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe3.Test
{
    public class PaymentsControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PaymentsControllerTests(TestWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        // GET /api/payments mit Parametern
        [Theory]
        [InlineData(1, null, 2)] // Passe expectedCount an deine Testdaten an!
        [InlineData(null, "2024-05-13", 1)]
        [InlineData(1, "2024-05-13", 1)]
        public async Task GetPayments_Filtered_ReturnsExpectedResults(int? cashDesk, string? dateFrom, int expectedCount)
        {
            var url = "/api/payments?";
            if (cashDesk.HasValue) url += $"cashDesk={cashDesk.Value}&";
            if (!string.IsNullOrEmpty(dateFrom)) url += $"dateFrom={dateFrom}&";
            url = url.TrimEnd('&', '?');

            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var payments = await response.Content.ReadFromJsonAsync<List<PaymentDto>>();
            Assert.Equal(expectedCount, payments!.Count);

            if (cashDesk.HasValue)
                Assert.All(payments, p => Assert.Equal(cashDesk.Value, p.CashDeskNumber));
            if (!string.IsNullOrEmpty(dateFrom))
                Assert.All(payments, p => Assert.True(p.PaymentDateTime.Date >= DateTime.Parse(dateFrom).Date));
        }

        // GET /api/payments/{id}
        [Fact]
        public async Task GetPaymentById_ReturnsCorrectStatusCodes()
        {
            var response = await _client.GetAsync("/api/payments/1");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var notFoundResponse = await _client.GetAsync("/api/payments/9999");
            Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
        }

        // PATCH /api/payments/{id}
        [Fact]
        public async Task PatchPayment_ReturnsExpectedStatusCodes()
        {
            // Erfolgreiches Patch (Passe den Body an dein API-Modell an)
            var patchContent = JsonContent.Create(new { Confirmed = true });
            var patchResponse = await _client.PatchAsync("/api/payments/1", patchContent);
            Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

            // BadRequest testen (z.B. ungültige Daten)
            var badRequestContent = JsonContent.Create(new { Confirmed = false }); // Beispiel: falls das ein Fehler ist
            var badRequestResponse = await _client.PatchAsync("/api/payments/1", badRequestContent);
            Assert.Equal(HttpStatusCode.BadRequest, badRequestResponse.StatusCode);

            // NotFound testen
            var notFoundContent = JsonContent.Create(new { Confirmed = true });
            var notFoundResponse = await _client.PatchAsync("/api/payments/9999", notFoundContent);
            Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
        }

        // DELETE /api/payments/{id}
        [Fact]
        public async Task DeletePayment_ReturnsExpectedStatusCodes()
        {
            var response = await _client.DeleteAsync("/api/payments/1");
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var notFoundResponse = await _client.DeleteAsync("/api/payments/9999");
            Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
        }
    }

    // PaymentDto nach deinem Modell (angepasst an PaymentDto.cs)
    public record PaymentDto(
        int Id,
        string EmployeeFirstName,
        string EmployeeLastName,
        DateTime PaymentDateTime,
        int CashDeskNumber,
        string PaymentType,
        decimal TotalAmount
    );
}
