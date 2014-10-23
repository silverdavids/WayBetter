using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Concrete
{
    public class ShortMatchCode
    {
        [Key]
        [Column(Order = 100)]
        [ForeignKey("Match")]
        public int MatchNo { get; set; }

        [Key]
        [Column(Order = 200)]
        public int SetNo { get; set; }

        [Key]
        [Column(Order = 300)]
        public int ShortCode { get; set; }

        [Required]
        public Match Match { get; set; }
    }
}
