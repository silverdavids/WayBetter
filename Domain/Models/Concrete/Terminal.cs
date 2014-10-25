using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Concrete
{
    public class Terminal
    {
        private ICollection<Shift> _Shifts;
        [Key]
        [Display(Name = "ID")]
        public int TerminalId { get; set; }
        [Display(Name = "Terminal")]
        public string TerminalName { get; set; }
        [Display(Name = "IP")]
        public string IpAddress { get; set; }
        [Display(Name = "Created")]
        public DateTime DateCreated { get; set; }
        public bool isActive { get; set; }

        public int? BranchId { get; set; }


        //Navigation properties
        public virtual Branch Branch { get; set; }
        public virtual ICollection<Shift> Shifts
        {
            get { return _Shifts ?? (new List<Shift>()); }
            set { _Shifts = value; }
        }
    }
}
