using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public abstract class Employee
    {
        protected Employee(int registrationNumber, string firstName, string lastName, Address address, string type)
        {
            RegistrationNumber = registrationNumber;
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            Type = type;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Employee() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RegistrationNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [MaxLength(4096)]
        public Address Address { get; set; }
        public string Type { get; set; } = null!;
    }
}