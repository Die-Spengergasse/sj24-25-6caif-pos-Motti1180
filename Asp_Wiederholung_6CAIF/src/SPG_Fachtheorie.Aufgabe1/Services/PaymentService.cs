using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Commands;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Linq;

namespace SPG_Fachtheorie.Aufgabe1.Services
{
    public class PaymentService
    {
        private readonly AppointmentContext _db;

        public PaymentService(AppointmentContext db)
        {
            _db = db;
        }

        public IQueryable<Payment> Payments => _db.Payments.AsQueryable();

        public Payment CreatePayment(NewPaymentCommand cmd)
        {
            var cashDesk = _db.CashDesks
                .FirstOrDefault(c => c.Number == cmd.CashDeskNumber);
            if (cashDesk is null)
                throw new PaymentServiceException("Invalid cashdesk.");

            var employee = _db.Employees
                .FirstOrDefault(e => e.RegistrationNumber == cmd.EmployeeRegistrationNumber);
            if (employee is null)
                throw new PaymentServiceException("Invalid employee.");

            var existingPayment = _db.Payments
                .FirstOrDefault(p => p.CashDesk.Number == cmd.CashDeskNumber && p.Confirmed == null);
            if (existingPayment != null)
                throw new PaymentServiceException("Open payment for cashdesk.");

            var paymentType = Enum.Parse<PaymentType>(cmd.PaymentType);
            if (paymentType == PaymentType.CreditCard && !(employee is Manager))
                throw new PaymentServiceException("Insufficient rights to create a credit card payment.");

            var payment = new Payment(
                cashDesk, DateTime.UtcNow, employee, paymentType);
            _db.Payments.Add(payment);
            SaveOrThrow();
            return payment;
        }

        public void ConfirmPayment(int paymentId)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment is null)
                throw new PaymentServiceException("Payment not found.");

            if (payment.Confirmed.HasValue)
                throw new PaymentServiceException("Payment already confirmed.");

            payment.Confirmed = DateTime.UtcNow;
            SaveOrThrow();
        }

        public void AddPaymentItem(NewPaymentCommand cmd)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == cmd.PaymentId);
            if (payment is null)
                throw new PaymentServiceException("Payment not found.");

            if (payment.Confirmed.HasValue)
                throw new PaymentServiceException("Payment already confirmed.");

            var paymentItem = new PaymentItem(cmd.ArticleName, cmd.Amount, cmd.Price, payment);
            _db.PaymentItems.Add(paymentItem);
            SaveOrThrow();
        }

        public void DeletePayment(int paymentId, bool deleteItems)
        {
            var payment = _db.Payments.FirstOrDefault(p => p.Id == paymentId);
            if (payment is null)
                throw new PaymentServiceException("Payment not found.");

            var paymentItems = _db.PaymentItems
                .Where(p => p.Payment.Id == paymentId)
                .ToList();

            if (deleteItems)
            {
                _db.PaymentItems.RemoveRange(paymentItems);
            }
            else if (paymentItems.Any())
            {
                throw new PaymentServiceException("Payment has payment items.");
            }

            _db.Payments.Remove(payment);
            SaveOrThrow();
        }

        private void SaveOrThrow()
        {
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new PaymentServiceException(e.InnerException?.Message ?? e.Message);
            }
        }
    }
}
