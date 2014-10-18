using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Concrete
{
    public class BetOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BetOptionId { get; set; }

        [Required(ErrorMessage = "* Required")]
        public string Option { get; set; }

        public string Line { get; set; }

        [ForeignKey("BetCategory")]
        public int BetCategoryId { get; set; }

        public virtual BetCategory BetCategory { get; set; }
    }
}
