using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZeusConsole.Models
{
    public class BetOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BetOptionId { get; set; }

        [ForeignKey("BetCategory")]
        public int BetCategoryId { get; set; }

        public string Option { get; set; }

        public string Line { get; set; }
        
        public virtual BetCategory BetCategory { get; set; }
    }
}
