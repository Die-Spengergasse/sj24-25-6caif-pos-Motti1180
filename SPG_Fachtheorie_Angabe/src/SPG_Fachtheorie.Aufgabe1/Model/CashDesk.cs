using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace SPG_Fachtheorie.Aufgabe1.Model
{
    public class CashDesk
    {
        public CashDesk(int number)
        {
            Number = number;
        }
        protected CashDesk() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Number { get; set; }
    }
}