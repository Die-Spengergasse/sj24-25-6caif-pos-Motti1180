namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Cashier : Employee
    {
        public Cashier(int registrationnumber, string firstname, string lastname, Address address, string type, 
            string jobSpezialisation): base (registrationnumber, firstname, lastname, address, type)
        {
            JobSpezialisation = jobSpezialisation;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Cashier() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public string JobSpezialisation { get; set; }
    }
}