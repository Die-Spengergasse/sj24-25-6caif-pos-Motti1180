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
            var address = new Address("Spengergasse", "Vienna", "1050");
            var employee = new Cashier(21, "Moritz", "Schneemann", address, "Cashier", "Cashier");
            // ACT
            db.Employees.Add(employee);
            db.SaveChanges();
            // ASSERT
            db.ChangeTracker.Clear();
            var cashierFromDb = db.Cashiers.First();
            Assert.True(cashierFromDb.Id != default);

        }

        [Fact]
        public void AddPaymentSuccessTest()
        {

        }

        [Fact]
        public void EmployeeDiscriminatorSuccessTest()
        {

        }
    }
}