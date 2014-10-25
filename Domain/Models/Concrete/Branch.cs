using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Concrete
{
    public class Branch
    {
        public Branch() 
        {
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
            Terminals=new HashSet<Terminal>() ;
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
           Employees= new HashSet<Employee>();
// ReSharper disable once DoNotCallOverridableMethodsInConstructor
           Receipts=new HashSet<Receipt>() ;
        
        }

       


        [Key]
        public int BranchId { get; set; }
        [Display(Name = "Name")]
        public string BranchName { get; set; }
        [Display(Name = "Code")]
        public string BranchCode { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
        [Display(Name = "Created"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }
        [DataType(DataType.Currency), Column(TypeName = "money"), Display(Name = "Maximum Cash")]
        public Decimal MaxCash { get; set; }
        [DataType(DataType.Currency), Column(TypeName = "money"), Display(Name = "Min Stake")]
        public Decimal? MinStake { get; set; }
        //[ForeignKey("Manager")]
        public string ManagerId { get; set; }
        public decimal Balance { get; set; }

        [ForeignKey("BranchType")]
        public int? BranchTypeId { get; set; }

        [ForeignKey("Company")]
        public int? CompanyId { get; set; }

        //Navigation properties
        public BranchType BranchType { get; set; }
       // public virtual Employee Manager { get; set; }
        public virtual ICollection<Terminal> Terminals
        {
            get;
            set;
        }
        public virtual ICollection<Employee> Employees
        {
            get;
            set;
        }
        public virtual ICollection<Receipt> Receipts
        {
            get;
            set;
        }
        public virtual Company Company { get; set; }
       


    }
}
