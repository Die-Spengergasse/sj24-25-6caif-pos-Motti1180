using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Commands;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using SPG_Fachtheorie.Aufgabe1.Services;
using System;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    public class PaymentServiceTests
    {
        private AppointmentContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppointmentContext>()
                .UseSqlite("Data Source=cash.db")
                .Options;

            var db = new AppointmentContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        private void SeedDatabase(AppointmentContext db, bool includeExistingPayment = false, bool confirmPayment = false)
        {
            var cashier = new Cashier(1001, "FN", "LN", new DateOnly(2000, 1, 1), 2000, null, "JobSpec");
            var manager = new Manager(1002, "FNM", "LNM", new DateOnly(1990, 1, 1), null, null, "SUV");
            var cashDesk1 = new CashDesk(1);
            var cashDesk2 = new CashDesk(2);

            db.Employees.AddRange(cashier, manager);
            db.CashDesks.AddRange(cashDesk1, cashDesk2);

            if (includeExistingPayment)
            {
                var existingPayment = new Payment(cashDesk2, DateTime.UtcNow, cashier, PaymentType.Cash);
                if (confirmPayment)
                {
                    existingPayment.Confirmed = DateTime.UtcNow.AddMinutes(-5);
                }
                db.Payments.Add(existingPayment);
            }

            db.SaveChanges();
            db.ChangeTracker.Clear();
        }

        [Theory]
        [InlineData(-1, 1001, "Cash", "Invalid cashdesk")]
        [InlineData(999, 1001, "Cash", "Invalid cashdesk")]
        [InlineData(1, -1, "Cash", "Invalid employee")]
        [InlineData(1, 9999, "Cash", "Invalid employee")]
        [InlineData(1, 1001, "CreditCard", "Insufficient rights to create a credit card payment.")]
        [InlineData(2, 1001, "Cash", "Open payment for cashdesk.")]
        public void CreatePayment_ShouldThrowException_WhenInvalidInput(int cashDeskNumber, int employeeRegistrationNumber, string paymentType, string expectedErrorMessage)
        {
            using var db = CreateDbContext();
            SeedDatabase(db, includeExistingPayment: true);

            var service = new PaymentService(db);
            var cmd = new NewPaymentCommand(cashDeskNumber, paymentType, employeeRegistrationNumber);

            var ex = Assert.Throws<PaymentServiceException>(() => service.CreatePayment(cmd));
            Assert.Equal(expectedErrorMessage, ex.Message);
        }

        [Fact]
        public void CreatePayment_ShouldSucceed_WhenValidInput()
        {
            using var db = CreateDbContext();
            SeedDatabase(db);

            var service = new PaymentService(db);

            var cmdCash = new NewPaymentCommand(1, "Cash", 1001);
            var cmdCreditCard = new NewPaymentCommand(2, "CreditCard", 1002);

            var paymentCash = service.CreatePayment(cmdCash);
            var paymentCreditCard = service.CreatePayment(cmdCreditCard);

            db.ChangeTracker.Clear();

            var paymentCashFromDb = db.Payments.Include(p => p.CashDesk).Include(p => p.Employee)
                .FirstOrDefault(p => p.Id == paymentCash.Id);
            var paymentCreditCardFromDb = db.Payments.Include(p => p.CashDesk).Include(p => p.Employee)
                .FirstOrDefault(p => p.Id == paymentCreditCard.Id);

            Assert.NotNull(paymentCashFromDb);
            Assert.Equal(1, paymentCashFromDb.CashDesk.Number);
            Assert.Equal(1001, paymentCashFromDb.Employee.RegistrationNumber);
            Assert.Equal(PaymentType.Cash, paymentCashFromDb.PaymentType);
            Assert.Null(paymentCashFromDb.Confirmed);

            Assert.NotNull(paymentCreditCardFromDb);
            Assert.Equal(2, paymentCreditCardFromDb.CashDesk.Number);
            Assert.Equal(1002, paymentCreditCardFromDb.Employee.RegistrationNumber);
            Assert.Equal(PaymentType.CreditCard, paymentCreditCardFromDb.PaymentType);
            Assert.Null(paymentCreditCardFromDb.Confirmed);
        }

        [Theory]
        [InlineData(999, true, "Payment not found")]
        [InlineData(1, false, "Payment already confirmed")]
        public void ConfirmPayment_ShouldThrowException_WhenInvalidInput(int paymentId, bool isNotFoundError, string expectedErrorMessage)
        {
            using var db = CreateDbContext();
            SeedDatabase(db, includeExistingPayment: true, confirmPayment: !isNotFoundError);

            var service = new PaymentService(db);

            var ex = Assert.Throws<PaymentServiceException>(() => service.ConfirmPayment(paymentId));
            Assert.Equal(expectedErrorMessage, ex.Message);
        }

        [Fact]
        public void ConfirmPayment_ShouldSucceed_WhenValidInput()
        {
            using var db = CreateDbContext();
            SeedDatabase(db, includeExistingPayment: true);

            var paymentId = db.Payments.First().Id;

            var service = new PaymentService(db);
            service.ConfirmPayment(paymentId);

            db.ChangeTracker.Clear();

            var confirmedPayment = db.Payments.Find(paymentId);
            Assert.NotNull(confirmedPayment);
            Assert.NotNull(confirmedPayment.Confirmed);
            Assert.True(confirmedPayment.Confirmed.Value > DateTime.UtcNow.AddMinutes(-1));
        }

        [Fact]
        public void AddPaymentItem_ShouldSucceed_WhenValidInput()
        {
            using var db = CreateDbContext();
            SeedDatabase(db, includeExistingPayment: true);

            var paymentId = db.Payments.First().Id;

            var service = new PaymentService(db);
            var cmd = new NewPaymentItemCommand("Test Item", 2, 20.0m, paymentId);

            service.AddPaymentItem(cmd);

            db.ChangeTracker.Clear();

            var paymentFromDb = db.Payments.Include(p => p.PaymentItems).FirstOrDefault(p => p.Id == paymentId);
            Assert.NotNull(paymentFromDb);
            Assert.Single(paymentFromDb.PaymentItems);
            var item = paymentFromDb.PaymentItems.First();
            Assert.Equal("Test Item", item.ArticleName);
            Assert.Equal(2, item.Amount);
            Assert.Equal(20.0m, item.Price);
        }
    }
}