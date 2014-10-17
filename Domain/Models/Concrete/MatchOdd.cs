using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Concrete
{
    public class MatchOdd
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MatchOddId { get; set; }

        [ForeignKey("Match")]
        [Column(Order = 100)]
        public int BetServiceMatchNo { get; set; }

        [ForeignKey("BetOption")]
        [Column(Order = 200)]
        public int BetOptionId { get; set; }

        public decimal Odd { get; set; }

        public DateTime? LastUpdateTime { get; set; }

        public virtual BetOption BetOption { get; set; }

        public Match Match { get; set; }
    }
}
