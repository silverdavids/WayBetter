using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Concrete
{
    public class Shift
    {
        [Key]
        public int ShiftId { get; set; }
        [Display(Name = "Start")]
        public DateTime StartTime { get; set; }
        [Display(Name = "End")]
        public DateTime EndTime { get; set; }
        public int AssignedBy { get; set; }
        public bool? IsClosed { get; set; }
        [DataType(DataType.Currency), Column(TypeName = "money"), Display(Name = "Start Cash")]
        public Decimal StartCash { get; set; }
        [DataType(DataType.Currency), Column(TypeName = "money"), Display(Name = "Cash In")]
        public Decimal CashIn { get; set; }
        [DataType(DataType.Currency), Column(TypeName = "money"), Display(Name = "Cash Out")]
        public Decimal CashOut { get; set; }
        [DataType(DataType.Currency), Display(Name = "Net Cash")]
        public Decimal NetCash
        {
            get
            {
                return StartCash + CashIn - CashOut;
            }
        }
        [DataType(DataType.Currency)]
        public Decimal Balance
        {
            get { return StartCash + CashIn - CashOut; }
        }
        [ForeignKey("Cashier")]
        public int PersonId { get; set; }

        public int? TerminalId { get; set; }


        //Navigation Properties
        public virtual Employee Cashier { get; set; }
        public virtual Terminal Terminal { get; set; }

    }
}
