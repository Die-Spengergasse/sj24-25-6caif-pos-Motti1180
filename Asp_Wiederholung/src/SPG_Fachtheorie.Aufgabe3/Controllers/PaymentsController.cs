using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe3.Dtos;

namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    [Route("api/[controller]")]  // --> api/payments
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly AppointmentContext _db;

        public PaymentsController(AppointmentContext db)
        {
            _db = db;
        }

        /// <summary>
        /// GET /api/payments
        /// GET /api/payments?cashDesk=1
        /// GET /api/payments?dateFrom=2024-05-13
        /// GET /api/payments?dateFrom=2024-05-13&cashDesk=1
        /// </summary>
        [HttpGet]
        public ActionResult<List<PaymentDto>> GetAllPayments(
            [FromQuery] int? cashDesk,
            [FromQuery] DateTime? dateFrom)
        {
            var payments = _db.Payments
                .Where(p => (!cashDesk.HasValue || p.CashDesk.Number == cashDesk.Value)
                         && (!dateFrom.HasValue || p.PaymentDateTime >= dateFrom.Value))
                .Select(p => new PaymentDto(
                    p.Id, p.Employee.FirstName, p.Employee.LastName,
                    p.PaymentDateTime,
                    p.CashDesk.Number, p.PaymentType.ToString(),
                    p.PaymentItems.Sum(pi => pi.Price)))
                .ToList();
            return Ok(payments);
        }

        /// <summary>
        /// GET /api/payments/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<PaymentDetailDto> GetPaymentById(int id)
        {
            var payment = _db.Payments
                .Where(p => p.Id == id)
                .Select(p => new PaymentDetailDto(
                    p.Id, p.Employee.FirstName, p.Employee.LastName,
                    p.CashDesk.Number, p.PaymentType.ToString(),
                    p.PaymentItems
                        .Select(pi => new PaymentItemDto(
                            pi.ArticleName, pi.Amount, pi.Price))
                        .ToList()))
                .FirstOrDefault();
            if (payment is null) return NotFound();
            return Ok(payment);
        }
    }
}
