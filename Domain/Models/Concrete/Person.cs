using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Domain.Models.Concrete
{
    public class Person
    {
        
        [Key]
        public int PersonId { get; set; }
        [Display(Name = "First Name"), StringLength(50, MinimumLength = 1, ErrorMessage = "Name has to be atlleast 1  and not more than 50 characters"), Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Display(Name = "Last Name"), StringLength(50, MinimumLength = 1, ErrorMessage = "Name has to be atlleast 1  and not more than 50 characters"), Required(ErrorMessage = "First name is required")]
        public string Surname { get; set; }
        [DisplayFormat(NullDisplayText = "Not Set")]
        public Gender Gender { get; set; }
        public string Email { set; get; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true), Display(Name = "Birth Date")]
        public DateTime DateOfBirth { get; set; }// date of birth

        [Display(Name = "AddressLine1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "AddressLine2")]
        public string AddressLine2 { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Name")]
        public string FullName { get { return FirstName + " " + MiddleName + " " + Surname; } }
        public string PhoneNo { get; set; }
        public string Mobile { get; set; }
        public string Photo { get; set; }
       
   


     }
     public enum Gender{ M, F}
}