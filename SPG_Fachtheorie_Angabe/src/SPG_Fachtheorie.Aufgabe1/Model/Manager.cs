namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class Manager : Employee
    {
        public Manager(int registrationnumber, string firstname, string lastname, Address address, string type, 
            string carType): base (registrationnumber, firstname, lastname, address, type)
        {
            CarType = carType;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected Manager() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string CarType { get; set; }
    }
}