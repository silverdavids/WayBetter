using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Concrete
{
    public class Employee:Person
    {
        public Employee()
        {
            Shifts = new List<Shift>();
        }

       
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime HireDate { get; set; }

        [ForeignKey("Branch")]
        public int BranchId { get; set; }
  
        public virtual Branch Branch { get; set; }    
        public ICollection<Shift> Shifts
        { 
            get; 
            set;
        }
    }

}