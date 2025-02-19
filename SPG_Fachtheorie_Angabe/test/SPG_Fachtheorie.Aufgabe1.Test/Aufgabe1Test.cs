using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using SPG_Fachtheorie.Aufgabe1.Infrastructure;
using SPG_Fachtheorie.Aufgabe1.Model;
using System;
using System.Linq;
using Xunit;

namespace SPG_Fachtheorie.Aufgabe1.Test
{
    [Collection("Sequential")]
    public class Aufgabe1Test
    {
        private AppointmentContext GetEmptyDbContext()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite(@"Data Source=cash.db")
                .Options;

            var db = new AppointmentContext(options);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }

        // Creates an empty DB in Debug\net8.0\cash.db
        [Fact]
        public void CreateDatabaseTest()
        {
            using var db = GetEmptyDbContext();
        }

        [Fact]
        public void AddCashierSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var address = new Model.Address("Spengergasse", "Wien", "1050");
            var cashier = new Cashier(21, "Moritz", "Schneemann", address, "Cashier", "Cashier");

            db.Employees.Add(cashier);
            db.SaveChanges();

            db.ChangeTracker.Clear();
            var cashierFromDb = db.Employees.First();
            Assert.True(cashierFromDb.RegistrationNumber != default);
        }

        [Fact]
        public void AddPaymentSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var address = new Model.Address("Spengergasse", "Wien", "1050");
            var cashier = new Cashier(21, "Moritz", "Schneemann", address, "Cashier", "Cashier");
            var cashDesk = new CashDesk(1);
            var currentDateTime = new DateTime(2025, 02, 19, 12, 0, 0);
            var payment = new Payment(cashDesk, currentDateTime, PaymentType.Cash, cashier);

            db.Employees.Add(cashier);
            db.CashDesks.Add(cashDesk);
            db.Payments.Add(payment);

            db.SaveChanges();
            db.ChangeTracker.Clear();

            var paymentFromDb = db.Payments.FirstOrDefault(p => p.PaymentDateTime == currentDateTime);
            Assert.NotNull(paymentFromDb);
            Assert.Equal(PaymentType.Cash, paymentFromDb!.PaymentType);
        }

        [Fact]
        public void EmployeeDiscriminatorSuccessTest()
        {
            using var db = GetEmptyDbContext();
            var address = new Model.Address("Spengergasse", "Wien", "1050");
            var cashier = new Cashier(21, "Moritz", "Schneemann", address, "Cashier", "Cashier");

            db.Employees.Add(cashier);
            db.SaveChanges();
            db.ChangeTracker.Clear();

            var employeeFromDb = db.Employees.FirstOrDefault(e => e.RegistrationNumber == 21);
            Assert.NotNull(employeeFromDb);
            Assert.True(employeeFromDb!.Type == "Cashier" || employeeFromDb.Type == "Manager");
        }
    }
}