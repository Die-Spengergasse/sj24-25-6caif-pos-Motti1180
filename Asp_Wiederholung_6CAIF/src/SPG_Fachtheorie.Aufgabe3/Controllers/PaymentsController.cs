using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SPG_Fachtheorie.Aufgabe1.Commands;
using SPG_Fachtheorie.Aufgabe1.Services;
using SPG_Fachtheorie.Aufgabe3.Commands;
using SPG_Fachtheorie.Aufgabe3.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPG_Fachtheorie.Aufgabe3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _service;

        public PaymentsController(PaymentService service)
        {
            _service = service;
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
            var payments = _service.Payments
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
            var payment = _service.Payments
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddPayment([FromBody] NewPaymentCommand cmd)
        {
            try
            {
                var payment = _service.CreatePayment(cmd);
                return CreatedAtAction(nameof(AddPayment), new { payment.Id });
            }
            catch (PaymentServiceException e)
            {
                return Problem(e.Message, statusCode: 400);
            }
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateConfirmed(int id)
        {
            try
            {
                _service.ConfirmPayment(id);
                return NoContent();
            }
            catch (PaymentServiceException e)
            {
                return Problem(e.Message, statusCode: 400);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeletePayment(int id, [FromQuery] bool deleteItems)
        {
            try
            {
                _service.DeletePayment(id, deleteItems);
                return NoContent();
            }
            catch (PaymentServiceException e)
            {
                return Problem(e.Message, statusCode: 400);
            }
        }

        [HttpPut("/api/paymentItems/{id}")]
        public IActionResult UpdatePayment(int id, [FromBody] UpdatePaymentItemCommand cmd)
        {
            // Diese Route kann auskommentiert werden
            return NoContent();
        }
    }
}
